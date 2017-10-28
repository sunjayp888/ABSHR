//using DocumentService.API.RESTClient.Interfaces;
//using DocumentService.API.RESTClient.Models;
using HR.Business.Extensions;
using HR.Business.Interfaces;
using HR.Business.Models;
using HR.Data.Extensions;
using HR.Data.Interfaces;
using HR.Entity;
using HR.Entity.Comparer;
using HR.Entity.Dto;
using HR.Entity.Interfaces;
using HR.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using SharedTypes.DataContracts;


namespace HR.Business
{
    public partial class HRBusinessService : IHRBusinessService, ITenantOrganisationService
    {
        private DateTime _defaultDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        private IHRDataService _hrDataService;
        private ICacheProvider _cacheProvider;
        private ITemplateService _templateService;
        private Interfaces.IEmailService _emailService;
        //   private IDocumentServiceRestClient _documentServiceAPI;
        private enum ShowColour { Company, Department, Team };
        private const string OrganisationCacheKey = "Organisations";
        private const string OrganisationEmploymentsTreeKey = "OrganisationEmploymentsTree";
        private const string AbsenceStatusTemplateKey = "HRAbsenceStatus";
        private const string OvertimeStatusTemplateKey = "HROvertimeStatus";
        private const string ApprovalAbsenceTemplateKey = "HRApprovalAbsence";
        private const string ApprovalOvertimeTemplateKey = "HRApprovalOvertime";
        private object lockObject = new object();
        #region DocumentService data
        readonly string PersonnelPhotoKey = "PersonnelPhoto";
        readonly string PersonnelProfileCategory = "ProfileImage";
        readonly string JobTitleDocumentCategory = "JobTitleDocument";
        readonly string JobTitleDocumentKey = "JobTitleDocuments";
        #endregion

        public HRBusinessService(IHRDataService hrDataService, ICacheProvider cacheProvider, ITemplateService templateService, Interfaces.IEmailService emailService)//, IDocumentServiceRestClient documentServiceAPI)
        {
            _hrDataService = hrDataService;
            _cacheProvider = cacheProvider;
            _templateService = templateService;
            _emailService = emailService;
            //     _documentServiceAPI = documentServiceAPI;
        }

        private void SendAbsenceStatusMessage(int organisationId, Absence absence)
        {
            var status = "Requested";
            switch ((ApprovalStates)absence.ApprovalStateId)
            {
                case ApprovalStates.Approved:
                    status = "Approved";
                    break;
                case ApprovalStates.Declined:
                    status = "Declined";
                    break;
            }

            var employmentsTree = RetrieveEmploymentsTree(absence.OrganisationId);

            if (absence.PersonnelAbsenceEntitlement == null)
                absence = _hrDataService.RetrieveAbsence(absence.OrganisationId, absence.AbsenceId);

            var personnelNode = employmentsTree.SelectMany(tree => tree.All).FirstOrDefault(node => node.Value.PersonnelId == absence.PersonnelAbsenceEntitlement.PersonnelId);
            if (personnelNode == null)
                return;

            // Email personnel the absence status
            var absenceStatusMessage = new AbsenceStatusMessage
            {
                Duration = Convert.ToDecimal(absence.AbsenceDays.Sum(d => d.Duration)),
                AbsenceType = absence.AbsenceType.Name,
                Name = personnelNode.Value.Personnel.Fullname,
                Status = status.ToLower(),
                From = absence.AbsenceDays.Min(day => day.Date).Date.ToLongDateString(),
                To = absence.AbsenceDays.Max(day => day.Date).Date.ToLongDateString(),
                LinkUrl = string.Format("{0}/Absence/Edit/{1}/{2}", HttpContext.Current.Request.Url.Authority, absence.PersonnelAbsenceEntitlement.PersonnelId, absence.AbsenceId),
                LinkText = "Click here to view absence."
            };

            var emailBody = _templateService.CreateText(organisationId, JsonConvert.SerializeObject(absenceStatusMessage), AbsenceStatusTemplateKey);

            var emailData = new EmailData
            {
                FromAddress = ConfigHelper.EmailDefaultFromAddress,
                ToAddressList = new List<string> { personnelNode.Value.Personnel.Email },
                Subject = string.Format("Absence {0}", status),
                Body = emailBody,
                IsHtml = true
            };

            if (!string.IsNullOrWhiteSpace(personnelNode.Value.Personnel.Email) && !string.IsNullOrWhiteSpace(ConfigHelper.EmailDefaultFromAddress))
                _emailService.SendEmail(emailData);

        }

        private void SendOvertimeStatusMessage(Overtime overtime)
        {
            var status = "Requested";
            switch ((ApprovalStates)overtime.ApprovalStateId)
            {
                case ApprovalStates.Approved:
                    status = "Approved";
                    break;
                case ApprovalStates.Declined:
                    status = "Declined";
                    break;
            }

            var employmentsTree = RetrieveEmploymentsTree(overtime.OrganisationId);

            var personnelNode = employmentsTree.SelectMany(tree => tree.All).FirstOrDefault(node => node.Value.PersonnelId == overtime.PersonnelId);
            if (personnelNode == null)
                return;

            overtime.OvertimePreference = _hrDataService.Retrieve<OvertimePreference>(overtime.OrganisationId, overtime.OvertimePreferenceId);

            // Email personnel the overtime status
            var overtimeStatusMessage = new ApprovalEmailOvetime
            {
                OvertimePreference = overtime.OvertimePreference.Name,
                PersonnelName = personnelNode.Value.Personnel.Fullname,
                ApprovalState = status.ToLower(),
                DateOfOvertime = overtime.Date.ToLongDateString(),
                Hours = overtime.Hours,
                LinkUrl = string.Format("{0}/Overtime/Edit/{1}/{2}", HttpContext.Current.Request.Url.Authority, overtime.PersonnelId, overtime.OvertimeId),
                LinkText = "Click here to view the overtime.",
                Reason = overtime.Reason
            };

            //var emailBody = _templateService.CreateText(JsonConvert.SerializeObject(overtimeStatusMessage), OvertimeStatusTemplateKey);

            var emailData = new EmailData
            {
                FromAddress = ConfigHelper.EmailDefaultFromAddress,
                ToAddressList = new List<string> { personnelNode.Value.Personnel.Email },
                Subject = string.Format("Overtime {0}", status),
                Body = "Testing",
                IsHtml = true
            };

            if (!string.IsNullOrWhiteSpace(personnelNode.Value.Personnel.Email) && !string.IsNullOrWhiteSpace(ConfigHelper.EmailDefaultFromAddress))
                _emailService.SendEmail(emailData);
        }

        #region // Mappers 

        private AbsenceRange AbsenceRangeUpdatePeriodAndCountry(PersonnelAbsenceEntitlement personnelAbsenceEntitlement, AbsenceRange absenceRange)
        {
            absenceRange.AbsencePeriodId = personnelAbsenceEntitlement.AbsencePolicyPeriod.AbsencePeriodId;
            return absenceRange;
        }

        public IEnumerable<AbsenceDay> AbsenceRangeToAbsenceDays(AbsenceRange absenceRange, WorkingPattern workingPattern, bool returnUnbookableDays = false)
        {
            if (absenceRange == null)
                return null;

            var cannotBeBookedDays = RetrieveCannotBeBookedDays(absenceRange, workingPattern);

            return absenceRange
                .BeginDateUtc
                .RangeTo(absenceRange.EndDateUtc)
                .ToAbsenceDayList(absenceRange)
                .ToAbsenceDaysSummary(cannotBeBookedDays, returnUnbookableDays)
                .ToList();

        }

        public Absence AbsenceRangeToAbsence(AbsenceRange absenceRange, WorkingPattern workingPattern)
        {
            var absence = new Absence
            {
                AbsenceId = absenceRange.AbsenceId ?? 0,
                OrganisationId = absenceRange.OrganisationId,
                PersonnelAbsenceEntitlementId = absenceRange.PersonnelAbsenceEntitlementId,
                AbsenceTypeId = absenceRange.AbsenceTypeId,
                Description = absenceRange.Description,
                ApprovalStateId = absenceRange.ApprovalStateId,
                AbsenceStatusByUser = absenceRange.AbsenceStatusByUser,
                AbsenceStatusDateTimeUtc = absenceRange.AbsenceStatusDateTimeUtc,
                DoctorsNoteSupplied = absenceRange.DoctorsNoteSupplied,
                ReturnToWorkCompleted = absenceRange.ReturnToWorkCompleted,
                AbsenceDays = AbsenceRangeToAbsenceDays(absenceRange, workingPattern).ToList()
            };

            return absence;
        }

        public AbsenceRange AbsenceToAbsenceRange(Absence absence)
        {
            var daysOrdered = absence.AbsenceDays.OrderBy(d => d.Date);
            var beginDay = daysOrdered.FirstOrDefault();
            var endDay = daysOrdered.LastOrDefault();
            var employment =
                _hrDataService.RetrieveEmployment(absence.OrganisationId,
                    absence.PersonnelAbsenceEntitlement.PersonnelId, beginDay.Date);
            var jobTitleName =
                _hrDataService.RetrieveJobTitle(absence.OrganisationId, Convert.ToInt32(employment.JobTitleId), p => true).Name;
            var jobGradeName =
               _hrDataService.RetrieveJobGrade(absence.OrganisationId, Convert.ToInt32(employment.JobGradeId), p => true).Name;
            return new AbsenceRange
            {
                AbsenceId = absence.AbsenceId,
                OrganisationId = absence.OrganisationId,
                PersonnelAbsenceEntitlementId = absence.PersonnelAbsenceEntitlementId,
                AbsenceTypeId = absence.AbsenceTypeId,
                AbsenceTypeName = absence.AbsenceType.Name,
                Description = absence.Description,
                ApprovalStateId = absence.ApprovalStateId,
                ApprovalState = absence.ApprovalState,
                AbsenceStatusByUser = absence.AbsenceStatusByUser,
                AbsenceStatusDateTimeUtc = absence.AbsenceStatusDateTimeUtc,
                Days = Convert.ToDecimal(daysOrdered.Sum(d => d.Duration)),
                DoctorsNoteSupplied = absence.DoctorsNoteSupplied,
                ReturnToWorkCompleted = absence.ReturnToWorkCompleted,
                PersonnelId = absence.PersonnelAbsenceEntitlement.PersonnelId,
                BeginDateUtc = beginDay.Date,
                BeginAbsencePart = beginDay.AbsencePart,
                EndDateUtc = endDay.Date,
                EndAbsencePart = endDay.AbsencePart,
                Employment = jobTitleName + " - " + jobGradeName + " - " + employment.StartDate.ToString("dd MMMM yyyy"),
                CanApproveAbsence = null


            };
        }

        #endregion


        #region // Create

        public PersonnelAbsenceEntitlement RemoveAbsenceFromPersonnelAbsenceEntitlementValues(Absence absence, PersonnelAbsenceEntitlement personnelAbsenceEntitlement)
        {
            if (absence == null ||
                personnelAbsenceEntitlement == null ||
                absence.PersonnelAbsenceEntitlementId != personnelAbsenceEntitlement.PersonnelAbsenceEntitlementId ||
                absence.AbsenceDays == null ||
                !absence.AbsenceDays.Any())
                return personnelAbsenceEntitlement;

            var duration = CalculateDuration(absence?.AbsenceDays);

            if (personnelAbsenceEntitlement.Entitlement > 0)
            {
                personnelAbsenceEntitlement.Used -= duration;
                personnelAbsenceEntitlement.Remaining += duration;
            }

            return personnelAbsenceEntitlement;
        }

        public PersonnelAbsenceEntitlement AddAbsenceToPersonnelAbsenceEntitlementValues(Absence absence, PersonnelAbsenceEntitlement personnelAbsenceEntitlement)
        {
            if (absence == null ||
                personnelAbsenceEntitlement == null ||
                absence.AbsenceDays == null ||
                !absence.AbsenceDays.Any() ||
                absence.AbsenceType == null)
                return null;

            var duration = CalculateDuration(absence?.AbsenceDays);

            if (personnelAbsenceEntitlement.Entitlement > 0)
            {
                personnelAbsenceEntitlement.Used += duration;
                personnelAbsenceEntitlement.Remaining -= duration;
            }

            return personnelAbsenceEntitlement;
        }



        public double CalculateDuration(IEnumerable<AbsenceDay> absenceDays)
        {
            return absenceDays?.Sum(day => day.Duration) ?? 0;
        }

        public ValidationResult CreateAbsence(int organisationId, AbsenceRange absenceRange)
        {
            var validate = ValidateAbsence(organisationId, absenceRange);
            if (!validate.Succeeded)
                return validate;

            var employment = RetrieveEmployment(organisationId, absenceRange.PersonnelId, absenceRange.BeginDateUtc);
            var personnelAbsenceEntitlement = RetrievePersonnelAbsenceEntitlement(organisationId, absenceRange, employment.EmploymentId);
            absenceRange = AbsenceRangeUpdatePeriodAndCountry(personnelAbsenceEntitlement, absenceRange);
            var absence = _hrDataService.CreateAbsence(organisationId, AbsenceRangeToAbsence(absenceRange, employment.WorkingPattern));

            absenceRange.AbsenceId = absence.AbsenceId;

            UpdateAbsencePersonnelAbsenceEntitlement(organisationId, absence, null);

            SendAbsenceStatusMessage(organisationId, absence);

            CreateAppovalRows(organisationId, absenceRange.PersonnelId, ApprovalTypes.Absence, absenceRange.AbsenceId.Value);
            StartApprovalProcess(organisationId, ApprovalTypes.Absence, absenceRange.AbsenceId.Value, string.Empty);


            return validate;

        }

