using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using HR.Business.Extensions;
using HR.Business.Interfaces;
using HR.Business.Models;
using HR.Entity;
using HR.Entity.Dto;

namespace HR.Business
{
    public partial class HRBusinessService : IHRBusinessService
    {
        //Create
        public ValidationResult<PublicHolidayPolicy> CreatePublicHolidayPolicy(int organisationId, PublicHolidayPolicy publicHolidayPolicy)
        {
            var validationResult = PublicHolidayPolicyAlreadyExists(organisationId, null, publicHolidayPolicy.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.CreatePublicHolidayPolicy(organisationId, publicHolidayPolicy);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<AbsencePolicy> CreateAbsencePolicy(int organisationId, AbsencePolicy absencePolicy)
        {
            var validationResult = AbsencePolicyAlreadyExists(organisationId, null, absencePolicy.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.CreateAbsencePolicy(organisationId, absencePolicy);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public AbsencePolicyEntitlement CreateAbsencePolicyEntitlement(int organisationId, int absencePolicyId, int absenceTypeId)
        {
            var absencepolicyEntitlement = new AbsencePolicyEntitlement()
            {
                OrganisationId = organisationId,
                AbsenceTypeId = absenceTypeId,
                FrequencyId = 1,
                IsPaid = false,
                IsUnplanned = false,
                HasEntitlement = false,
                MaximumCarryForward = 0,
                AbsencePolicyId = absencePolicyId
            };
            return _hrDataService.CreateAbsencePolicyEntitlement(organisationId, absencePolicyId, absencepolicyEntitlement);
        }

        public JobTitleJobGrade CreateJobTitleJobGrade(int organisationId, int jobTitleId, int jobGradeId)
        {
            var jobTitleJobGrade = new JobTitleJobGrade()
            {
                OrganisationId = organisationId,
                JobTitleId = jobTitleId,
                JobGradeId = jobGradeId
            };
            return _hrDataService.CreateJobTitleJobGrade(organisationId, jobTitleJobGrade);
        }

        public ValidationResult<WorkingPattern> CreateAbsencePolicyWorkingPattern(int organisationId, AbsencePolicy absencePolicy, List<WorkingPatternDay> workingPatternDays)
        {
            var validationResult = new ValidationResult<WorkingPattern>
            {
                Succeeded = true
            };
            try
            {
                validationResult.Entity = _hrDataService.CreateAbsencePolicyWorkingPattern(organisationId, absencePolicy, workingPatternDays);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public AbsencePolicyPeriod CreateAbsencePolicyAbsencePeriod(int organisationId, AbsencePolicyPeriod absencePolicyPeriod)
        {
            absencePolicyPeriod = _hrDataService.CreateAbsencePolicyAbsencePeriod(organisationId, absencePolicyPeriod);
            var employments = _hrDataService.RetrieveActiveEmploymentsByAbsencePolicy(organisationId, absencePolicyPeriod.AbsencePolicyId);
            CreatePersonnelAbsenceEntitlements(organisationId, employments, absencePolicyPeriod.AbsencePolicyId);
            return absencePolicyPeriod;
        }

        public IEnumerable<PersonnelAbsenceEntitlement> CreatePersonnelAbsenceEntitlements(int organisationId, IEnumerable<Employment> employments, int absencePolicyId)// int divisionId, int countryId)
        {
            if (employments == null || !employments.Any())
                return null;
            var absenceTypeEntitlements = _hrDataService.RetrieveAbsencePolicyEntitlements(organisationId, absencePolicyId, null, null).Items;

            var absencePolicyAbsencePeriods = RetrieveAbsencePolicyAbsencePeriods(organisationId, absencePolicyId, null, null)?.Items;

            if (absencePolicyAbsencePeriods == null || !absencePolicyAbsencePeriods.Any())
                return null;

            absencePolicyAbsencePeriods = absencePolicyAbsencePeriods.Where(d => d.AbsencePeriod.EndDate > employments.OrderBy(by => by.StartDate).FirstOrDefault().StartDate).Distinct();

            var absencePolicyWorkingPattern = _hrDataService.RetrieveAbsencePolicy(organisationId, absencePolicyId).WorkingPattern;


            var personnelAbsenceEntitlements = new List<PersonnelAbsenceEntitlement>();

            foreach (var absencePolicyAbsencePeriod in absencePolicyAbsencePeriods)
            {
                var absencePeriodAbsencePolicyAbsenceTypeEntitlements = GenerateAbsencePolicyAbsenceTypeEntitlements(absenceTypeEntitlements, absencePolicyAbsencePeriod);

                if (absencePeriodAbsencePolicyAbsenceTypeEntitlements == null || !absencePeriodAbsencePolicyAbsenceTypeEntitlements.Any())
                    continue;

                // Entitlements per employment

                var absencePolicyEntitlements = (from employment in employments
                                                 join absencePolicyAbsenceTypeEntitlement in
                                                 absencePeriodAbsencePolicyAbsenceTypeEntitlements
                                                 on
                                                 new
                                                 {
                                                     AbsencePolicyId = employment.AbsencePolicyId
                                                 }
                                                 equals new
                                                 {
                                                     AbsencePolicyId = absencePolicyAbsenceTypeEntitlement.AbsencePolicyId
                                                 }
                                                 where employment.StartDate <= absencePolicyAbsenceTypeEntitlement.EndDate
                                                 select new
                                                 {
                                                     OrganisationId = employment.OrganisationId,
                                                     AbsencePolicyId = employment.AbsencePolicyId,
                                                     PersonnelId = employment.PersonnelId,
                                                     EmploymentId = employment.EmploymentId,
                                                     AbsencePolicyPeriodId = absencePolicyAbsencePeriod.AbsencePolicyPeriodId,
                                                     AbsenceTypeId = absencePolicyAbsenceTypeEntitlement.AbsenceTypeId > 0 ? absencePolicyAbsenceTypeEntitlement.AbsenceTypeId : (int?)null,
                                                     StartDate = absencePolicyAbsenceTypeEntitlement.StartDate.Value,
                                                     EndDate = absencePolicyAbsenceTypeEntitlement.EndDate.Value,
                                                     Entitlement = CalculateProRataEntitlement(absencePolicyAbsenceTypeEntitlement, absencePolicyWorkingPattern, absencePolicyAbsenceTypeEntitlement.StartDate.Value, absencePolicyAbsenceTypeEntitlement.EndDate.Value, employment.StartDate < absencePolicyAbsenceTypeEntitlement.StartDate.Value ? absencePolicyAbsenceTypeEntitlement.StartDate.Value : employment.StartDate),
                                                     CarriedOver = 0,
                                                     Used = 0,
                                                     MaximumCarryForward = absencePolicyAbsenceTypeEntitlement.MaximumCarryForward,
                                                     FrequencyId = absencePolicyAbsenceTypeEntitlement.FrequencyId,
                                                 })
                                                   .ToList();

                // Only return new entitlements

                var existingPersonnelAbsenceEntitlements = _hrDataService.RetrievePersonnelAbsenceEntitlements(organisationId, p => p.AbsencePolicyPeriodId == absencePolicyAbsencePeriod.AbsencePolicyPeriodId).ToList();

                var absencePeriodPersonnelAbsenceEntitlements = (from absencePolicyEntitlement in absencePolicyEntitlements
                                                                 join existingPersonnelAbsenceEntitlement in existingPersonnelAbsenceEntitlements on
                                                                 new
                                                                 {
                                                                     OrganisationId = absencePolicyEntitlement.OrganisationId,
                                                                     AbsencePolicyPeriodId = absencePolicyEntitlement.AbsencePolicyPeriodId,
                                                                     PersonnelId = absencePolicyEntitlement.PersonnelId,
                                                                     AbsenceTypeId = (int?)absencePolicyEntitlement.AbsenceTypeId,
                                                                     FrequencyId = absencePolicyEntitlement.FrequencyId,
                                                                 }
                                                                 equals new
                                                                 {
                                                                     OrganisationId = existingPersonnelAbsenceEntitlement.OrganisationId,
                                                                     AbsencePolicyPeriodId = existingPersonnelAbsenceEntitlement.AbsencePolicyPeriodId,
                                                                     PersonnelId = existingPersonnelAbsenceEntitlement.PersonnelId,
                                                                     AbsenceTypeId = existingPersonnelAbsenceEntitlement.AbsenceTypeId,
                                                                     FrequencyId = existingPersonnelAbsenceEntitlement.FrequencyId,
                                                                 }
                                                                 into existing
                                                                 from existingPersonnelAbsenceEntitlement in existing.DefaultIfEmpty()
                                                                 where existingPersonnelAbsenceEntitlement == null
                                                                 select new PersonnelAbsenceEntitlement
                                                                 {
                                                                     OrganisationId = absencePolicyEntitlement.OrganisationId,
                                                                     PersonnelId = absencePolicyEntitlement.PersonnelId,
                                                                     AbsencePolicyPeriodId = absencePolicyEntitlement.AbsencePolicyPeriodId,
                                                                     AbsenceTypeId = absencePolicyEntitlement.AbsenceTypeId,
                                                                     StartDate = absencePolicyEntitlement.StartDate,
                                                                     EndDate = absencePolicyEntitlement.EndDate,
                                                                     Entitlement = absencePolicyEntitlement.Entitlement,
                                                                     CarriedOver = 0,
                                                                     Used = 0,
                                                                     Remaining = absencePolicyEntitlement.Entitlement,
                                                                     MaximumCarryForward = absencePolicyEntitlement.MaximumCarryForward,
                                                                     FrequencyId = absencePolicyEntitlement.FrequencyId,
                                                                 })
                                                                 .ToList();

                var distinctAbsencePeriodPersonnelAbsenceEntitlements = absencePeriodPersonnelAbsenceEntitlements
                        .GroupBy(pae => new { pae.AbsenceTypeId, pae.AbsencePolicyPeriodId, pae.Period, pae.PersonnelId })
                        .Select(pae => pae.First())
                        .ToList();

                personnelAbsenceEntitlements.AddRange(distinctAbsencePeriodPersonnelAbsenceEntitlements);
            }

            if (personnelAbsenceEntitlements.Any())
            {
                //Join with 
                var data = _hrDataService.CreatePersonnelAbsenceEntitlements(organisationId, personnelAbsenceEntitlements);
                CreateEmploymentPersonnelAbsenceEntitlements(organisationId, employments, data);
                return data;
            }
            return null;
        }

        public IEnumerable<PersonnelAbsenceEntitlement> RetrieveExistingPersonnelAbsenceEntitlements(int organisationId, IEnumerable<Employment> employments, int absencePolicyId)// int divisionId, int countryId)
        {
            if (employments == null || !employments.Any())
                return null;
            var absenceTypeEntitlements = _hrDataService.RetrieveAbsencePolicyEntitlements(organisationId, absencePolicyId, null, null).Items;
            var absencePolicyAbsencePeriods = RetrieveAbsencePolicyAbsencePeriods(organisationId, absencePolicyId, null, null)?.Items;
            if (absencePolicyAbsencePeriods == null || !absencePolicyAbsencePeriods.Any())
                return null;
            absencePolicyAbsencePeriods = absencePolicyAbsencePeriods.Where(d => d.AbsencePeriod.EndDate > employments.OrderBy(by => by.StartDate).FirstOrDefault().StartDate).Distinct();
            List<PersonnelAbsenceEntitlement> existingPersonnelAbsenceEntitlementsList = new List<PersonnelAbsenceEntitlement>();
            foreach (var absencePolicyAbsencePeriod in absencePolicyAbsencePeriods)
            {
                var absencePeriodAbsencePolicyAbsenceTypeEntitlements = GenerateAbsencePolicyAbsenceTypeEntitlements(absenceTypeEntitlements, absencePolicyAbsencePeriod);
                if (absencePeriodAbsencePolicyAbsenceTypeEntitlements == null || !absencePeriodAbsencePolicyAbsenceTypeEntitlements.Any())
                    continue;
                var existingPersonnelAbsenceEntitlements = _hrDataService.RetrievePersonnelAbsenceEntitlements(organisationId, p => p.AbsencePolicyPeriodId == absencePolicyAbsencePeriod.AbsencePolicyPeriodId).ToList();
                existingPersonnelAbsenceEntitlementsList.AddRange(existingPersonnelAbsenceEntitlements);
            }
            return existingPersonnelAbsenceEntitlementsList;
        }

        private void CreateEmploymentPersonnelAbsenceEntitlements(int organisationId, IEnumerable<Employment> employments, IEnumerable<PersonnelAbsenceEntitlement> employmentPersonnelAbsenceEntitlements)
        {

            var data = employmentPersonnelAbsenceEntitlements.Join(employments, p => p.PersonnelId, e => e.PersonnelId,
                (p, e) =>
                    new { EmploymentId = e.EmploymentId, PersonnelAbsenceEntitlementId = p.PersonnelAbsenceEntitlementId });

            var employmentPersonnelAbsenceEntitlement =
              data.Select(item => new EmploymentPersonnelAbsenceEntitlement()
              {
                  EmploymentId = item.EmploymentId,
                  PersonnelAbsenceEntitlementId = item.PersonnelAbsenceEntitlementId,
                  OrganisationId = organisationId
              }).ToList();

            _hrDataService.Create<EmploymentPersonnelAbsenceEntitlement>(organisationId, employmentPersonnelAbsenceEntitlement);
        }

        public double? CalculateProRataEntitlement(AbsencePolicyEntitlement absencePolicyEntitlement, WorkingPattern absencePolicyWorkingPattern, DateTime periodStartDate, DateTime periodEndDate, DateTime employmentStartDate)
        {
            if (absencePolicyEntitlement == null || absencePolicyWorkingPattern == null)
                return null;

            if (absencePolicyEntitlement.Entitlement == 0)
                return 0;

            if (employmentStartDate == periodStartDate)
                return absencePolicyEntitlement.Entitlement;

            var entitlementForWeek = (absencePolicyEntitlement.Entitlement * 7) / (absencePolicyEntitlement.EndDate.Value - absencePolicyEntitlement.StartDate.Value).TotalDays;
            var weekLeft = ((periodEndDate - employmentStartDate).TotalDays) / 7;
            var entitlement = (entitlementForWeek * weekLeft);

            // round to nearest half day
            return Math.Round(entitlement * 2, MidpointRounding.AwayFromZero) / 2;
        }

        private List<AbsencePolicyEntitlement> GenerateAbsencePolicyAbsenceTypeEntitlements(IEnumerable<AbsencePolicyEntitlement> absencePolicyEntitlements, AbsencePolicyPeriod absencePolicyPeriod)
        {
            var absencePeriodAbsencePolicyAbsenceTypeEntitlements = new List<AbsencePolicyEntitlement>();

            if (absencePolicyPeriod == null)
                return null;

            if (absencePolicyEntitlements != null)
            {
                foreach (var dcEntitlement in absencePolicyEntitlements)
                {
                    dcEntitlement.StartDate = absencePolicyPeriod.AbsencePeriod.StartDate;
                    if (dcEntitlement.Frequency.Name.Trim().ToLower() == "yearly")
                    {
                        dcEntitlement.EndDate = absencePolicyPeriod.AbsencePeriod.EndDate;
                        absencePeriodAbsencePolicyAbsenceTypeEntitlements.Add(dcEntitlement);
                    }
                    else
                    {
                        dcEntitlement.EndDate = absencePolicyPeriod.AbsencePeriod.StartDate.AddMonths(3).AddDays(-1);
                        absencePeriodAbsencePolicyAbsenceTypeEntitlements.Add(dcEntitlement);
                        for (int i = 1; i < 4; i++)
                        {
                            var extraEntitlement = dcEntitlement.Copy();
                            extraEntitlement.StartDate = absencePolicyPeriod.AbsencePeriod.StartDate.AddMonths(i * 3);
                            extraEntitlement.EndDate = extraEntitlement.StartDate.Value.AddMonths(3).AddDays(-1);
                            absencePeriodAbsencePolicyAbsenceTypeEntitlements.Add(extraEntitlement);
                        }
                    }
                }
            }

            // Set to -1 so we can set to null later

            var nullOthersEntitlement = new AbsencePolicyEntitlement
            {
                OrganisationId = absencePolicyPeriod.OrganisationId,
                AbsenceTypeId = 0,
                FrequencyId = 1,
                Entitlement = 0,
                MaximumCarryForward = 0,
                AbsencePolicyId = absencePolicyPeriod.AbsencePolicyId,
                Frequency = new Frequency { FrequencyId = 1, Name = "yearly" },
                StartDate = absencePolicyPeriod.AbsencePeriod.StartDate,
                EndDate = absencePolicyPeriod.AbsencePeriod.EndDate,
                //HasEntitlement = false
            };
            absencePeriodAbsencePolicyAbsenceTypeEntitlements.Add(nullOthersEntitlement);
            return absencePeriodAbsencePolicyAbsenceTypeEntitlements;
        }

        public ValidationResult<PublicHoliday> CreatePublicHoliday(int organisationId, PublicHoliday publicHoliday)
        {
            var validationResult = PublicHolidayNameAlreadyExists(organisationId, publicHoliday.PublicHolidayPolicyId, null, publicHoliday.Date);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                _hrDataService.CreatePublicHolidays(organisationId, new List<PublicHoliday> { publicHoliday });
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public Personnel CreatePersonnel(int organisationId, Personnel personnel, Employment employment, IEnumerable<WorkingPatternDay> workingPatternDays, List<int> departmentIds, List<int> teamIds)
        {
            var createdPersonnel = CreatePersonnel(organisationId, personnel);
            employment.PersonnelId = createdPersonnel.PersonnelId;
            employment.OrganisationId = organisationId;
            CreateEmployment(organisationId, employment, workingPatternDays, departmentIds, teamIds);
            return createdPersonnel;
        }

        public Employment CreateEmployment(int organisationId, Employment employment, IEnumerable<WorkingPatternDay> workingPatternDays, List<int> departmentIds, List<int> teamIds)
        {
            //Create Working Pattern
            var workingPattern = CreateWorkingPatternDays(organisationId, workingPatternDays);
            employment.WorkingPatternId = workingPattern.WorkingPatternId;

            //Create Employment
            employment = _hrDataService.CreateEmployment(organisationId, employment);

            //Create Department Employment
            var employmentdepartment = departmentIds.Select(item => new EmploymentDepartment()
            {
                OrganisationId = organisationId,
                DepartmentId = item,
                EmploymentId = employment.EmploymentId
            }).ToList();
            _hrDataService.Create<EmploymentDepartment>(organisationId, employmentdepartment);

            //Create Team Employment
            var employmentTeam = teamIds.Select(item => new EmploymentTeam()
            {
                OrganisationId = organisationId,
                TeamId = item,
                EmploymentId = employment.EmploymentId
            }).ToList();
            _hrDataService.Create<EmploymentTeam>(organisationId, employmentTeam);

            UpdatePersonnelCurrentEmployment(organisationId, employment.EmploymentId, employment.PersonnelId);
            var data = CreatePersonnelAbsenceEntitlements(organisationId, new List<Employment> { employment }, employment.AbsencePolicyId);
            //Create only PersonnelAbsenceEntitlements Mapping if same policy
            if (data == null)
            {
                var existingPersonnelAbsenceEntitlements = RetrieveExistingPersonnelAbsenceEntitlements(organisationId,
                    new List<Employment> { employment }, employment.AbsencePolicyId);
                CreateEmploymentPersonnelAbsenceEntitlements(organisationId, new List<Employment> { employment }, existingPersonnelAbsenceEntitlements);
            }
            ResetEmploymentsTree(organisationId);
            return employment;
        }

        //Retrieve
        public PagedResult<PublicHolidayPolicy> RetrievePublicHolidayPolicies(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrievePublicHolidayPolicies(organisationId, p => true, orderBy, paging);
        }

        private ValidationResult<AbsencePolicy> AbsencePolicyAlreadyExists(int organisationId, int? absencePolicyId, string name)
        {
            var alreadyExists = _hrDataService.RetrieveAbsencePolicies(organisationId,
                c => c.Name.ToLower() == name.Trim().ToLower() && c.AbsencePolicyId != (absencePolicyId ?? -1)).Items.Any();
            return new ValidationResult<AbsencePolicy>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "AbsencePolicy already exists." } : null
            };
        }

        public PublicHolidayPolicy RetrievePublicHolidayPolicy(int organisationId, int publicHolidayPolicyId)
        {
            return _hrDataService.RetrievePublicHolidayPolicies(organisationId, p => p.PublicHolidayPolicyId == publicHolidayPolicyId).Items.FirstOrDefault();
        }

        private ValidationResult<PublicHolidayPolicy> PublicHolidayPolicyAlreadyExists(int organisationId, int? publicHolidayPolicyId, string name)
        {
            var alreadyExists =
                _hrDataService.RetrievePublicHolidayPolicies(organisationId,
                    c => c.Name.ToLower() == name.Trim().ToLower() && c.PublicHolidayPolicyId != (publicHolidayPolicyId ?? -1)).Items.Any();

            return new ValidationResult<PublicHolidayPolicy>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "Public Holiday Policy already exists." } : null
            };
        }

        private ValidationResult<PublicHoliday> PublicHolidayNameAlreadyExists(int organisationId, int publicHolidayPolicyId, int? publicHolidayId, DateTime date)
        {
            var alreadyExists = _hrDataService.RetrievePublicHolidays(organisationId, publicHolidayPolicyId, p => true)
                               .Items.Any(p => p.Date == date.Date && p.PublicHolidayId != (publicHolidayId ?? -1));
            return new ValidationResult<PublicHoliday>
            {
                Succeeded = !alreadyExists,
                Errors = alreadyExists ? new List<string> { "Public Holiday already exists." } : null
            };
        }

        public List<int> RetrievePublicHolidayYear(int organisationId, int publicHolidayPolicyId)
        {
            return _hrDataService.RetrievePublicHolidays(organisationId, publicHolidayPolicyId, p => true)
                    .Items
                    .Select(s => s.Date.Year)
                    .Distinct().OrderBy(s => s).ToList();
        }

        public PagedResult<PublicHoliday> RetrievePublicHolidays(int organisationId, int publicHolidayPolicyId, int year, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrievePublicHolidays(organisationId, publicHolidayPolicyId, p => p.Date.Year == year, orderBy, paging);
        }

        public PagedResult<AbsencePolicy> RetrieveAbsencePolicies(int organisationId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveAbsencePolicies(organisationId, p => true, orderBy, paging);
        }

        public PagedResult<AbsencePolicyEntitlement> RetrieveAbsencePolicyEntitlements(int organisationId, int absencePolicyId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveAbsencePolicyEntitlements(organisationId, absencePolicyId, orderBy, paging);
        }

        public IEnumerable<AbsenceType> RetrieveUnassignedAbsencePolicyAbsenceTypes(int organisationId, int absencePolicyId)
        {
            return _hrDataService.RetrieveAbsenceTypes(organisationId, a => !a.AbsencePolicyEntitlements.Any(d => d.AbsencePolicyId == absencePolicyId), null, null).Items.ToList();
        }

        public IEnumerable<JobGrade> RetrieveUnassignedJobGrades(int organisationId, int jobtitleId)
        {
            return _hrDataService.RetrieveJobGrades(organisationId, a => !a.JobTitleJobGrades.Any(d => d.JobTitleId == jobtitleId), null, null).Items.ToList();
        }

        public PagedResult<JobGrade> RetrieveJobTitleJobGrade(int organisationId, int jobGradeId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveJobTitleJobGrade(organisationId, jobGradeId, orderBy, paging);
        }

        public AbsencePolicyEntitlement RetrieveAbsencePolicyEntitlement(int organisationId, int absencePolicyEntitlementId)
        {
            return _hrDataService.RetrieveAbsencePolicyEntitlement(organisationId, absencePolicyEntitlementId);
        }

        public AbsencePolicy RetrieveAbsencePolicy(int organisationId, int absencePolicyId)
        {
            return _hrDataService.RetrieveAbsencePolicy(organisationId, absencePolicyId);
        }

        public PagedResult<AbsencePolicyPeriod> RetrieveAbsencePolicyAbsencePeriods(int organisationId, int absencePolicyId, List<OrderBy> orderBy, Paging paging)
        {
            return _hrDataService.RetrieveAbsencePolicyAbsencePeriods(organisationId, absencePolicyId, orderBy, paging);
        }

        public IEnumerable<AbsencePeriod> RetrieveUnassignedAbsencePolicyPeriods(int organisationId, int absencePolicyId)
        {
            //get unassigned absence periods 
            var unassignedAbsencePeriods = _hrDataService.RetrieveAbsencePeriods(organisationId, a => !a.AbsencePolicyPeriods.Any(d => d.AbsencePolicyId == absencePolicyId), null, null).Items.ToList();
            //get assigned absence periods
            var assignedAbsencePeriods = _hrDataService.RetrieveAbsencePeriods(organisationId, a => a.AbsencePolicyPeriods.Any(d => d.AbsencePolicyId == absencePolicyId), null, null).Items.ToList();
            return unassignedAbsencePeriods.Where(ua => !ua.OverlapsWithAny(assignedAbsencePeriods));
        }

        public bool CanDeletePublicHolidayPolicy(int organisationId, int publicHolidayPolicyId)
        {
            return _hrDataService.RetrievePublicHolidayPolicies(organisationId, p => p.PublicHolidayPolicyId == publicHolidayPolicyId && !p.Employments.Any()).Items.Any();
        }

        public bool CanDeleteAbsencePolicy(int organisationId, int absencePolicyId)
        {
            var absencePolicyEntitlements =
                _hrDataService.RetrieveEmployments(organisationId, p => p.AbsencePolicyId == absencePolicyId).Any();
            return absencePolicyEntitlements;
        }

        public bool IsAbsencesAssignedToAbsencePolicyPeriod(int organisationId, int absencePolicyPeriodId)
        {
            return _hrDataService.RetrieveAbsencesOfAbsencePolicyPeriod(organisationId, absencePolicyPeriodId);
        }

        public bool IsAbsencesAssignedToAbsencePolicyAbsenceType(int organisationId, int absencePolicyId, int absenceTypeId)
        {
            return _hrDataService.RetrieveAbsencesOfAbsencePolicyAbsenceType(organisationId, absencePolicyId, absenceTypeId);
        }
        public IEnumerable<AbsencePolicy> RetrieveAbsencePolices(int organisationId)
        {
            var absencePolicy = _hrDataService.Retrieve<AbsencePolicy>(organisationId, p => true).ToList();
            return absencePolicy;
        }
        public IEnumerable<PublicHolidayPolicy> RetrievePublicHolidayPolices(int organisationId)
        {
            var publicHolidayPolicy = _hrDataService.Retrieve<PublicHolidayPolicy>(organisationId, p => true).ToList();
            return publicHolidayPolicy;
        }
        public bool AbsencePolicyPersonnelEmploymentHasAbsences(int organisationId, int employmentId, int absencePolicyId)
        {
            var result = _hrDataService.AbsencePolicyPersonnelEmploymentHasAbsences(organisationId, employmentId, absencePolicyId);
            return result;

        }

        public Employment RetrieveEmployment(int organisationId, int personnelId, DateTime dateTimeNow)
        {
            return _hrDataService.RetrieveEmployment(organisationId, personnelId, dateTimeNow);
        }

        public IEnumerable<EmploymentPersonnelAbsenceEntitlement> RetrieveEmploymentPersonnelAbsenceEntitlements(int organisationId, Expression<Func<EmploymentPersonnelAbsenceEntitlement, bool>> predicate)
        {
            return _hrDataService.RetrieveEmploymentPersonnelAbsenceEntitlements(organisationId, predicate);
        }

        public IEnumerable<AbsencePolicyPeriod> RetrieveAbsencePolicyAbsencePeriodsByPersonnel(int organisationId, int personnelId)
        {
            return _hrDataService.RetrieveAbsencePolicyAbsencePeriodsByPersonnel(organisationId, personnelId, null, null).Items.ToList();
        }
        //Update
        public ValidationResult<PublicHoliday> UpdatePublicHoliday(int organisationId, PublicHoliday publicHoliday)
        {
            var validationResult = PublicHolidayNameAlreadyExists(organisationId, publicHoliday.PublicHolidayPolicyId, publicHoliday.PublicHolidayId, publicHoliday.Date);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                _hrDataService.UpdateOrganisationEntityEntry(organisationId, publicHoliday);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<WorkingPattern> UpdateAbsencePolicy(int organisationId, AbsencePolicy absencePolicy, List<WorkingPatternDay> workingPatternDays)
        {
            var validationResult = new ValidationResult<WorkingPattern>
            {
                Succeeded = true
            };
            var absencePolicyAlreadyExists = AbsencePolicyAlreadyExists(organisationId, absencePolicy.AbsencePolicyId, absencePolicy.Name);
            if (!absencePolicyAlreadyExists.Succeeded)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { absencePolicyAlreadyExists.Errors.FirstOrDefault() };
                return validationResult;
            }

            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                _hrDataService.UpdateOrganisationEntityEntry(organisationId, absencePolicy);
                _hrDataService.UpdateAbsencePolicyWorkingPattern(organisationId, absencePolicy.WorkingPatternId.Value, workingPatternDays);
                validationResult.Entity = _hrDataService.RetrieveAbsencePolicy(organisationId, absencePolicy.AbsencePolicyId).WorkingPattern;
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public ValidationResult<PublicHolidayPolicy> UpdatePublicHolidayPolicy(int organisationId, PublicHolidayPolicy publicHolidayPolicy)
        {
            var validationResult = PublicHolidayPolicyAlreadyExists(organisationId, publicHolidayPolicy.PublicHolidayPolicyId, publicHolidayPolicy.Name);
            if (!validationResult.Succeeded)
            {
                return validationResult;
            }
            try
            {
                validationResult.Entity = _hrDataService.UpdateOrganisationEntityEntry(organisationId, publicHolidayPolicy);
            }
            catch (Exception ex)
            {
                validationResult.Succeeded = false;
                validationResult.Errors = new List<string> { ex.InnerMessage() };
                validationResult.Exception = ex;
            }
            return validationResult;
        }

        public AbsencePolicyEntitlement UpdateAbsencePolicyEntitlement(int organisationId, AbsencePolicyEntitlement absencePolicyEntitlement)
        {
            absencePolicyEntitlement = _hrDataService.UpdateOrganisationEntityEntry(organisationId, absencePolicyEntitlement);
            var employments = _hrDataService.RetrieveActiveEmploymentsByAbsencePolicy(organisationId, absencePolicyEntitlement.AbsencePolicyId);
            CreatePersonnelAbsenceEntitlements(organisationId, employments, absencePolicyEntitlement.AbsencePolicyId);
            return absencePolicyEntitlement;
        }

        public Employment UpdateEmployment(int organisationId, Employment employment, int previousAbsencePolicyId, IEnumerable<WorkingPatternDay> workingPatternDays, List<int> departmentIds, List<int> teamIds)
        {
            var updatedEmployment = _hrDataService.UpdateOrganisationEntityEntry(organisationId, employment);
            _hrDataService.DeleteRange<EmploymentDepartment>(organisationId, p => p.EmploymentId == employment.EmploymentId);
            _hrDataService.DeleteRange<EmploymentTeam>(organisationId, p => p.EmploymentId == employment.EmploymentId);
            _hrDataService.DeleteRange<WorkingPatternDay>(organisationId, p => p.WorkingPatternId == employment.WorkingPatternId);
            var employments = _hrDataService.RetrievePersonnelEmployments(organisationId, employment.PersonnelId).Where(e => e.EmploymentId == employment.EmploymentId);
            //Create Team,Department and Working pattern for Employment
            var employmentdepartment = departmentIds.Select(item => new EmploymentDepartment()
            {
                OrganisationId = organisationId,
                DepartmentId = item,
                EmploymentId = employment.EmploymentId
            }).ToList();

            var employmentTeam = teamIds.Select(item => new EmploymentTeam()
            {
                OrganisationId = organisationId,
                TeamId = item,
                EmploymentId = employment.EmploymentId
            }).ToList();

            var hasAbsences = AbsencePolicyPersonnelEmploymentHasAbsences(organisationId, employment.EmploymentId, previousAbsencePolicyId);
            if (previousAbsencePolicyId != employment.AbsencePolicyId && !hasAbsences)
            {
                DeletePersonnelEntitlementsAndMapping(organisationId, employment.PersonnelId, employment.EmploymentId);
                var employmentData = new List<Employment> { employment };
                var data = CreatePersonnelAbsenceEntitlements(organisationId, employmentData, updatedEmployment.AbsencePolicyId);
                //If same policy add only PersonnelAbsenceEntitlements Mappings
                if (data == null)
                {
                    var existingPersonnelAbsenceEntitlements = RetrieveExistingPersonnelAbsenceEntitlements(organisationId,
                        new List<Employment> { employment }, employment.AbsencePolicyId);
                    CreateEmploymentPersonnelAbsenceEntitlements(organisationId, new List<Employment> { employment }, existingPersonnelAbsenceEntitlements);
                }
            }
            _hrDataService.Create<EmploymentDepartment>(organisationId, employmentdepartment);
            _hrDataService.Create<EmploymentTeam>(organisationId, employmentTeam);
            workingPatternDays.ToList().ForEach(p => p.WorkingPatternId = employment.WorkingPatternId ?? 0);
            _hrDataService.Create<WorkingPatternDay>(organisationId, workingPatternDays);
            ResetEmploymentsTree(organisationId);
            return updatedEmployment;
        }

        private void DeletePersonnelEntitlementsAndMapping(int organisationId, int personnelId, int employmentId)
        {
            var personnelAbsenceEntitlements = _hrDataService.RetrievePersonnelAbsenceEntitlements(organisationId, personnelId, employmentId);

            _hrDataService.DeleteRange<EmploymentPersonnelAbsenceEntitlement>(organisationId, p => p.EmploymentId == employmentId);
            var employmentPersonnelAbsenceEntitlements = _hrDataService.RetrieveEmploymentPersonnelAbsenceEntitlements(organisationId, e => e.PersonnelAbsenceEntitlement.PersonnelId == personnelId);

            var hasMapping = (from pe in personnelAbsenceEntitlements
                              join employmentPersonnelAbsenceEntitlement in employmentPersonnelAbsenceEntitlements on
                                  pe.PersonnelAbsenceEntitlementId equals
                                  employmentPersonnelAbsenceEntitlement.PersonnelAbsenceEntitlementId
                              select pe).ToList();

            if (hasMapping.Count == 0)
            {
                foreach (var item in personnelAbsenceEntitlements)
                {
                    _hrDataService.Delete<PersonnelAbsenceEntitlement>(organisationId,
                        p => p.PersonnelAbsenceEntitlementId == item.PersonnelAbsenceEntitlementId);
                }
            }
        }

        //Clone
        public PublicHolidayPolicy ClonePublicHolidayPolicy(int organisationId, int publicHolidayPolicyId)
        {
            var publicHolidayPolicy = RetrievePublicHolidayPolicy(organisationId, publicHolidayPolicyId);
            var clonedPublicHolidayPolicy = new PublicHolidayPolicy
            {
                Name = string.Format("{0} {1}", publicHolidayPolicy.Name, DateTime.UtcNow.ToString("HH:mm:ss")),
            };

            var result = CreatePublicHolidayPolicy(organisationId, clonedPublicHolidayPolicy);
            if (result.Succeeded)
            {
                var publicHolidays = _hrDataService.RetrievePublicHolidays(organisationId,
                    publicHolidayPolicyId, p => true).Items.ToList();
                publicHolidays.ForEach(p => p.PublicHolidayPolicyId = result.Entity.PublicHolidayPolicyId);
                _hrDataService.CreatePublicHolidays(organisationId, publicHolidays);
            }
            return result.Entity;
        }

        public AbsencePolicy CloneAbsencePolicy(int organisationId, int absencePolicyId)
        {
            var absencePolicy = RetrieveAbsencePolicy(organisationId, absencePolicyId);
            var absencePolicyEntitlements = absencePolicy.AbsencePolicyEntitlements;
            var absencePolicyPeriods = absencePolicy.AbsencePolicyPeriods;

            var cloneAbsencePolicy = new AbsencePolicy()
            {
                Name = string.Format("{0} {1}", absencePolicy.Name, DateTime.UtcNow.ToString("HH:mm:ss")),
                OrganisationId = organisationId
            };
            //Clone Absence Policy
            var clonedAbsencePolicy = CreateAbsencePolicy(organisationId, cloneAbsencePolicy);
            if (clonedAbsencePolicy.Succeeded)
            {
                //Clone Working Pattern
                if (absencePolicy.WorkingPattern != null)
                {
                    var absencePolicyWorkingPatternDays = absencePolicy.WorkingPattern.WorkingPatternDays;
                    var workingPatternDays = absencePolicyWorkingPatternDays.Select(item => new WorkingPatternDay()
                    {
                        AM = item.AM,
                        DayOfWeek = item.DayOfWeek,
                        PM = item.PM
                    }).ToList();
                    //Clone Working Pattern.
                    _hrDataService.CreateAbsencePolicyWorkingPattern(organisationId, clonedAbsencePolicy.Entity, workingPatternDays);
                }
                //Clone Absence Policy Entitlement.
                if (absencePolicyEntitlements.Any())
                {
                    _hrDataService.Create<AbsencePolicyEntitlement>(organisationId,
                        absencePolicyEntitlements.Select(item => new AbsencePolicyEntitlement()
                        {
                            OrganisationId = organisationId,
                            AbsenceTypeId = item.AbsenceTypeId,
                            FrequencyId = item.FrequencyId,
                            IsPaid = item.IsPaid,
                            IsUnplanned = item.IsUnplanned,
                            HasEntitlement = item.HasEntitlement,
                            MaximumCarryForward = item.MaximumCarryForward,
                            AbsencePolicyId = clonedAbsencePolicy.Entity.AbsencePolicyId,
                            Entitlement = item.Entitlement
                        }));
                }
                //Clone Absence Policy Periods.
                if (absencePolicyPeriods.Any())
                {
                    _hrDataService.Create<AbsencePolicyPeriod>(organisationId,
                        absencePolicyPeriods.Select(item => new AbsencePolicyPeriod()
                        {
                            OrganisationId = organisationId,
                            AbsencePeriodId = item.AbsencePeriodId,
                            AbsencePolicyId = clonedAbsencePolicy.Entity.AbsencePolicyId
                        }));
                }
            }
            return clonedAbsencePolicy.Entity;
        }

        //Delete

        public void DeletePublicHoliday(int organisationId, int publicHolidayId)
        {
            _hrDataService.DeletePublicHoliday(organisationId, publicHolidayId);
        }

        public void DeletePublicHolidayPolicy(int organisationId, int publicHolidayPolicyId)
        {
            _hrDataService.DeletePublicHolidayPolicy(organisationId, publicHolidayPolicyId);
        }

        public void DeleteAbsencePolicyPeriod(int organisationId, int absencePolicyPeriodId)
        {
            _hrDataService.DeleteAbsencePolicyPeriod(organisationId, absencePolicyPeriodId);
        }

        public void DeleteAbsencePolicy(int organisationId, int id)
        {
            _hrDataService.DeleteAbsencePolicy(organisationId, id);
        }

        public void DeleteAbsencePolicyAbsenceType(int organisationId, int absencePolicyId, int absenceTypeId)
        {
            _hrDataService.DeletePersonnelAbsenceEntitlementsForAbsencePolicyAbsenceType(organisationId, absencePolicyId, absenceTypeId);
            var absencePolicyEntitlementId =
                _hrDataService.RetrieveAbsencePolicyEntitlements(organisationId, absencePolicyId, null, null)
                    .Items.First(s => s.AbsenceTypeId == absenceTypeId).AbsencePolicyEntitlementId;
            _hrDataService.DeleteAbsencePolicyAbsenceType(organisationId, absencePolicyEntitlementId);
        }

        public void DeleteJobTitleJobGrade(int organisationId, int jobTitleId, int jobGradeId)
        {
            _hrDataService.DeleteJobTitleJobGrade(organisationId, jobTitleId, jobGradeId);
        }
    }
}