        private Absence CanBeBookAbsenceDay(AbsenceRange absenceRange, IEnumerable<INotAbsenceDay> cannotBookAbsenceDays)
        {
            var absenceDayList = absenceRange.BeginDateUtc.RangeTo(absenceRange.EndDateUtc).ToAbsenceDayList(absenceRange).ToList();

            var absenceDaysCanBeBooked = (from a in absenceDayList
                                          from c in cannotBookAbsenceDays
                                          where (a.AM == c.AM
                                                || a.PM == c.PM) && a.Date == c.Date
                                          select new AbsenceDay()
                                          {
                                              AM = a.AM == c.AM,
                                              PM = a.PM == c.PM,
                                              Date = a.Date,
                                              Duration = a.Duration
                                          }).ToList();

            if (!cannotBookAbsenceDays.Any())
                absenceDaysCanBeBooked = absenceDayList;

            var absence = new Absence
            {
                AbsenceId = absenceRange.AbsenceId ?? 0,
                OrganisationId = absenceRange.OrganisationId,
                PersonnelAbsenceEntitlementId = absenceRange.PersonnelAbsenceEntitlementId,
                AbsenceTypeId = absenceRange.AbsenceTypeId,
                Description = absenceRange.Description,
                //AbsenceStatusId = absenceRange.AbsenceStatusId,
                ApprovalStateId = absenceRange.ApprovalStateId,
                AbsenceStatusByUser = absenceRange.AbsenceStatusByUser,
                AbsenceStatusDateTimeUtc = absenceRange.AbsenceStatusDateTimeUtc,
                DoctorsNoteSupplied = absenceRange.DoctorsNoteSupplied,
                ReturnToWorkCompleted = absenceRange.ReturnToWorkCompleted,
                AbsenceDays = absenceDaysCanBeBooked
            };
            return absence;
        }

        public ValidationResult<AbsencePeriod> CreateAbsencePeriod(int organisationId, AbsencePeriod absencePeriod)
        {
            var validationResult = AbsencePeriodAlreadyExists(organisationId, null, absencePeriod.StartDate, absencePeriod.EndDate);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.CreateAbsencePeriod(organisationId, absencePeriod);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.Message };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<AbsenceType> CreateAbsenceType(int organisationId, AbsenceType absenceType)
        {
            var validationResult = AbsenceTypeAlreadyExists(organisationId, null, absenceType.Name);
            if (!validationResult.Succeeded)
                return validationResult;

            try
            {
                validationResult.Entity = _hrDataService.CreateAbsenceType(organisationId, absenceType);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<ApprovalLevelUser> CreateApprovalLevelUser(int organisationId, ApprovalLevelUser approvalLevelUser)
        {
            var validationResult = ApprovalLevelUserAlreadyExists(organisationId, approvalLevelUser.ApprovalLevelId, null, approvalLevelUser.AspNetUserId);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.Create(organisationId, approvalLevelUser);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<ApprovalModel> CreateApprovalModel(int organisationId, ApprovalModel approvalModel)
        {
            var validationResult = ApprovalModelAlreadyExists(organisationId, null, approvalModel.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.Create(organisationId, approvalModel);
                for (int count = 1; count <= 3; count++)
                {
                    var approvalLevel = new ApprovalLevel
                    {
                        ApprovalModelId = validationResult.Entity.ApprovalModelId,
                        LevelNumber = count
                    };
                    _hrDataService.Create(organisationId, approvalLevel);
                };
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public Building CreateBuilding(int organisationId, Building building)
        {
            return _hrDataService.CreateBuilding(organisationId, building);
        }

        public ValidationResult<Country> CreateCountry(int organisationId, Country country)
        {
            var validationResult = CountryAlreadyExists(organisationId, null, country.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.CreateCountry(organisationId, country);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }


        public ValidationResult<Company> CreateCompany(int organisationId, Company company)
        {
            var validationResult = CompanyAlreadyExists(organisationId, null, company.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.CreateCompany(organisationId, company);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<Department> CreateDepartment(int organisationId, Department department)
        {
            var validationResult = DepartmentAlreadyExists(organisationId, null, department.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.CreateDepartment(organisationId, department);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<Overtime> CreateOvertime(int organisationId, Overtime overtime)
        {
            var employment = _hrDataService.RetrieveEmployment(organisationId, overtime.PersonnelId, _defaultDate);
            var validOvertimeDate = (employment.EndDate == null || employment.EndDate.Value.Date > overtime.Date.Date) && (employment.TerminationDate == null || employment.TerminationDate.Value.Date > overtime.Date.Date);
            var overtimeValidation = new ValidationResult<Overtime>
            {
                Succeeded = validOvertimeDate,
                Errors = !validOvertimeDate ? new List<string> { "Cannot file overtime on or beyond end date or temination date." } : null
            };

            try
            {
                overtimeValidation.Entity = _hrDataService.Create(organisationId, overtime);
                SendOvertimeStatusMessage(overtime);
                CreateAppovalRows(organisationId, overtime.PersonnelId, ApprovalTypes.Overtime, overtime.OvertimeId);
                StartApprovalProcess(organisationId, ApprovalTypes.Overtime, overtime.OvertimeId, overtime.CreatedBy);
            }
            catch (Exception ex)
            {
                overtimeValidation.Succeeded = false;
                overtimeValidation.Errors = new List<string> { ex.InnerMessage() };
                overtimeValidation.Exception = ex;
            }
            return overtimeValidation;
        }


        public Employment CreateEmployment(int organisationId, Employment employment, IEnumerable<WorkingPatternDay> workingPatternDays)
        {
            if (workingPatternDays != null)
            {
                var workingPattern = _hrDataService.CreateWorkingPattern(organisationId, new WorkingPattern
                {
                    WorkingPatternDays = workingPatternDays.ToList()
                });

                employment.WorkingPatternId = workingPattern.WorkingPatternId;
            }
            employment = _hrDataService.CreateEmployment(organisationId, employment);
            return employment;
        }

        public EmergencyContact CreateEmergencyContact(int organisationId, EmergencyContact emergencyContact)
        {
            return _hrDataService.CreateEmergencyContact(organisationId, emergencyContact);
        }

        public void CreateJobTitleDocument(int organisationId, JobTitleDocument jobTitleDocument, string createdBy)
        {
            var organisation = RetrieveOrganisation(organisationId);
            //_documentServiceAPI.CreateDocument(
            //    new Document
            //    {
            //        Product = organisation.Name,
            //        Category = JobTitleDocumentCategory,
            //        Content = jobTitleDocument.DocumentBytes,
            //        Description = jobTitleDocument.Name,
            //        FileName = jobTitleDocument.Attachment.FileName.Split('\\').Last(), //Internet explorer includes the filepath which causes the issue
            //        PayrollId = jobTitleDocument.JobTitleId.ToString(),
            //        EmployeeName = jobTitleDocument.Name,
            //        CreatedBy = createdBy,
            //        DocumentAttribute = new List<DocumentAttribute>
            //        {
            //            new DocumentAttribute
            //            {
            //                Key = JobTitleDocumentKey,
            //                Value = jobTitleDocument.JobTitleId.ToString()
            //            }
            //        }
            //    });
        }

        public Personnel CreatePersonnel(int organisationId, Personnel personnel, Employment employment, bool overrideDefaultWorkingPattern, IEnumerable<WorkingPatternDay> workingPatternDays)
        {
            var createdPersonnel = CreatePersonnel(organisationId, personnel);
            return createdPersonnel;
        }

        public ValidationResult<PersonnelApprovalModel> CreatePersonnelApprovalModel(int organisationId, PersonnelApprovalModel personnelApprovalModel)
        {
            var validationResult = PersonnelApprovalModelAlreadyExists(organisationId, null, personnelApprovalModel.PersonnelId, personnelApprovalModel.ApprovalModelId, personnelApprovalModel.ApprovalEntityTypeId);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.Create(organisationId, personnelApprovalModel);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<EmploymentDepartment> CreateEmploymentDepartment(int organisationId, EmploymentDepartment employmentDepartment)
        {
            var validationResult = EmploymentDepartmentAlreadyExists(organisationId, employmentDepartment.EmploymentId, employmentDepartment.DepartmentId);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                employmentDepartment.OrganisationId = organisationId;
                validationResult.Entity = _hrDataService.Create(organisationId, employmentDepartment);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<EmploymentTeam> CreateEmploymentTeam(int organisationId, EmploymentTeam employmentTeam)
        {
            var validationResult = EmploymentTeamAlreadyExists(organisationId, employmentTeam.EmploymentId, employmentTeam.TeamId);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                employmentTeam.OrganisationId = organisationId;
                validationResult.Entity = _hrDataService.Create(organisationId, employmentTeam);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public void CreateOvertimeAdjustment(int organisationId, OvertimeSummary overtimeSummary, string createdBy, string comment)
        {
            if (overtimeSummary.PaidHours != 0)
            {
                Overtime overtime = new Overtime
                {
                    OvertimePreferenceId = (int)OvertimePreferences.Paid,
                    PersonnelId = overtimeSummary.PersonnelId,
                    Date = DateTime.Now,
                    Hours = overtimeSummary.PaidHours,
                    Reason = "Adjustment",
                    Comment = comment,
                    CreatedBy = createdBy,
                    ApprovalStateId = (int)ApprovalStates.Approved
                };
                overtime = _hrDataService.Create(organisationId, overtime);
            }
            if (overtimeSummary.TOILHours != 0)
            {
                Overtime overtime = new Overtime
                {
                    OvertimePreferenceId = (int)OvertimePreferences.TOIL,
                    PersonnelId = overtimeSummary.PersonnelId,
                    Date = DateTime.Now,
                    Hours = overtimeSummary.TOILHours,
                    Reason = "Adjustment",
                    Comment = comment,
                    CreatedBy = createdBy,
                    ApprovalStateId = (int)ApprovalStates.Approved
                };
                overtime = _hrDataService.Create(organisationId, overtime);
            }
        }

        private Personnel CreatePersonnel(int organisationId, Personnel personnel)
        {
            personnel = _hrDataService.CreatePersonnel(organisationId, personnel);
            return personnel;
        }

        private void CreateAppovalRows(int organisationId, int personnelId, ApprovalTypes approvalEntityTypes, int entityId)
        {
            var personnelApprovalModel = _hrDataService.PersonnelApprovalModels(organisationId, personnelId, approvalEntityTypes);
            if (personnelApprovalModel != null)
            {
                var approvals = personnelApprovalModel.ApprovalModel.ApprovalLevels
                    .Select(pam => new Approval
                    {
                        EntityId = entityId,
                        ApprovalLevelId = pam.ApprovalLevelId,
                        ApprovalStateId = (int)ApprovalStates.Requested,
                        ApprovalEntityTypeId = (int)approvalEntityTypes
                    });
                _hrDataService.Create(organisationId, approvals);
            }
        }

        private void StartApprovalProcess(int organisationId, ApprovalTypes approvalEntityTypes, int entityId, string createdBy)
        {
            var approvers = _hrDataService.RetrieveNextApprovers(organisationId, approvalEntityTypes, entityId);
            if (!approvers.Any())
            {
                if (approvalEntityTypes == ApprovalTypes.Absence)
                {
                    AbsenceApproved(organisationId, entityId, createdBy);
                }
                else if (approvalEntityTypes == ApprovalTypes.Overtime)
                {
                    OvertimeApproved(organisationId, entityId, createdBy);
                }
            }
            else
            {
                if (approvalEntityTypes == ApprovalTypes.Absence)
                {
                    //Task.Run(() => EmailAbsenceApprovers(organisationId, entityId, approvers));
                    EmailAbsenceApprovers(organisationId, entityId, approvers);
                }
                else if (approvalEntityTypes == ApprovalTypes.Overtime)
                {
                    //Task.Run(() => EmailOvetimeApprovers(organisationId, entityId, approvers));
                    EmailOvetimeApprovers(organisationId, entityId, approvers);
                }
            }
        }

        private void EmailAbsenceApprovers(int organisationId, int entityId, IEnumerable<Approver> approvers)
        {
            var absence = _hrDataService.RetrieveAbsence(organisationId, entityId);
            var approvalEmailAbsences = approvers.Select(a => new ApprovalEmailAbsence
            {
                Duration = absence.AbsenceDays.Count(),
                ApprovalState = absence.ApprovalState.Name,
                From = absence.AbsenceDays.Min(ad => ad.Date).ToLongDateString(),
                To = absence.AbsenceDays.Max(ad => ad.Date).ToLongDateString(),
                Description = absence.Description,
                Email = a.Email,
                LevelNumber = a.LevelNumber,
                LinkUrl = string.Format("{0}/Absence/Edit/{1}/{2}", HttpContext.Current.Request.Url.Authority, absence.PersonnelAbsenceEntitlement.PersonnelId, absence.AbsenceId),
                LinkText = "Click here to view the absence.",
                PersonnelId = absence.PersonnelAbsenceEntitlement.PersonnelId,
                PersonnelName = absence.PersonnelAbsenceEntitlement.Personnel.Fullname,
                AbsenceType = absence.AbsenceType.Name
            }).FirstOrDefault();


            var emailBody = _templateService.CreateText(organisationId, JsonConvert.SerializeObject(approvalEmailAbsences), ApprovalAbsenceTemplateKey);

            var emailData = new EmailData
            {
                FromAddress = ConfigHelper.EmailDefaultFromAddress,
                ToAddressList = approvers.Select(a => a.Email).ToList(),
                Subject = string.Format("Absence for {0}", absence.PersonnelAbsenceEntitlement.Personnel.Fullname),
                Body = emailBody,
                IsHtml = true
            };

            _emailService.SendEmail(emailData);
        }

        private void EmailOvetimeApprovers(int organisationId, int entityId, IEnumerable<Approver> approvers)
        {
            var overtime = _hrDataService.RetrieveOvertime(organisationId, entityId);
            var approvalEmailOvertimes = approvers.Select(a => new ApprovalEmailOvetime
            {
                ApprovalState = overtime.ApprovalState.Name,
                DateOfOvertime = overtime.Date.ToLongDateString(),
                Hours = overtime.Hours,
                OvertimePreference = overtime.OvertimePreference.Name,
                Email = a.Email,
                LevelNumber = a.LevelNumber,
                LinkUrl = string.Format("{0}/Overtime/Edit/{1}/{2}", HttpContext.Current.Request.Url.Authority, overtime.Personnel.PersonnelId, overtime.OvertimeId),
                LinkText = "Click here to view the overtime.",
                PersonnelId = overtime.Personnel.PersonnelId,
                PersonnelName = overtime.Personnel.Fullname,
                Reason = overtime.Reason
            }).FirstOrDefault();

            var emailBody = _templateService.CreateText(organisationId, JsonConvert.SerializeObject(approvalEmailOvertimes), ApprovalOvertimeTemplateKey);

            var emailData = new EmailData
            {
                FromAddress = ConfigHelper.EmailDefaultFromAddress,
                ToAddressList = approvers.Select(a => a.Email).ToList(),
                Subject = string.Format("Overtime for {0}", overtime.Personnel.Fullname),
                Body = emailBody,
                IsHtml = true
            };

            _emailService.SendEmail(emailData);
        }

        public void UploadPhoto(int organisationId, int personnelId, byte[] photo)
        {
            var personnel = _hrDataService.RetrievePersonnel(organisationId, personnelId, x => true);
            var organisation = RetrieveOrganisation(organisationId);
            //var document = RetrieveDocumentPhoto(organisationId, personnelId);

            //if (document != null)
            //{
            //    DeleteDocument(document.DocumentGuid);
            //}

            //_documentServiceAPI.CreateDocument(
            //            new Document
            //            {
            //                Product = organisation.Name,
            //                Category = PersonnelProfileCategory,
            //                Content = photo,
            //                Description = "Profile Picture",
            //                FileName = personnel.Fullname + ".jpg",
            //                PayrollId = personnelId.ToString(),
            //                EmployeeName = personnel.Fullname,
            //                CreatedBy = personnelId.ToString(),
            //                DocumentAttribute = new List<DocumentAttribute>
            //                {
            //                    new DocumentAttribute
            //                    {
            //                        Key = PersonnelPhotoKey,
            //                        Value = personnelId.ToString()
            //                    }
            //                }
            //            });

            string cacheKey = PersonnelPhotoKey + personnelId;
            //remove cache if there is existing photo
            if (_cacheProvider.IsSet(cacheKey))
                _cacheProvider.Invalidate(cacheKey);
            //add chache
            _cacheProvider.Set(cacheKey, Convert.ToBase64String(photo), ConfigHelper.CacheTimeout);

        }

        public ValidationResult<EmploymentType> CreateEmploymentType(int organisationId, EmploymentType employmentType)
        {
            var validationResult = EmploymentTypeAlreadyExists(organisationId, null, employmentType.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.Create(organisationId, employmentType);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<Team> CreateTeam(int organisationId, Team team)
        {
            var validationResult = TeamAlreadyExists(organisationId, null, team.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.Create(organisationId, team);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<JobGrade> CreateJobGrade(int organisationId, JobGrade jobGrade)
        {
            var validationResult = JobGradeAlreadyExists(organisationId, null, jobGrade.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.Create(organisationId, jobGrade);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="jobTitle"></param>
        /// <returns></returns>
        public ValidationResult<JobTitle> CreateJobTitle(int organisationId, JobTitle jobTitle)
        {
            var validationResult = JobTitleAlreadyExists(organisationId, null, jobTitle.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.Create(organisationId, jobTitle);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<Site> CreateSite(int organisationId, Site site)
        {
            var validationResult = SiteAlreadyExists(organisationId, null, site.CountryId, site.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.CreateSite(organisationId, site);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public WorkingPattern CreateWorkingPatternDays(int organisationId, IEnumerable<WorkingPatternDay> workingPatternDays)
        {
            return _hrDataService.CreateWorkingPatternWithDays(organisationId, workingPatternDays);
        }

        public CompanyBuilding CreateCompanyBuilding(int organisationId, int companyId, int buildingId)
        {
            return _hrDataService.CreateCompanyBuilding(organisationId, companyId, buildingId);

        }
        #endregion

        #region // Retrieve

        private ValidationResult<AbsenceType> AbsenceTypeAlreadyExists(int organisationId, int? absenceTypeId, string name)
        {
            var alreadyExists =
               _hrDataService.RetrieveAbsenceTypes(organisationId, at => at.Name.ToLower() == name.Trim().ToLower() && at.AbsenceTypeId != (absenceTypeId ?? -1))
                    .Items.Any();
            return new ValidationResult<AbsenceType>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "Absence type already exists." } : null
            };
        }

        private ValidationResult<AbsencePeriod> AbsencePeriodAlreadyExists(int organisationId, int? absencePeriodId, DateTime startDate, DateTime endDate)
        {
            var alreadyExists =
               _hrDataService.RetrieveAbsencePeriods(organisationId, ap => ap.StartDate == startDate && ap.EndDate == endDate && ap.AbsencePeriodId != (absencePeriodId ?? -1), null, null)
                    .Items.Any();
            return new ValidationResult<AbsencePeriod>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "Absence Period already exists." } : null
            };
        }

        private ValidationResult<ApprovalLevelUser> ApprovalLevelUserAlreadyExists(int organisationId, int approvalLevelId, int? approvalLevelUserId, string aspNetUsersId)
        {
            var alreadyExists = _hrDataService.Retrieve<ApprovalLevelUser>(organisationId, alu => alu.AspNetUserId == aspNetUsersId && alu.ApprovalLevelId == approvalLevelId && alu.ApprovalLevelUserId != (approvalLevelUserId ?? -1)).Any();
            return new ValidationResult<ApprovalLevelUser>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "Approval Level User already exists." } : null
            };
        }

        private ValidationResult<ApprovalModel> ApprovalModelAlreadyExists(int organisationId, int? approvalModelId, string name)
        {
            var alreadyExists = _hrDataService.Retrieve<ApprovalModel>(organisationId, am => am.Name.ToLower() == name.Trim().ToLower() && am.ApprovalModelId != (approvalModelId ?? -1)).Any();
            return new ValidationResult<ApprovalModel>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "Approval Model already exists." } : null
            };
        }

        private ValidationResult<Country> CountryAlreadyExists(int organisationId, int? countryId, string name)
        {
            var alreadyExists =
                _hrDataService.RetrieveCountries(organisationId,
                    c => c.Name.ToLower() == name.Trim().ToLower() && c.OrganisationId == organisationId
                         && c.CountryId != (countryId ?? -1)).Items.Any();

            return new ValidationResult<Country>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "Country already exists." } : null
            };
        }

        private ValidationResult<Company> CompanyAlreadyExists(int organisationId, int? companyId, string name)
        {
            var alreadyExists = _hrDataService.RetrieveCompanies(organisationId,
                c => c.Name.ToLower() == name.Trim().ToLower() && c.CompanyId != (companyId ?? -1)).Items.Any();
            return new ValidationResult<Company>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "Company already exists." } : null
            };
        }

        private ValidationResult<Department> DepartmentAlreadyExists(int organisationId, int? departmentId, string name)
        {
            var alreadyExists =
                _hrDataService.RetrieveDepartments(organisationId, d => d.Name.ToLower() == name.Trim().ToLower() && d.DepartmentId != (departmentId ?? -1))
                    .Items.Any();
            return new ValidationResult<Department>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "Department already exists." } : null
            };
        }

        private ValidationResult<EmploymentDepartment> EmploymentDepartmentAlreadyExists(int organisationId, int employmentID, int teamId)
        {
            var alreadyExists = _hrDataService.Retrieve<EmploymentDepartment>(organisationId, d => d.OrganisationId == organisationId && d.EmploymentId == employmentID && d.DepartmentId == teamId).Any();
            return new ValidationResult<EmploymentDepartment>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "EmploymentDepartment already exists." } : null
            };
        }

        private ValidationResult<EmploymentTeam> EmploymentTeamAlreadyExists(int organisationId, int employmentID, int teamId)
        {
            var alreadyExists = _hrDataService.Retrieve<EmploymentTeam>(organisationId, d => d.OrganisationId == organisationId && d.EmploymentId == employmentID && d.TeamId == teamId).Any();
            return new ValidationResult<EmploymentTeam>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "EmploymentTeam already exists." } : null
            };
        }

        private ValidationResult<EmploymentType> EmploymentTypeAlreadyExists(int organisationId, int? employmentTypeId, string name)
        {
            var alreadyExists = _hrDataService.Retrieve<EmploymentType>(organisationId, et => et.Name.ToLower() == name.Trim().ToLower() && et.EmploymentTypeId != (employmentTypeId ?? -1)).Any();
            return new ValidationResult<EmploymentType>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "EmploymentType already exists." } : null
            };
        }

        private ValidationResult<Team> TeamAlreadyExists(int organisationId, int? teamId, string name)
        {
            var alreadyExists = _hrDataService.Retrieve<Team>(organisationId, d => d.Name.ToLower() == name.Trim().ToLower() && d.TeamId != (teamId ?? -1)).Any();
            return new ValidationResult<Team>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "Team already exists." } : null
            };
        }

        private ValidationResult<JobGrade> JobGradeAlreadyExists(int organisationId, int? JobGradeId, string name)
        {
            var alreadyExists = _hrDataService.Retrieve<JobGrade>(organisationId, d => d.Name.ToLower() == name.Trim().ToLower() && d.JobGradeId != (JobGradeId ?? -1)).Any();
            return new ValidationResult<JobGrade>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "JobGrade already exists." } : null
            };
        }

        private ValidationResult<JobTitle> JobTitleAlreadyExists(int organisationId, int? jobTitleId, string name)
        {
            var alreadyExists = _hrDataService.Retrieve<JobTitle>(organisationId, d => d.Name.ToLower() == name.Trim().ToLower() && d.JobTitleId != (jobTitleId ?? -1)).Any();
            return new ValidationResult<JobTitle>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "JobTitle already exists." } : null
            };
        }

        private ValidationResult<PersonnelApprovalModel> PersonnelApprovalModelAlreadyExists(int organisationId, int? personnelApprovalModelId, int personnelId, int approvalModelId, int approvalEntityTypeId)
        {
            var alreadyExists = _hrDataService.Retrieve<PersonnelApprovalModel>(organisationId, pam => pam.PersonnelId == personnelId && pam.ApprovalModelId == approvalModelId && pam.ApprovalEntityTypeId == approvalEntityTypeId && pam.PersonnelApprovalModelId == (personnelApprovalModelId ?? -1)).Any();
            return new ValidationResult<PersonnelApprovalModel>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "Personnel Approval Model already exists." } : null
            };
        }

        private ValidationResult<Site> SiteAlreadyExists(int organisationId, int? siteId, int countryId, string name)
        {
            var alreadyExists = _hrDataService.RetrieveSites(organisationId, s => s.Name.ToLower() == name.Trim().ToLower() && s.CountryId == countryId && s.SiteId != (siteId ?? -1)).Items.Any();
            return new ValidationResult<Site>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "Site already exists." } : null
            };
        }

        private async Task ResetEmploymentsTree(int organisationId)
        {
            lock (lockObject)
            {
                var cacheKey = string.Format("{0}_{1}", OrganisationEmploymentsTreeKey, organisationId);

                var employment = _hrDataService.RetrieveCurrentEmployments(organisationId);
                var tree = Node<Employment>.CreateTree(employment, e => e.PersonnelId, e => e.ReportsToPersonnelId);

                _cacheProvider.Invalidate(cacheKey);
                _cacheProvider.Set(cacheKey, tree, 300);
            }

        }

        public bool CanApproveAbsence(int organisationId, int absenceId, bool isAdmin, string userId)
        {
            var approvers = _hrDataService.RetrieveNextApprovers(organisationId, ApprovalTypes.Absence, absenceId);
            return ((isAdmin && approvers.Any()) || approvers.Any(a => a.AspNetUserId == userId));
        }

        public bool CanApproveOvertime(int organisationId, int overtimeId, bool isAdmin, string userId)
        {
            var approvers = _hrDataService.RetrieveNextApprovers(organisationId, ApprovalTypes.Overtime, overtimeId);
            return ((isAdmin && approvers.Any()) || approvers.Any(a => a.AspNetUserId == userId));
        }

        public bool CanDeleteAbsencePeriod(int organisationId, int absencePeriodId)
        {
            return !_hrDataService.RetrieveAbsencePeriod(organisationId, absencePeriodId).AbsencePolicyPeriods.Any();
        }

        public bool CanDeleteAbsenceType(int organisationId, int absenceTypeId)
        {
            return _hrDataService.RetrieveAbsenceType(organisationId, absenceTypeId, p => true).AbsencePolicyEntitlements.All(c => c.AbsenceTypeId != absenceTypeId);
        }

        public bool CanDeleteApprovalModel(int organisationId, int approvalModelId)
        {
            return _hrDataService.Retrieve<PersonnelApprovalModel>(organisationId, pam => pam.ApprovalModelId == approvalModelId).Any();
        }

        public bool CanDeleteBuilding(int organisationId, int buildingId)
        {
            var result = false;
            var companyBuilding =
                _hrDataService.RetrieveCompanyBuilding(organisationId, p => p.BuildingId == buildingId).Any();
            var employments = _hrDataService.RetrieveEmployments(organisationId, p => p.BuildingId == buildingId).Any();
            if (companyBuilding || employments)
                result = true;
            return result;

        }

        public bool CanDeleteCountry(int organisationId, int countryId)
        {
            var country = _hrDataService.RetrieveSites(organisationId, p => p.CountryId == countryId).Items.Any();
            return country;
        }

        public bool CanDeleteCompany(int organisationId, int companyId)
        {
            return _hrDataService.RetrieveEmployments(organisationId, p => p.CompanyId == companyId).Any();
        }

        public bool CanDeleteDepartment(int organisationId, int departmentId)
        {
            return _hrDataService.Retrieve<EmploymentDepartment>(organisationId, e => e.DepartmentId == departmentId).Any();
        }

        public bool CanDeleteEmploymentType(int organisationId, int employmentTypeId)
        {
            return _hrDataService.Retrieve<Employment>(organisationId, e => e.EmploymentTypeId == employmentTypeId).Any();
        }

        //Todo: can delete the link table of Jobgrade which is Jobtitle
        public bool CanDeleteJobGrade(int organisationId, int jobGradeId)
        {
            return _hrDataService.Retrieve<JobTitleJobGrade>(organisationId, e => e.JobGradeId == jobGradeId).Any();
        }

        public bool CanDeleteJobTitle(int organisationId, int jobTitleId)
        {
            return _hrDataService.Retrieve<Employment>(organisationId, e => e.JobTitleId == jobTitleId).Any();
        }

        public bool CanDeleteJobTitleJobGrade(int organisationId, int jobTitleId, int jobGradeId)
        {
            var result = _hrDataService.Retrieve<Employment>(organisationId, e => e.JobTitleId == jobTitleId && e.JobGradeId == jobGradeId).Any();
            return result;
        }

        public bool CanDeleteJobTitleDocument(int organisationId, int jobTitleDocumentId)
        {
            //no table is dependent to JobTitleDocument
            return false;
        }

        public bool CanDeleteTeam(int organisationId, int teamId)
        {
            return _hrDataService.Retrieve<EmploymentTeam>(organisationId, e => e.TeamId == teamId).Any();
        }

        public bool CanDeleteSite(int organisationId, int siteId)
        {
            var site = _hrDataService.RetrieveSite(organisationId, siteId, p => true);
            return site.Buildings.Any();
        }

        public bool CanDeleteCompanyBuilding(int organisationId, int companyId, int buildingId)
        {
            var employment = _hrDataService.RetrieveEmployments(organisationId, p => p.BuildingId == buildingId && p.CompanyId == companyId).Any();
            return employment;
        }

        public IEnumerable<Node<Employment>> RetrieveEmploymentsTree(int organisationId)
        {
            lock (lockObject)
            {
                var cacheKey = string.Format("{0}_{1}", OrganisationEmploymentsTreeKey, organisationId);
                if (_cacheProvider.IsSet(cacheKey))
                    return (IEnumerable<Node<Employment>>)_cacheProvider.Get(cacheKey);

                var employment = _hrDataService.RetrieveCurrentEmployments(organisationId);
                var tree = Node<Employment>.CreateTree(employment, e => e.PersonnelId, e => e.ReportsToPersonnelId);

                _cacheProvider.Set(cacheKey, tree, 300);

                return tree;
            }

        }

        public Node<Employment> RetrievePersonnelEmploymentNode(int organisationId, int personnelId)
        {
            var employmentsTree = RetrieveEmploymentsTree(organisationId);
            var node = employmentsTree.SelectMany(tree => tree.All).FirstOrDefault(n => n.Value.PersonnelId == personnelId);
            return node;
        }

        public bool IsManagerOfPersonnel(int organisationId, int userPersonnelId, int? personnelId = null)
        {
            return RetrievePersonnelEmploymentNode(organisationId, userPersonnelId)?.Descendants?.Any(d => !personnelId.HasValue || d.Value.PersonnelId == personnelId) ?? false;
        }

        public bool CanDeletePersonnelEmployment(int organisationId, int personnelId, int employmentId)
        {
            return !_hrDataService.PersonnelEmploymentHasAbsences(organisationId, personnelId, employmentId);
        }

        public AbsenceRange RetrieveAbsenceRange(int organisationId, int absenceId)
        {
            var absence = _hrDataService.RetrieveAbsence(organisationId, absenceId);
            return AbsenceToAbsenceRange(absence);
        }

        public AbsenceRequest RetrieveAbsenceRequest(int organisationId, AbsenceRange absenceRange)
        {
            if (absenceRange == null)
                return null;
            var employment = RetrieveEmployment(organisationId, absenceRange.PersonnelId, absenceRange.BeginDateUtc);
            if (employment == null)
                return null;
            absenceRange.PublicHolidayPolicyId = employment.PublicHolidayPolicyId;
            var personnelAbsenceEntitlement = RetrievePersonnelAbsenceEntitlement(organisationId, absenceRange, employment.EmploymentId);
            var absenceTypes = employment.AbsencePolicy.AbsencePolicyEntitlements.Select(item => item.AbsenceType).ToList();
            var absenceType = personnelAbsenceEntitlement.AbsenceType;
            var selectedAbsenceType = absenceType ?? absenceTypes.FirstOrDefault();
            absenceRange.AbsenceTypeId = selectedAbsenceType.AbsenceTypeId;
            absenceRange = AbsenceRangeUpdatePeriodAndCountry(personnelAbsenceEntitlement, absenceRange);
            var absenceDays = AbsenceRangeToAbsenceDays(absenceRange, employment.WorkingPattern, true);
            if (absenceRange.AbsenceId.HasValue)
            {
                var absence = _hrDataService.RetrieveAbsence(organisationId, absenceRange.AbsenceId.Value);
                personnelAbsenceEntitlement = RemoveAbsenceFromPersonnelAbsenceEntitlementValues(absence, personnelAbsenceEntitlement);
            }
            return new AbsenceRequest
            {
                AbsenceType = selectedAbsenceType,
                AbsenceDays = absenceDays,
                PersonnelAbsenceEntitlement = personnelAbsenceEntitlement,
                Duration = CalculateDuration(absenceDays),
                AbsenceTypes = absenceTypes,

            };
        }

        public PersonnelAbsenceEntitlement RetrievePersonnelAbsenceEntitlement(int organisationId, AbsenceRange absenceRange, int employmentId)
        {
            var personnelAbsenceEntitlement = _hrDataService.RetrievePersonnelAbsenceEntitlements(organisationId, absenceRange.PersonnelId, employmentId)
                .FirstOrDefault(
                p =>
                     p.AbsenceTypeId == absenceRange.AbsenceTypeId &&
                     p.StartDate <= absenceRange.BeginDateUtc &&
                     p.EndDate >= absenceRange.BeginDateUtc);

            if (personnelAbsenceEntitlement != null)
                return personnelAbsenceEntitlement;

            return _hrDataService.RetrievePersonnelAbsenceEntitlements(organisationId, absenceRange.PersonnelId, employmentId).FirstOrDefault(p =>
                     !p.AbsenceTypeId.HasValue || (
                     p.StartDate <= absenceRange.BeginDateUtc &&
                     p.EndDate >= absenceRange.BeginDateUtc));

        }

        public IEnumerable<INotAbsenceDay> RetrieveCannotBeBookedDays(AbsenceRange absenceRange, WorkingPattern workingPattern)
        {
            // Working Pattern Not Absence Days
            var workingPatternNotAbsenceDays = workingPattern?.WorkingPatternDays?.ToWorkingPatternNotAbsenceDays(absenceRange.BeginDateUtc, absenceRange.EndDateUtc)?.ToList();

            var publicHolidays = _hrDataService.RetrievePublicHolidays(absenceRange.OrganisationId, absenceRange.PublicHolidayPolicyId, e => e.Date >= absenceRange.BeginDateUtc && e.Date <= absenceRange.EndDateUtc).Items.ToList();
            var alreadyBookedAbsencesExcludingCurrentAbsence = RetrieveAlreadyBookedAbsencesExcludingCurrentAbsence(absenceRange).ToList();
            var notAbsenceDays = new List<INotAbsenceDay>();

            notAbsenceDays.AddRange(workingPatternNotAbsenceDays);
            notAbsenceDays.AddRange(publicHolidays);
            notAbsenceDays.AddRange(alreadyBookedAbsencesExcludingCurrentAbsence);

            return
                 notAbsenceDays.GroupBy(d => d.Date)
                     .Select(e => new NotAbsenceDay() { Date = e.Key.Date, NotAbsenceDays = e.ToList() });

        }

        public IEnumerable<CanBeBookedAbsenceDay> RetrieveAlreadyBookedAbsencesExcludingCurrentAbsence(AbsenceRange absenceRange)
        {
            // Already Booked Not Absence Days exclude current absence
            var bookedAbsences = _hrDataService.RetrieveAbsenceRangeBookedAbsenceDays(absenceRange)
                                .Where(e => e.AbsenceId != (absenceRange.AbsenceId ?? -1))
                                .Select(e => new CanBeBookedAbsenceDay
                                {
                                    Date = e.Date,
                                    AM = !e.AM,
                                    PM = !e.PM,
                                }).ToList();

            return bookedAbsences;
        }

        public PagedResult<Absence> RetrieveAbsences(int organisationId, ApprovalFilter approvalFilter, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveAbsenceTransactions(organisationId, approvalFilter, orderBy, paging);
        }

        public PagedResult<AbsenceForApproval> RetrieveAbsenceForApprovals(int organisationId, string userId, bool isAdmin, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveAbsenceForApprovals(organisationId, userId, isAdmin, p => true, orderBy, paging);
        }

        public IEnumerable<AbsenceRange> RetrieveAbsences(int organisationId, int personnelId, int absencePeriodId, bool isAdmin)
        {

            var absences = _hrDataService
                .RetrieveAbsences(organisationId, personnelId, absencePeriodId)
                .Select(a =>
                {
                    var absenceRange = AbsenceToAbsenceRange(a);
                    absenceRange.Permissions = RetrievePersonnelPermissions(isAdmin, organisationId, personnelId, a.PersonnelAbsenceEntitlement.PersonnelId);
                    return absenceRange;
                })
                .OrderByDescending(a => a.BeginDateUtc)
                .ToList();

            return absences;
        }

        public Schedule RetrieveAbsenceSchedule(int organisationId, DateTime beginDate, DateTime endDate, Permissions permission, int personnelId, PersonnelFilter personnelFilter, int showColourBy, string userId)
        {
            var absences = _hrDataService.RetrieveAbsences(organisationId, beginDate, endDate, personnelFilter);
            var publicHoliday = _hrDataService.RetrievePersonnelPublicHolidayInDateRange(organisationId, beginDate, endDate);

            var types = absences
                .Select(absence => new ScheduleItemType
                {
                    Name = absence.AbsenceType.Name,
                    Colour = absence.AbsenceType?.Colour?.Hex ?? "6699CC"
                })
                .Distinct(new ScheduleItemTypeComparer())
                .OrderBy(by => by.Name)
                .ToList();

            var personnelDetails = RetrievePersonnelDetails(organisationId, personnelId, permission, personnelFilter, showColourBy, true);
            //Call a method GetAbsenceSlots
            var scheduleItems = personnelDetails.Select(p => new ScheduleItem
            {
                Name = string.Format("{0} {1}", p.Forename, p.Surname),
                PersonnelId = Convert.ToInt32(p.Id),
                Permissions = RetrievePersonnelPermissions(permission.IsAdmin, organisationId, personnelId, Convert.ToInt32(p.Id)),
                Colours = p.Colours,
                Slots = RetrieveAbsenceSlots(absences
                    .Where(a => a.PersonnelAbsenceEntitlement.PersonnelId == Convert.ToInt32(p.Id))
                    , beginDate, endDate).Where(s => s.SlotBeginDate < endDate.Date.AddDays(1) && s.SlotBeginDate >= beginDate.Date)
                    .Concat<Slot>(
                        publicHoliday?
                        .Where(ph => ph.PersonnelId == Convert.ToInt32(p.Id))
                        .Select(ph => new PublicHolidaySlot
                        {
                            Details = ph.Name,
                            BeginDate = ph.Date
                        }))
                    .ToList()
            }).ToList();

            return new Schedule
            {
                BeginDate = beginDate,
                Items = scheduleItems,
                Types = types
            };
        }

        public List<Slot> RetrieveAbsenceSlots(IEnumerable<Absence> absences, DateTime beginDate, DateTime endDate)
        {
            var slots = new List<Slot>();
            foreach (var absence in absences)
            {
                slots.AddRange(RetrieveAbsenceSlots(absence, beginDate, endDate));
            }
            return slots;
        }

        public IEnumerable<AbsenceSlot> RetrieveAbsenceSlots(Absence absence, DateTime beginDate, DateTime endDate)
        {
            var absenceSlots = new List<AbsenceSlot>();
            //var employmentId = absence.PersonnelAbsenceEntitlement.EmploymentPersonnelAbsenceEntitlements.FirstOrDefault()?.EmploymentId ?? 0;
            //var workingPatternId = _hrDataService.Retrieve<Employment>(absence.OrganisationId, a => a.EmploymentId == employmentId
            //    ).FirstOrDefault()?.WorkingPatternId ?? 0;
            //var workingPatternDays = _hrDataService.Retrieve<WorkingPatternDay>(absence.OrganisationId, a => a.WorkingPatternId == workingPatternId);

            //var firstAbsenceDay = absence.AbsenceDays.Where(a => a.Date >= beginDate && a.Date <= endDate).OrderBy(a => a.Date).FirstOrDefault();
            //var lastAbsenceDay = absence.AbsenceDays.Where(a => a.Date >= beginDate && a.Date <= endDate).OrderBy(a => a.Date).LastOrDefault();

            //IEnumerable<AbsenceDay> absenceDays = absence.AbsenceDays;
            //bool dayBeforeAbsenceIsAmOnly = false;
            //bool dayAfterAbsenceIsPmOnly = false;

            //if (workingPatternDays.Any(a => firstAbsenceDay.PM = true || (a.DayOfWeek == firstAbsenceDay.DayOfWeek && a.PM == false))
            //    && workingPatternDays.Any(a => lastAbsenceDay.AM = true || (a.DayOfWeek == lastAbsenceDay.DayOfWeek && a.AM == false)))
            //{
            //    List<DateTime> dateGap = new List<DateTime>();

            //    for (int counter = 1; absence.AbsenceDays.Min(a => a.Date).Date.AddDays(counter) < absence.AbsenceDays.Max(a => a.Date).Date; counter++)
            //    {
            //        dateGap.Add(absence.AbsenceDays.Min(a => a.Date).Date.AddDays(counter));
            //    }

            //    absenceDays = absence.AbsenceDays.Concat(
            //            dateGap.Select(a => new AbsenceDay
            //            {
            //                Date = a.Date,
            //                AbsenceId = 0,
            //                AM = true,
            //                CanBeBookedAsAbsence = false,
            //                Duration = 1,
            //                OrganisationId = absence.OrganisationId,
            //                PM = true
            //            }).Where(a => !absence.AbsenceDays.Any(b => b.Date == a.Date))
            //        );

            //    int countAbsenceDay = absenceDays.Count(a => a.Date >= beginDate && a.Date <= endDate);
            //    dayBeforeAbsenceIsAmOnly = workingPatternDays.Any(a => a.DayOfWeek == firstAbsenceDay.DayOfWeek && a.PM == false) && countAbsenceDay > 1;
            //    dayAfterAbsenceIsPmOnly = workingPatternDays.Any(a => a.DayOfWeek == lastAbsenceDay.DayOfWeek && a.AM == false) && countAbsenceDay > 1;
            //}



            //var absenceDayGroups = absenceDays.GroupWhile((prev, next) => true).ToList();

            var absenceDayGroups = absence.AbsenceDays.GroupWhile((prev, next) => prev.Date == next.Date.AddDays(-1) && prev.PM && next.AM).ToList();

            foreach (var absenceDayGroup in absenceDayGroups)
            {
                absenceSlots.Add(new AbsenceSlot
                {
                    //DayBeforeAbsenceIsAmOnly = dayBeforeAbsenceIsAmOnly,
                    //DayAfterAbsenceIsPmOnly = dayAfterAbsenceIsPmOnly,
                    AbsenceId = absence.AbsenceId,
                    BeginDate = beginDate,
                    EndDate = endDate,
                    SlotBeginDate = absenceDayGroup.FirstOrDefault().Date,
                    AbsenceType = absence.AbsenceType,
                    ApprovalState = absence.ApprovalState,
                    AbsenceDays = absenceDayGroup,
                    Colour = absence.AbsenceType?.Colour?.Hex ?? "6699CC",
                    CanApprove = null
                });
            }
            return absenceSlots.ToList();
        }

        public AbsencePeriod RetrieveAbsencePeriod(int organisationId, int absencePeriodId)
        {
            return _hrDataService.RetrieveAbsencePeriod(organisationId, absencePeriodId);
        }

        public PagedResult<AbsencePeriod> RetrieveAbsencePeriods(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveAbsencePeriods(organisationId, p => true, orderBy, paging);
        }

        public AbsenceType RetrieveAbsenceType(int organisationId, int id)
        {
            return _hrDataService.RetrieveAbsenceType(organisationId, id, p => true);
        }

        public PagedResult<AbsenceType> RetrieveAbsenceTypes(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveAbsenceTypes(organisationId, p => true, orderBy, paging);
        }

        public IEnumerable<ApprovalLevel> RetrieveApprovalLevels(int organisationId, int approvalModelId)
        {
            return _hrDataService.Retrieve<ApprovalLevel>(organisationId, al => al.ApprovalModelId == approvalModelId);
        }

        public IEnumerable<ApprovalEntityTypeAssignment> ApprovalEntityTypeAssignments(int organisationId, int personnelId)
        {
            var approvalEntityTypeAssignments = (
                from ae in _hrDataService.Retrieve<ApprovalEntityType>(organisationId, ae => true)
                join pam in _hrDataService.Retrieve<PersonnelApprovalModel>(organisationId, pam => pam.PersonnelId == personnelId)
                    on new { ApprovalEntityTypeId = ae.ApprovalEntityTypeId } equals new { pam.ApprovalEntityTypeId }
                    into joinPersonnelApprovalModel
                from personnelApprovalModel in joinPersonnelApprovalModel.DefaultIfEmpty()
                select new ApprovalEntityTypeAssignment
                {
                    ApprovalEntityId = ae.ApprovalEntityTypeId,
                    ApprovalModelId = personnelApprovalModel?.ApprovalModelId ?? 0,
                    PersonnelApprovalModelId = personnelApprovalModel?.PersonnelApprovalModelId ?? 0,
                    Name = ae.Name
                });

            return approvalEntityTypeAssignments;
        }

        public ApprovalModel RetrieveApprovalModel(int organisationId, int approvalModelId)
        {
            return _hrDataService.RetrieveApprovalModel(organisationId, approvalModelId);
        }

        public IEnumerable<ApprovalModel> RetrieveApprovalModels(int organisationId)
        {
            return _hrDataService.Retrieve<ApprovalModel>(organisationId, p => true);
        }

        public PagedResult<ApprovalModel> RetrieveApprovalModels(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrievePagedResult<ApprovalModel>(organisationId, p => true, orderBy, paging);
        }

        public IEnumerable<ApprovalUser> RetrieveAvailableApprovalUsers(int organisationId, int approvalLevelId)
        {
            var approvalLevel = _hrDataService.Retrieve<ApprovalLevelUser>(organisationId, alu => alu.ApprovalLevelId == approvalLevelId);
            var departments = _hrDataService.RetrieveDepartments(organisationId, new List<int>());
            var approvalUsers = (
                    from anu in _hrDataService.Retrieve<AspNetUsers>(organisationId, anu => true)
                    join p in _hrDataService.Retrieve<Personnel>(organisationId, p => true)
                        on new { PersonnelId = anu?.PersonnelId ?? 0 } equals new { p.PersonnelId }
                        into joinPersonnel
                    from personnel in joinPersonnel.DefaultIfEmpty()
                    join e in _hrDataService.Retrieve<Employment>(organisationId, e => true)
                        on new { PersonnelId = anu?.PersonnelId ?? 0 } equals new { e.PersonnelId }
                        into joinEmployment
                    from employment in joinEmployment.DefaultIfEmpty()
                    where (
                    !approvalLevel.Where(al => al.AspNetUserId == anu.Id).Any()
                    )
                    orderby personnel?.Surname ?? string.Empty, anu.Email
                    select new ApprovalUser
                    {
                        AspNetUserId = anu.Id,
                        Fullname = personnel?.Fullname ?? anu.Email,
                        Surname = personnel?.Surname ?? anu.Email,
                        Forenames = personnel?.Forenames ?? string.Empty,
                        Departments = departments.Where(d => d.EmploymentDepartments.Select(ed => ed.EmploymentId).Contains(employment == null ? 0 : employment.EmploymentId)).ToList()
                    }
                ).ToList();

            return approvalUsers;
        }

        public IEnumerable<ApprovalUser> RetrieveApprovalUsers(int organisationId, int approvalLevelId)
        {
            var approvalUser = (
                    from alu in _hrDataService.Retrieve<ApprovalLevelUser>(organisationId, alu => alu.ApprovalLevelId == approvalLevelId)
                    from anu in _hrDataService.Retrieve<AspNetUsers>(organisationId, anu => true)
                    where alu.AspNetUserId == anu.Id
                    join p in _hrDataService.Retrieve<Personnel>(organisationId, p => true)
                        on new { PersonnelId = anu?.PersonnelId ?? 0 } equals new { p.PersonnelId }
                        into d
                    from desc in d.DefaultIfEmpty()
                    select new ApprovalUser
                    {
                        ApprovalLevelUserId = alu.ApprovalLevelUserId,
                        AspNetUserId = anu.Id,
                        Fullname = desc?.Fullname ?? anu.Email,
                        Title = desc?.Title ?? string.Empty
                    }
                );

            return approvalUser;
        }

        public Building RetrieveBuilding(int organisationId, int id)
        {
            return _hrDataService.RetrieveBuilding(organisationId, id, p => true);
        }

        public IEnumerable<Building> RetrieveBuildings(int organisationId)
        {
            return _hrDataService.RetrieveBuildings(organisationId, p => true).Items;
        }

        public PagedResult<Building> RetrieveBuildings(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveBuildings(organisationId, p => true, orderBy, paging);
        }

        public IEnumerable<Colour> RetrieveColours()
        {
            return _hrDataService.RetrieveColours(p => true);
        }

        public Company RetrieveCompany(int organisationId, int id)
        {
            return _hrDataService.RetrieveCompany(organisationId, id, p => true);
        }

        public PagedResult<Company> RetrieveCompanies(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveCompanies(organisationId, p => true, orderBy, paging);
        }

        private List<PersonnelDetail> RetrievePersonnelDetails(int organisationId, int personnelId, Permissions permission, PersonnelFilter personnelFilter, int showColourBy, bool hasAccessRestriction)
        {
            if (personnelFilter.CompanyIds == null)
                personnelFilter.CompanyIds = new List<int>();

            if (personnelFilter.DepartmentIds == null)
                personnelFilter.DepartmentIds = new List<int>();

            if (personnelFilter.TeamIds == null)
                personnelFilter.TeamIds = new List<int>();

            var employmentNode = RetrievePersonnelEmploymentNode(organisationId, personnelId);
            var descendants = new List<Node<Employment>>();
            Node<Employment> personnelDetail = null;
            var personnelDepartmentIds = new List<int>();
            var teammateDepartmentIds = new List<int>();

            if (!permission.IsAdmin)
            {
                descendants = employmentNode?.SelfAndDescendants?.ToList();
                personnelDetail = descendants.FirstOrDefault(x => x.Value.PersonnelId == personnelId);
                personnelDepartmentIds = _hrDataService.Retrieve<EmploymentDepartment>(organisationId, a => a.EmploymentId == personnelDetail.Value.EmploymentId && (!personnelFilter.TeamIds.Any() || personnelFilter.TeamIds.Contains(a.DepartmentId))).Select(a => a.DepartmentId).ToList();
                teammateDepartmentIds = _hrDataService.Retrieve<EmploymentDepartment>(organisationId, a => personnelDepartmentIds.Contains(a.DepartmentId)).Select(a => a.EmploymentId).ToList();
            }

            var companies = new List<Company>();
            var departments = new List<Department>();
            var teams = new List<Team>();

            if (showColourBy == (int)ShowColour.Company)
                companies = _hrDataService.RetrieveCompanies(organisationId, personnelFilter.CompanyIds).ToList();

            if (showColourBy == (int)ShowColour.Department)
                departments = _hrDataService.RetrieveDepartments(organisationId, personnelFilter.DepartmentIds).ToList();

            if (showColourBy == (int)ShowColour.Team)
                teams = _hrDataService.RetrieveTeams(organisationId, personnelFilter.TeamIds).ToList();

            var currentEmployment = _hrDataService.RetrieveEmployment(organisationId, personnelId, _defaultDate);

            var personnelDetails = (
                    from p in _hrDataService.RetrievePersonnel(organisationId, personnelFilter.CompanyIds, personnelFilter.DepartmentIds, personnelFilter.TeamIds)
                    join d in descendants
                        on new { PersonnelId = p.PersonnelId } equals new { d.Value.PersonnelId }
                        into d
                    from desc in d.DefaultIfEmpty()
                    where (
                        //for flow chart
                        (!hasAccessRestriction
                        //admin restriction
                        || permission.IsAdmin
                        //manager restriction
                        || desc != null
                        //allowed to everyone
                        || p.PersonnelId == personnelDetail.Value.PersonnelId || p.PersonnelId == personnelDetail.Value.ReportsToPersonnelId || teammateDepartmentIds.Contains(p.Employments.LastOrDefault(e => !e.EndDate.HasValue)?.EmploymentId ?? 0))
                        //check if employee is still employed
                        && p.Employments.Any(e => (e.TerminationDate == null) ? true : e.TerminationDate > DateTime.Today)
                        )
                    select new PersonnelDetail
                    {
                        Id = p.PersonnelId.ToString(),
                        ParentId = p.Employments.LastOrDefault(e => !e.EndDate.HasValue)?.ReportsToPersonnelId.ToString() ?? string.Empty,
                        JobTitleId = p.Employments.LastOrDefault(e => !e.EndDate.HasValue)?.JobTitleId ?? 0,
                        Forename = p.Forenames,
                        Surname = p.Surname,
                        Photo = "../Personnel/Photo/" + p.PersonnelId,
                        showLink = permission.IsAdmin || desc != null,
                        Colours = (showColourBy == (int)ShowColour.Company) ?
                            companies.Where(a => a.CompanyId == (p.Employments.LastOrDefault(e => !e.EndDate.HasValue)?.CompanyId ?? 0)).Select(a => a.Colour).ToList()
                            : ((showColourBy == (int)ShowColour.Department) ? departments.Where(a =>
                                    (a.EmploymentDepartments.FirstOrDefault()?.EmploymentId ?? 0)
                                        == (p.Employments.LastOrDefault(e => !e.EndDate.HasValue)?.EmploymentId ?? 0))
                                    .Select(a => a.Colour).ToList()
                            : (showColourBy == (int)ShowColour.Team) ? teams.Where(a =>
                                    (a.EmploymentTeams.FirstOrDefault()?.EmploymentId ?? 0)
                                        == (p.Employments.LastOrDefault(e => !e.EndDate.HasValue)?.EmploymentId ?? 0))
                                    .Select(a => a.Colour).ToList() : new List<Colour>())
                    }
                );
            return personnelDetails.OrderBy(pd => pd.Forename).ToList();
        }

        public PersonnelDetailFilter RetrievePersonnelDetailFilters(int organisationId, int personnelId, bool isAdmin)
        {
            var employmentNode = RetrievePersonnelEmploymentNode(organisationId, personnelId);
            IEnumerable<Node<Employment>> descendants = null;

            if (!isAdmin)
                descendants = employmentNode?.SelfAndDescendants?.ToList();

            var colours = _hrDataService.RetrieveColours(p => true);

            var companyFilters = (
                    from c in (!isAdmin)
                        ? descendants?.Where(e => e.Value.Company != null).Select(e => e.Value.Company).Distinct(new CompanyComparer())
                        : _hrDataService.RetrieveCompanies(organisationId, p => true).Items
                    join colour in colours on c.ColourId equals colour.ColourId
                    select new CompanyFilter
                    {
                        CompanyId = c.CompanyId,
                        Name = c.Name,
                        Hex = colour.Hex
                    }
                ).ToList();

            var departmentFilter = (
                    from d in (!isAdmin)
                        ? _hrDataService.RetrieveDepartments(organisationId, p => true).Items.Where(d =>
                            _hrDataService.Retrieve<EmploymentDepartment>(organisationId, p => true).Where(ed =>
                                    descendants.Select(ds => ds.Value.EmploymentId).Contains(ed.EmploymentId)).Select(ds => ds.DepartmentId)
                                    .Contains(d.DepartmentId))
                       : _hrDataService.RetrieveDepartments(organisationId, p => true).Items
                    join colour in colours on d.ColourId equals colour.ColourId
                    select new DepartmentFilter
                    {
                        DepartmentId = d.DepartmentId,
                        Name = d.Name,
                        Hex = colour.Hex
                    }
                ).ToList();

            var teamFilter = (
                    from d in (!isAdmin)
                        ? _hrDataService.RetrieveTeams(organisationId, p => true).Items.Where(d =>
                            _hrDataService.Retrieve<EmploymentTeam>(organisationId, p => true).Where(ed =>
                                    descendants.Select(ds => ds.Value.EmploymentId).Contains(ed.EmploymentId)).Select(ds => ds.TeamId)
                                    .Contains(d.TeamId))
                       : _hrDataService.RetrieveTeams(organisationId, p => true).Items
                    join colour in colours on d.ColourId equals colour.ColourId
                    select new TeamFilter
                    {
                        TeamId = d.TeamId,
                        Name = d.Name,
                        Hex = colour.Hex
                    }
                ).ToList();

            var personnelDetailFilter = new PersonnelDetailFilter
            {
                Company = companyFilters,
                Department = departmentFilter,
                Team = teamFilter
            };

            return personnelDetailFilter;
        }

        public Country RetrieveCountry(int organisationId, int countryId)
        {
            return _hrDataService.RetrieveCountry(organisationId, countryId, p => true);
        }

        public PagedResult<Country> RetrieveCountries(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveCountries(organisationId, p => true, orderBy, paging);
        }

        public IEnumerable<WorkingPatternDay> RetrieveDefaultWorkingPatternDays()
        {
            var days = Enumerable.Range(1, 6).Select(wd => new WorkingPatternDay
            {
                DayOfWeek = (short)wd,
                AM = false,
                PM = false
            }).ToList();

            // Add sunday as 0 at the end of the list
            days.Add(new WorkingPatternDay
            {
                DayOfWeek = 0,
                AM = false,
                PM = false
            });

            return days;

        }

        public Department RetrieveDepartment(int organisationId, int id)
        {
            return _hrDataService.RetrieveDepartment(organisationId, id, p => true);
        }

        public IEnumerable<Department> RetrieveDepartments(int organisationId)
        {
            return _hrDataService.RetrieveDepartments(organisationId, p => true).Items;
        }

        public PagedResult<Department> RetrieveDepartments(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveDepartments(organisationId, p => true, orderBy, paging);
        }

        public IEnumerable<DepartmentFilter> RetrieveDepartmentFilters(int organisationId)
        {
            var departmentFilter = _hrDataService.RetrieveDepartments(organisationId, p => true).Items.Select(t =>
                   new DepartmentFilter
                   {
                       DepartmentId = t.DepartmentId,
                       Name = t.Name,
                       Hex = t.Hex
                   }
                ).ToList();
            return departmentFilter;
        }

        //private Document RetrieveDocumentPhoto(int organisationId, int personnelId)

        //{
        //    var organisation = RetrieveOrganisation(organisationId);
        //    var documentPhoto = _documentServiceAPI.RetrieveDocuments(organisation.Name, PersonnelProfileCategory,
        //                        new DocumentAttribute
        //                        {
        //                            Key = PersonnelPhotoKey,
        //                            Value = personnelId.ToString()
        //                        });
        //    if (documentPhoto == null)
        //        return null;

        //    return documentPhoto.FirstOrDefault();
        //}

        public string RetrievePhoto(int organisationId, int personnelId)
        {
            string bytes = string.Empty;
            string cacheKey = PersonnelPhotoKey + personnelId;
            if (_cacheProvider.IsSet(cacheKey))
            {
                bytes = (string)_cacheProvider.Get(cacheKey);
            }
            else
            {
                //var document = RetrieveDocumentPhoto(organisationId, personnelId);
                //if (document == null)
                //{
                //    _cacheProvider.Set(cacheKey, string.Empty, ConfigHelper.CacheTimeout);
                //    return string.Empty;
                //}
                //var documentBytes = _documentServiceAPI.Download(document.DocumentGuid);
                //bytes = documentBytes.Bytes;
                //_cacheProvider.Set(cacheKey, bytes, ConfigHelper.CacheTimeout);
            }

            return bytes;
        }

        public EmergencyContact RetrieveEmergencyContact(int organisationId, int id)
        {
            return _hrDataService.RetrieveEmergencyContact(organisationId, id, p => true);
        }

        public IEnumerable<Employment> RetrievePersonnelEmployments(int organisationId, int personnelId)
        {
            return _hrDataService.RetrievePersonnelEmployments(organisationId, personnelId);
        }

        public IEnumerable<EmergencyContact> RetrieveEmergencyContactsbyPersonnelId(int organisationId, int personnelId)
        {
            return _hrDataService.RetrievePersonnel(organisationId, personnelId, p => true).EmergencyContacts;
        }

        public IEnumerable<Absence> RetrieveManagerAbsencesRequiringApproval(int organisationId, List<int> personnelIds)
        {
            return _hrDataService.RetrieveManagerAbsencesRequiringApproval(organisationId, personnelIds);
        }

        public OrganisationalChart RetrieveOrganisationalChart(int organisationId, int personnelId, Permissions permission, PersonnelFilter personnelFilter, int showColourBy)
        {
            var personnelDetails = RetrievePersonnelDetails(organisationId, personnelId, permission, personnelFilter, showColourBy, false);

            var jobTitleIds = personnelDetails.Select(a => a.JobTitleId).ToList();
            var jobTitles = _hrDataService.Retrieve<JobTitle>(organisationId, a => jobTitleIds.Contains(a.JobTitleId)).ToList();

            personnelDetails = personnelDetails.Select(a =>
                new PersonnelDetail
                {
                    Forename = a.Forename,
                    Id = a.Id,
                    JobTitleId = a.JobTitleId,
                    JobTitle = jobTitles.FirstOrDefault(b => b.JobTitleId == a.JobTitleId)?.Name ?? string.Empty,
                    ParentId = a.ParentId,
                    Photo = a.Photo,
                    showLink = a.showLink,
                    Surname = a.Surname,
                    Colours = a.Colours
                }
            ).OrderBy(p => p.Forename).ToList();

            var organisationalChart = new OrganisationalChart
            {
                ChartItems = personnelDetails
            };

            return organisationalChart;
        }

        public Organisation RetrieveOrganisation(int organisationId)
        {
            EnsureOrganisationCache();
            var organisation = (List<Organisation>)_cacheProvider.Get(OrganisationCacheKey);
            return organisation.FirstOrDefault(o => o.OrganisationId == organisationId);
        }

        public Organisation RetrieveOrganisation(string name)
        {
            EnsureOrganisationCache();
            var organisation = (List<Organisation>)_cacheProvider.Get(OrganisationCacheKey);
            return organisation.FirstOrDefault(o => o.Name == name);
        }

        private void EnsureOrganisationCache()
        {
            lock (lockObject)
            {
                if (_cacheProvider.IsSet(OrganisationCacheKey))
                    return;

                var organisation = _hrDataService.RetrieveOrganisations().ToList();
                _cacheProvider.Set(OrganisationCacheKey, organisation, ConfigHelper.CacheTimeout);
            }
        }

        public IEnumerable<OvertimePreference> RetrieveOvertimePreferences(int organisationId)
        {
            return _hrDataService.Retrieve<OvertimePreference>(organisationId, p => true);
        }

        public Personnel RetrievePersonnel(int organisationId, int personnelId)
        {
            var personnel = _hrDataService.RetrievePersonnel(organisationId, personnelId, p => true);
            var employment = _hrDataService.RetrieveEmployment(organisationId, personnelId, _defaultDate);
            if (employment == null)
                return personnel;

            var currentPersonnelCurrentAbsenceEntitlements = _hrDataService.RetrievePersonnelCurrentAbsenceEntitlements(organisationId, personnelId, employment.EmploymentId).Where(a => a.AbsenceTypeId != null);
            personnel.CurrentAbsenceTypeEntitlements = currentPersonnelCurrentAbsenceEntitlements;
            return personnel;
        }

        public PagedResult<Personnel> RetrievePersonnel(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrievePersonnel(organisationId, p => true, orderBy, paging);
        }

        public IEnumerable<PersonnelApprovalModel> RetrievePersonnelApprovalModels(int organisationId, int personnelId)
        {
            return _hrDataService.Retrieve<PersonnelApprovalModel>(organisationId, pam => pam.PersonnelId == personnelId);
        }

        public PersonnelAbsenceEntitlement RetrievePersonnelAbsenceEntitlement(int organisationId, int personnelId, int personnelCurrentAbsenceEntitlementId)
        {
            return _hrDataService.RetrievePersonnelAbsenceEntitlement(organisationId, personnelId, personnelCurrentAbsenceEntitlementId);
        }

        public IEnumerable<PersonnelAbsenceEntitlement> RetrievePersonnelAbsenceEntitlements(int organisationId, int personnelId, int absencePeriodId)
        {
            return _hrDataService.RetrievePersonnelAbsenceEntitlements(organisationId, p => p.PersonnelId == personnelId && p.AbsencePolicyPeriod.AbsencePeriodId == absencePeriodId);
        }

        public IEnumerable<AbsencePeriod> RetrievePersonnelAbsencePeriods(int organisationId, int personnelId)
        {
            var absencePeriods = _hrDataService.RetrieveAbsencePeriods(organisationId, a => true, null, null).Items;
            return absencePeriods;
        }

        public PagedResult<PersonnelSearchField> RetrievePersonnelBySearchKeyword(int organisationId, string searchKeyword, List<OrderBy> orderBy = null, Paging paging = null)
        {
            return _hrDataService.RetrievePersonnelBySearchKeyword(organisationId, searchKeyword, orderBy, paging);
        }

        public Employment RetrievePersonnelCurrentEmployment(int organisationId, int personnelId)
        {
            return _hrDataService.RetrievePersonnelCurrentEmployment(organisationId, personnelId);
        }

        public IEnumerable<PersonnelAbsenceEntitlement> RetrievePersonnelCurrentAbsenceEntitlements(int organisationId, int personnelId, int employmentId)
        {
            return _hrDataService.RetrievePersonnelCurrentAbsenceEntitlements(organisationId, personnelId, employmentId);

        }

        public IEnumerable<Personnel> RetrievePersonnelChildrenPersonnel(int organisationId, int personnelId)
        {
            var employmentNode = RetrievePersonnelEmploymentNode(organisationId, personnelId);

            var descendants = employmentNode?.Children?.ToList();

            return descendants?.Select(e => e.Value.Personnel).ToList();
        }

        public IEnumerable<Company> RetrievePersonnelDescendantCompanies(int organisationId, int personnelId)
        {
            var employmentNode = RetrievePersonnelEmploymentNode(organisationId, personnelId);
            var descendants = employmentNode?.SelfAndDescendants?.ToList();
            return null;
        }

        public Employment RetrievePersonnelEmployment(int organisationId, int personnelId, int employmentId)
        {
            return _hrDataService.RetrievePersonnelEmployment(organisationId, personnelId, employmentId);
        }

        public IEnumerable<EmploymentDepartment> RetrieveEmploymentDepartments(int organisationId, int employmentId)
        {
            return _hrDataService.RetrieveEmploymentDepartments(organisationId, employmentId);
        }

        public IEnumerable<EmploymentTeam> RetrieveEmploymentTeams(int organisationId, int employmentId)
        {
            return _hrDataService.RetrieveEmploymentTeams(organisationId, employmentId);
        }

        public EmploymentType RetrieveEmploymentType(int organisationId, int employmentTypeId)
        {
            return _hrDataService.Retrieve<EmploymentType>(organisationId, employmentTypeId);
        }

        public IEnumerable<EmploymentType> RetrieveEmploymentTypes(int organisationId)
        {
            return _hrDataService.Retrieve<EmploymentType>(organisationId, p => true);
        }

        public PagedResult<EmploymentType> RetrieveEmploymentType(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrievePagedResult<EmploymentType>(organisationId, p => true, orderBy, paging);
        }

        public Overtime RetrieveOvertime(int organisationId, int overtimeId)
        {
            return _hrDataService.RetrieveOvertime(organisationId, overtimeId);
        }

        public PagedResult<Overtime> RetrieveOvertimes(int organisationId, int personnelId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveOvertimes(organisationId, p => p.PersonnelId == personnelId, orderBy, paging);
        }

        public PagedResult<Overtime> RetrieveOvertimes(int organisationId, OvertimeFilter overtimeFilter, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveOvertimeTransactions(organisationId, overtimeFilter, orderBy, paging);
        }

        public OvertimeForApproval RetrieveOvertimeForApprovals(int organisationId, int overtimeId, string userId, bool isAdmin)
        {
            return _hrDataService.RetrieveOvertimeForApprovals(organisationId, userId, isAdmin, ofa => ofa.OvertimeId == overtimeId, null, null).Items.FirstOrDefault();
        }

        public PagedResult<OvertimeForApproval> RetrieveOvertimeForApprovals(int organisationId, string userId, bool isAdmin, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveOvertimeForApprovals(organisationId, userId, isAdmin, p => true, orderBy, paging);
        }

        public OvertimeSummary RetrieveOvertimeSummary(int organisationId, int personnelId)
        {
            return _hrDataService.RetrieveOvertimeSummaries(organisationId, os => os.PersonnelId == personnelId).Items.FirstOrDefault();
        }

        public PagedResult<OvertimeSummary> RetrieveOvertimeSummaries(int organisationId, IEnumerable<int> companyIds, IEnumerable<int> departmentIds, IEnumerable<int> teamIds, List<OrderBy> orderBy, Paging paging)
        {

            if (companyIds == null)
                companyIds = new List<int>();

            if (departmentIds == null)
                departmentIds = new List<int>();

            if (teamIds == null)
                teamIds = new List<int>();

            return _hrDataService.RetrieveOvertimeSummaries(organisationId, (os =>
                (!companyIds.Any() || companyIds.Contains(os.CompanyId)) &&
                (!departmentIds.Any() || os.Departments.Any(d => d.EmploymentDepartments.Any(a => departmentIds.Contains(a.DepartmentId)))) &&
                (!teamIds.Any() || os.Teams.Any(d => d.EmploymentTeams.Any(a => teamIds.Contains(a.TeamId))))),
                orderBy, paging);
        }

        public Permissions RetrievePersonnelPermissions(bool isAdmin, int organisationId, int userPersonnelId, int? personnelId = null)
        {
            var isManagerOf = IsManagerOfPersonnel(organisationId, userPersonnelId, personnelId);
            var isPerson = userPersonnelId == personnelId;
            var personnelNode = personnelId.HasValue ? RetrievePersonnelEmploymentNode(organisationId, personnelId.Value) : null;
            var personnelIsTerminated = personnelNode?.Value.TerminationDate.HasValue ?? false;

            return new Permissions
            {
                IsAdmin = isAdmin,
                IsManager = isManagerOf,
                CanViewProfile = isAdmin || isManagerOf || isPerson,
                CanEditProfile = isAdmin || (!personnelIsTerminated && isPerson),
                CanCreateAbsence = isAdmin || (!personnelIsTerminated && (isManagerOf || isPerson)),
                CanEditAbsence = isAdmin || isManagerOf || (!personnelIsTerminated && isPerson),
                CanCancelAbsence = isAdmin || isManagerOf || (!personnelIsTerminated && isPerson),
                CanEditEntitlements = isAdmin,
                CanEditEmployments = isAdmin,
                CanCreateOvertime = isAdmin || (!personnelIsTerminated && (isManagerOf || isPerson)),
                CanEditOvertime = isAdmin || (!personnelIsTerminated && (isManagerOf || isPerson)),
                CanDeleteOvertime = isAdmin || (!personnelIsTerminated && isPerson),
                CanEditApprover = isAdmin
            };
        }

        public WorkingPattern RetrievePersonnelWorkingPattern(int organisationId, int personnelId)
        {
            return _hrDataService.RetrievePersonnelWorkingPattern(organisationId, personnelId);
        }

        public IEnumerable<Personnel> RetrieveReportsToPersonnel(int organisationId, int personnelId)
        {
            return _hrDataService.RetrievePersonnel(organisationId, p => p.PersonnelId != personnelId).Items;
        }

        public Team RetrieveTeam(int organisationId, int id)
        {
            return _hrDataService.RetrieveTeam(organisationId, id, p => true);
        }

        public PagedResult<Team> RetrieveTeam(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveTeams(organisationId, p => true, orderBy, paging);
        }

        public IEnumerable<TeamFilter> RetrieveTeamFilters(int organisationId)
        {
            var teamFilter = _hrDataService.RetrieveTeams(organisationId, p => true).Items.Select(t =>
                   new TeamFilter
                   {
                       TeamId = t.TeamId,
                       Name = t.Name,
                       Hex = t.Hex
                   }
                ).ToList();
            return teamFilter;
        }

        public Site RetrieveSite(int organisationId, int id)
        {
            return _hrDataService.RetrieveSite(organisationId, id, p => true);
        }

        public PagedResult<Site> RetrieveSites(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveSites(organisationId, p => true, orderBy, paging);
        }

        public IEnumerable<Site> RetrieveSites(int organisationId)
        {
            return _hrDataService.RetrieveSites(organisationId, p => true).Items;
        }

        public IEnumerable<TenantOrganisation> RetrieveTenantOrganisations()
        {
            var hosts = _hrDataService.RetrieveHosts();

            return hosts
                .Select(h => new TenantOrganisation
                {
                    OrganisationId = h.OrganisationId,
                    Name = h.Organisation.Name,
                    HostName = h.HostName
                })
                .ToList();
        }

        public IAuthorisation RetrieveUserAuthorisation(string aspNetUserId)
        {
            var userAuthorisation = _hrDataService.RetrieveUserAuthorisation(aspNetUserId);
            if (userAuthorisation == null)
                return null;

            return new Authorisation
            {
                OrganisationId = userAuthorisation.OrganisationId,
                RoleId = int.Parse(userAuthorisation.RoleId)
            };
        }

        public IEnumerable<CompanyBuilding> RetrieveCompanyBuilding(int organisationId, int companyId)
        {
            var result = _hrDataService.RetrieveCompanyBuilding(organisationId, p => p.CompanyId == companyId);
            return result;
        }
        public IEnumerable<CompanyBuilding> RetrieveEmploymentCompanyBuilding(int organisationId)
        {
            var result = _hrDataService.RetrieveCompanyBuilding(organisationId, p => true);
            return result;
        }

        public IEnumerable<JobTitleJobGrade> RetrieveJobTitleJobGrade(int organisationId)
        {
            var result = _hrDataService.RetrieveJobTitleJobGrade(organisationId, p => true);
            return result;
        }

        public IEnumerable<AbsenceType> RetrieveAbsenceTypes(int organisationId, int absencePolicyId)
        {
            return _hrDataService.RetrieveAbsenceTypes(organisationId, absencePolicyId);
        }

        public IEnumerable<Building> RetrieveBuildingsSitesUnassignedCompany(int organisationId, int companyId)
        {
            var unassignedBuildingSites =
                _hrDataService.RetrieveBuildings(organisationId,
                    p => p.CompanyBuildings.All(e => e.CompanyId != companyId), null,
                    null).Items.ToList();
            return unassignedBuildingSites;

        }

        public JobGrade RetrieveJobGrade(int organisationId, int id)
        {
            return _hrDataService.RetrieveJobGrade(organisationId, id, p => true);
        }

        public PagedResult<JobGrade> RetrieveJobGrade(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveJobGrades(organisationId, p => true, orderBy, paging);
        }

        public JobTitle RetrieveJobTitle(int organisationId, int id)
        {
            return _hrDataService.RetrieveJobTitle(organisationId, id, p => true);
        }

        //Todo : change this to fethch included tables
        public PagedResult<JobTitle> RetrieveJobTitle(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveJobTitles(organisationId, p => true, orderBy, paging);
        }

        public JobTitleDocument RetrieveJobTitleDocument(int organisationId, Guid documentDetailGuid)
        {

            var organisation = RetrieveOrganisation(organisationId);
            //var document =  _documentServiceAPI.RetrieveDocument(documentDetailGuid);

            //if (document == null)
            //    return null;

            //var documentBytes = _documentServiceAPI.Download(document.DocumentGuid);

            //JobTitleDocument jobTitleDocument = new JobTitleDocument
            //{
            //    DocumentBytesString = documentBytes.Bytes,
            //    DocumentFileName = document.FileName
            //};

            //return jobTitleDocument;
            return null;
        }

        public PagedResult<JobTitleDocument> RetrieveJobTitleDocuments(int organisationId, int jobTitleId, Paging paging)
        {

            var organisation = RetrieveOrganisation(organisationId);
            //var documents = _documentServiceAPI.RetrieveDocuments(organisation.Name, JobTitleDocumentCategory, jobTitleId.ToString())
            //    .Select(rd => new JobTitleDocument
            //    {
            //        Name = rd.Description,
            //        DocumentDetailId = rd.DocumentGuid
            //    })
            //    .AsQueryable().Paginate(paging);
            //return documents;
            return null;
        }

        public WorkingPattern RetrieveWorkingPattern(int organisationId, int workingPatternId)
        {
            return _hrDataService.RetrieveWorkingPattern(organisationId, workingPatternId);
        }
        #endregion

        #region // Update

        public void ApproveAbsence(int organisationId, int absenceId, string userId, bool isAdmin)
        {
            var absence = _hrDataService.RetrieveAbsence(organisationId, absenceId);
            if (absence != null)
            {
                ApproveApprovalEntity(organisationId, ApprovalTypes.Absence, absence.AbsenceId, userId, isAdmin);
                absence.ApprovalState = null;
                absence.ApprovalStateId = ApprovalStates.InApproval.GetHashCode();
                absence.AbsenceStatusByUser = userId;
                absence.AbsenceStatusDateTimeUtc = DateTime.UtcNow;
                _hrDataService.UpdateOrganisationEntityEntry(organisationId, absence);
                StartApprovalProcess(organisationId, ApprovalTypes.Absence, absence.AbsenceId, userId);
            }
        }

        public void ApproveAbsence(int organisationId, Absence absence, string userId, bool isAdmin)
        {
            ApproveApprovalEntity(organisationId, ApprovalTypes.Absence, absence.AbsenceId, userId, isAdmin);
            absence.ApprovalStateId = (int)ApprovalStates.InApproval;
            absence.AbsenceStatusByUser = userId;
            absence.AbsenceStatusDateTimeUtc = DateTime.UtcNow;
            _hrDataService.UpdateOrganisationEntityEntry(organisationId, absence);
            StartApprovalProcess(organisationId, ApprovalTypes.Absence, absence.AbsenceId, userId);
        }

        public void DeclineAbsence(int organisationId, int absenceId, string userId, bool isAdmin)
        {
            var absence = _hrDataService.RetrieveAbsence(organisationId, absenceId);
            if (absence != null)
            {
                DeclineApprovalEntity(organisationId, ApprovalTypes.Absence, absence.AbsenceId, userId, isAdmin);
                absence.ApprovalState = null;
                absence.ApprovalStateId = ApprovalStates.Declined.GetHashCode();
                absence.AbsenceStatusByUser = userId;
                absence.AbsenceStatusDateTimeUtc = DateTime.UtcNow;
                absence = _hrDataService.UpdateOrganisationEntityEntry(organisationId, absence);
                UpdateAbsencePersonnelAbsenceEntitlement(organisationId, null, absence);
                SendAbsenceStatusMessage(organisationId, absence);
            }
        }

        public void DeclineAbsence(int organisationId, Absence absence, string userId, bool isAdmin)
        {
            DeclineApprovalEntity(organisationId, ApprovalTypes.Absence, absence.AbsenceId, userId, isAdmin);
            absence.ApprovalStateId = (int)ApprovalStates.Declined;
            absence.AbsenceStatusByUser = userId;
            absence.AbsenceStatusDateTimeUtc = DateTime.UtcNow;
            _hrDataService.UpdateOrganisationEntityEntry(organisationId, absence);
            UpdateAbsencePersonnelAbsenceEntitlement(organisationId, null, absence);
            SendAbsenceStatusMessage(organisationId, absence);
        }

        private void AbsenceApproved(int organisationId, int entityId, string updatedBy)
        {
            var absence = _hrDataService.Retrieve<Absence>(organisationId, entityId);
            absence.ApprovalStateId = (int)ApprovalStates.Approved;
            absence.AbsenceStatusDateTimeUtc = DateTime.UtcNow;
            _hrDataService.UpdateOrganisationEntityEntry(organisationId, absence);
            SendAbsenceStatusMessage(organisationId, absence);
        }

        public ValidationResult UpdateAbsence(int organisationId, AbsenceRange absenceRange)
        {
            var validate = ValidateAbsence(organisationId, absenceRange);
            if (!validate.Succeeded)
                return validate;

            DeleteAbsence(organisationId, absenceRange.AbsenceId.Value);

            absenceRange.AbsenceId = null;
            CreateAbsence(organisationId, absenceRange);

            return validate;
        }

        public void UpdateAbsencePersonnelAbsenceEntitlement(int organisationId, Absence absence, Absence previousAbsence)
        {
            PersonnelAbsenceEntitlement personnelAbsenceEntitlement = null;
            if (previousAbsence != null)
                personnelAbsenceEntitlement = RemoveAbsenceFromPersonnelAbsenceEntitlementValues(previousAbsence, previousAbsence.PersonnelAbsenceEntitlement);

            if (absence != null)
            {
                absence.PersonnelAbsenceEntitlement = personnelAbsenceEntitlement ?? absence.PersonnelAbsenceEntitlement;
                personnelAbsenceEntitlement = AddAbsenceToPersonnelAbsenceEntitlementValues(absence, absence.PersonnelAbsenceEntitlement);
            }

            if (personnelAbsenceEntitlement == null)
                return;

            _hrDataService.UpdateOrganisationEntityEntry(organisationId, personnelAbsenceEntitlement);
        }

        public ValidationResult<AbsencePeriod> UpdateAbsencePeriod(int organisationId, AbsencePeriod absencePeriod)
        {
            var validationResult = AbsencePeriodAlreadyExists(organisationId, absencePeriod.AbsencePeriodId, absencePeriod.StartDate, absencePeriod.EndDate);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, absencePeriod);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.Message };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<AbsenceType> UpdateAbsenceType(int organisationId, AbsenceType absenceType)
        {
            var validationResult = AbsenceTypeAlreadyExists(organisationId, absenceType.AbsenceTypeId, absenceType.Name);
            if (!validationResult.Succeeded)
                return validationResult;

            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, absenceType);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        private void OvertimeApproved(int organisationId, int entityId, string updatedBy)
        {
            var overtime = _hrDataService.Retrieve<Overtime>(organisationId, entityId);
            overtime.ApprovalStateId = (int)ApprovalStates.Approved;
            overtime.UpdatedBy = updatedBy;
            overtime.UpdatedDateUtc = DateTime.UtcNow;
            _hrDataService.UpdateOrganisationEntityEntry(organisationId, overtime);
            SendOvertimeStatusMessage(overtime);
        }

        private void ApproveApprovalEntity(int organisationId, ApprovalTypes approvalEntityTypes, int entityId, string userId, bool isAdmin)
        {
            var approvers = _hrDataService.RetrieveNextApprovers(organisationId, approvalEntityTypes, entityId);
            if ((isAdmin && approvers.Any()) || approvers.Any(a => a.AspNetUserId == userId))
            {
                Approval approval = _hrDataService.Retrieve<Approval>(organisationId, approvers.FirstOrDefault().ApprovalId);
                approval.ApprovalStateId = (int)ApprovalStates.Approved;
                approval.UpdatedDateUtc = DateTime.UtcNow;
                _hrDataService.UpdateOrganisationEntityEntry(organisationId, approval);
            }
        }

        private void DeclineApprovalEntity(int organisationId, ApprovalTypes approvalEntityTypes, int entityId, string userId, bool isAdmin)
        {
            var approvers = _hrDataService.RetrieveNextApprovers(organisationId, approvalEntityTypes, entityId);
            if ((isAdmin && approvers.Any()) || approvers.Any(a => a.AspNetUserId == userId))
            {
                Approval approval = _hrDataService.Retrieve<Approval>(organisationId, approvers.FirstOrDefault().ApprovalId);
                approval.ApprovalStateId = (int)ApprovalStates.Declined;
                approval.UpdatedDateUtc = DateTime.UtcNow;
                _hrDataService.UpdateOrganisationEntityEntry(organisationId, approval);
            }
        }

        public ValidationResult<ApprovalModel> UpdateApprovalModel(int organisationId, ApprovalModel approvalModel)
        {
            var validationResult = ApprovalModelAlreadyExists(organisationId, approvalModel.ApprovalModelId, approvalModel.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, approvalModel);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public Building UpdateBuilding(int organisationId, Building building)
        {
            return _hrDataService.UpdateOrganisationEntityEntry(organisationId, building);
        }

        public ValidationResult<Company> UpdateCompany(int organisationId, Company company)
        {
            var validationResult = CompanyAlreadyExists(organisationId, company.CompanyId, company.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, company);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<Country> UpdateCountry(int organisationId, Country country)
        {
            var validationResult = CountryAlreadyExists(organisationId, country.CountryId, country.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, country);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<Department> UpdateDepartment(int organisationId, Department department)
        {
            var validationResult = DepartmentAlreadyExists(organisationId, department.DepartmentId, department.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, department);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public EmergencyContact UpdateEmergencyContact(int organisationId, EmergencyContact emergencyContact)
        {
            return _hrDataService.UpdateOrganisationEntityEntry(organisationId, emergencyContact);
        }

        // Call this when we are providing a custom working pattern

        //call when only update the End date of Previous Employment
        public ValidationResult<Employment> UpdateEmploymentEndDate(int organisationId, Employment employment)
        {
            var validate = ValidateEmployment(employment);
            if (!validate.Succeeded)
                return validate;

            var validationResult = new ValidationResult<Employment>();
            try
            {
                var previousEmployment = RetrievePersonnelEmployment(organisationId, employment.PersonnelId, employment.EmploymentId);
                previousEmployment.EndDate = employment.EndDate;
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, previousEmployment);
                validationResult.Succeeded = true;
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.Message };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<EmploymentType> UpdateEmploymentType(int organisationId, EmploymentType employmentType)
        {
            var validationResult = EmploymentTypeAlreadyExists(organisationId, employmentType.EmploymentTypeId, employmentType.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, employmentType);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public Overtime UpdateOvertime(int organisationId, Overtime overtime)
        {
            return _hrDataService.UpdateOrganisationEntityEntry(organisationId, overtime);
        }

        public Personnel UpdatePersonnel(int organisationId, Personnel personnel)
        {
            return _hrDataService.UpdateOrganisationEntityEntry(organisationId, personnel);
        }

        public ValidationResult<PersonnelAbsenceEntitlement> UpdatePersonnelAbsenceEntitlement(int organisationId, PersonnelAbsenceEntitlement personnelAbsenceEntitlement)
        {
            var validationResult = new ValidationResult<PersonnelAbsenceEntitlement>
            {
                Succeeded = true
            };

            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, personnelAbsenceEntitlement);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<PersonnelApprovalModel> UpdatePersonnelApprovalModel(int organisationId, PersonnelApprovalModel personnelPersonnelApprovalModel)
        {
            var validationResult = PersonnelApprovalModelAlreadyExists(organisationId, personnelPersonnelApprovalModel.PersonnelApprovalModelId, personnelPersonnelApprovalModel.PersonnelId, personnelPersonnelApprovalModel.ApprovalModelId, personnelPersonnelApprovalModel.ApprovalModelId);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, personnelPersonnelApprovalModel);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public void UpdatePersonnelCurrentEmployment(int organisationId, int employmentId, int personnelId)
        {
            var personnel = _hrDataService.RetrievePersonnel(organisationId, p => p.PersonnelId == personnelId).Items.FirstOrDefault();
            if (personnel == null) return;
            UpdatePersonnel(organisationId, personnel);
        }


        public void ApproveOvertime(int organisationId, int overtimeId, string userId, bool isAdmin)
        {
            var overtime = _hrDataService.Retrieve<Overtime>(organisationId, overtimeId);
            if (overtime != null)
            {
                ApproveApprovalEntity(organisationId, ApprovalTypes.Overtime, overtime.OvertimeId, userId, isAdmin);
                overtime.ApprovalStateId = (int)ApprovalStates.InApproval;
                _hrDataService.UpdateOrganisationEntityEntry(organisationId, overtime);
                StartApprovalProcess(organisationId, ApprovalTypes.Overtime, overtime.OvertimeId, userId);
            }
        }

        public void ApproveOvertime(int organisationId, Overtime overtime, string userId, bool isAdmin)
        {
            ApproveApprovalEntity(organisationId, ApprovalTypes.Overtime, overtime.OvertimeId, userId, isAdmin);
            overtime.ApprovalStateId = (int)ApprovalStates.InApproval;
            _hrDataService.UpdateOrganisationEntityEntry(organisationId, overtime);
            StartApprovalProcess(organisationId, ApprovalTypes.Overtime, overtime.OvertimeId, userId);
        }

        public void DeclineOvertime(int organisationId, int overtimeId, string userId, bool isAdmin)
        {
            var overtime = _hrDataService.Retrieve<Overtime>(organisationId, overtimeId);
            if (overtime != null)
            {
                DeclineApprovalEntity(organisationId, ApprovalTypes.Overtime, overtime.OvertimeId, userId, isAdmin);
                overtime.ApprovalStateId = (int)ApprovalStates.Declined;
                _hrDataService.UpdateOrganisationEntityEntry(organisationId, overtime);
                SendOvertimeStatusMessage(overtime);
            }
        }

        public void DeclineOvertime(int organisationId, Overtime overtime, string userId, bool isAdmin)
        {
            DeclineApprovalEntity(organisationId, ApprovalTypes.Overtime, overtime.OvertimeId, userId, isAdmin);
            overtime.ApprovalStateId = (int)ApprovalStates.Declined;
            _hrDataService.UpdateOrganisationEntityEntry(organisationId, overtime);
            SendOvertimeStatusMessage(overtime);
        }

        public ValidationResult<Team> UpdateTeam(int organisationId, Team team)
        {
            var validationResult = TeamAlreadyExists(organisationId, team.TeamId, team.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, team);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<JobGrade> UpdateJobGrade(int organisationId, JobGrade jobGrade)
        {
            var validationResult = JobGradeAlreadyExists(organisationId, jobGrade.JobGradeId, jobGrade.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, jobGrade);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<JobTitle> UpdateJobTitle(int organisationId, JobTitle jobTitle)
        {
            var validationResult = JobTitleAlreadyExists(organisationId, jobTitle.JobTitleId, jobTitle.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, jobTitle);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }


        public ValidationResult<Site> UpdateSite(int organisationId, Site site)
        {
            var validationResult = SiteAlreadyExists(organisationId, site.SiteId, site.CountryId, site.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, site);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public IEnumerable<WorkingPatternDay> UpdateWorkingPatternDays(int organisationId, int absencePolicyId, IEnumerable<WorkingPatternDay> workingPatternDays)
        {
            foreach (var workingPatternDay in workingPatternDays)
            {
                _hrDataService.UpdateOrganisationEntityEntry(organisationId, workingPatternDay);
            }
            return workingPatternDays;
        }

        #endregion

        #region // Delete

        public void DeleteAbsence(int organisationId, int absenceId)
        {
            var absence = _hrDataService.RetrieveAbsence(organisationId, absenceId);
            if (absence != null)
            {
                _hrDataService.DeleteAbsence(organisationId, absence);
                UpdateAbsencePersonnelAbsenceEntitlement(organisationId, null, absence);
            }
        }

        public void DeleteAbsencePeriod(int organisationId, int absencePeriodId)
        {
            _hrDataService.DeleteAbsencePeriod(organisationId, absencePeriodId);
        }

        public void DeleteAbsenceType(int organisationId, int absenceTypeId)
        {
            _hrDataService.DeleteAbsenceType(organisationId, absenceTypeId);
        }

        public void DeleteApprovalLevelUser(int organisationId, int approvalLevelUserId)
        {
            _hrDataService.Delete<ApprovalLevelUser>(organisationId, approvalLevelUserId);
        }

        public void DeleteApprovalModel(int organisationId, int approvalModelId)
        {
            var approvalLevels = _hrDataService.Retrieve<ApprovalLevel>(organisationId, al => al.ApprovalModelId == approvalModelId).Select(al => al.ApprovalLevelId).ToList();
            _hrDataService.DeleteRange<ApprovalLevelUser>(organisationId, alu => approvalLevels.Contains(alu.ApprovalLevelId));
            _hrDataService.DeleteRange<ApprovalLevel>(organisationId, al => al.ApprovalModelId == approvalModelId);
            _hrDataService.Delete<ApprovalModel>(organisationId, approvalModelId);
        }

        public void DeleteBuilding(int organisationId, int buildingId)
        {
            _hrDataService.DeleteBuilding(organisationId, buildingId);
        }

        public void DeleteCountry(int organisationId, int countryId)
        {
            _hrDataService.DeleteCountry(organisationId, countryId);
        }

        public void DeleteCompany(int organisationId, int companyId)
        {
            _hrDataService.DeleteCompany(organisationId, companyId);
        }

        public void DeleteDepartment(int organisationId, int departmentId)
        {
            _hrDataService.DeleteDepartment(organisationId, departmentId);
        }

        public void DeleteEmergencyContact(int organisationId, int emergencyContactId)
        {
            _hrDataService.DeleteEmergencyContact(organisationId, emergencyContactId);
        }

        public void DeletePersonnel(int organisationId, int personnelId)
        {
            _hrDataService.DeletePersonnel(organisationId, personnelId);
        }

        public void DeletePersonnelApprovalModel(int organisationId, int personnelApprovalModelId)
        {
            _hrDataService.Delete<PersonnelApprovalModel>(organisationId, personnelApprovalModelId);
        }

        public void DeleteEmploymentDepartment(int organisationId, int employmentId, int departmentId)
        {
            _hrDataService.Delete<EmploymentDepartment>(organisationId, p => p.EmploymentId == employmentId && p.DepartmentId == departmentId);
        }

        public void DeleteEmploymentTeam(int organisationId, int employmentId, int teamId)
        {
            _hrDataService.Delete<EmploymentTeam>(organisationId, p => p.EmploymentId == employmentId && p.TeamId == teamId);
        }

        public void DeleteEmploymentTeam(int organisationId, int employmentTeamId)
        {
            _hrDataService.DeleteEmploymentTeam(organisationId, employmentTeamId);
        }

        public void DeleteEmploymentType(int organisationId, int employmentTypeId)
        {
            _hrDataService.Delete<EmploymentType>(organisationId, employmentTypeId);
        }

        public void DeleteJobTitleDocument(int organisationId, Guid documentDetailGuid)
        {
            //DeleteDocument(documentDetailGuid);
        }

        //private void DeleteDocument(Guid documentGuid)
        //{
        //    _documentServiceAPI.DeleteDocuments(new List<Guid> { documentGuid });
        //}

        public void DeleteOvertime(int organisationId, int overtimeId)
        {
            _hrDataService.Delete<Overtime>(organisationId, overtimeId);
        }

        //public void DeletePhoto(int organisationId, int personnelId)
        //{
        //    var document = RetrieveDocumentPhoto(organisationId, personnelId);
        //    if (document != null)
        //    {
        //        DeleteDocument(document.DocumentGuid);
        //    }
        //    string cacheKey = PersonnelPhotoKey + personnelId;
        //    if (_cacheProvider.IsSet(cacheKey))
        //        _cacheProvider.Invalidate(cacheKey);
        //    //set cache to let the server know there are no existing photo for this employee
        //    _cacheProvider.Set(cacheKey, string.Empty, ConfigHelper.CacheTimeout);
        //}

        public void DeleteTeam(int organisationId, int teamId)
        {
            _hrDataService.DeleteTeam(organisationId, teamId);
        }

        public void DeleteJobGrade(int organisationId, int jobGradeId)
        {
            _hrDataService.DeleteJobGrade(organisationId, jobGradeId);
        }

        public void DeleteJobTitle(int organisationId, int jobTitleId)
        {
            _hrDataService.DeleteJobTitle(organisationId, jobTitleId);
        }

        public void DeleteSite(int organisationId, int siteId)
        {
            _hrDataService.DeleteSite(organisationId, siteId);
        }

        public void DeleteWorkingPattern(int organisationId, int workingPatternId)
        {
            _hrDataService.DeleteWorkingPattern(organisationId, workingPatternId);
        }

        public void DeletePersonnelEmployment(int organisationId, int personnelId, int employmentId)
        {
            _hrDataService.DeletePersonnelAbsenceEntitlements(organisationId, personnelId, employmentId);
            _hrDataService.DeleteEmployment(organisationId, employmentId);
        }
        public void DeleteCompanyBuilding(int organisationId, int companyBuildingId)
        {
            _hrDataService.DeleteCompanyBuilding(organisationId, companyBuildingId);
        }

        #endregion

        #region // Validate

        public ValidationResult ValidateAbsence(int organisationId, AbsenceRange absenceRange)
        {
            var absenceRequest = RetrieveAbsenceRequest(organisationId, absenceRange);
            if (absenceRequest == null)
                return new ValidationResult
                {
                    Succeeded = false,
                    Errors = new List<string> { "Absence range is null" }
                };

            var errors = new List<string>();
            var success = true;

            if (organisationId != absenceRange.OrganisationId)
            {
                success = false;
                errors.Add("Organisation does not equal personnel organisation");
            }

            if (absenceRequest.Duration <= 0)
            {
                success = false;
                errors.Add("Absence does not contain any valid days of absence");
            }

            if (absenceRequest.PersonnelAbsenceEntitlement.Entitlement > 0)
            {
                if (absenceRequest.Duration > absenceRequest.PersonnelAbsenceEntitlement?.Remaining)
                {
                    success = false;
                    errors.Add("Request is greater than remaining entitlement");
                }
            }
            return new ValidationResult
            {
                Succeeded = success,
                Errors = errors.Any() ? errors : null
            };
        }

        public ValidationResult<Employment> ValidateEmployment(Employment employment)
        {
            if (employment == null)
                return new ValidationResult<Employment>
                {
                    Succeeded = false,
                    Errors = new List<string> { "Employment is null" },
                    Entity = employment
                };

            var errors = new List<string>();
            var success = true;

            if (employment.EndDate.HasValue && employment.StartDate > employment.EndDate)
            {
                success = false;
                errors.Add("Employment end date should be greater than start date");
            }

            if (employment.TerminationDate.HasValue && employment.StartDate > employment.TerminationDate)
            {
                success = false;
                errors.Add("Employment termination date should be greater than start date");
            }

            if (employment.EndDate.HasValue && employment.TerminationDate.HasValue && employment.EndDate > employment.TerminationDate)
            {
                success = false;
                errors.Add("Employment termination date should be greater than or equal to end date");
            }

            return new ValidationResult<Employment>
            {
                Succeeded = success,
                Errors = errors.Any() ? errors : null,
                Entity = employment
            };

        }

        #endregion




    }
}
