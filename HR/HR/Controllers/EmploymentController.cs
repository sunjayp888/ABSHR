using HR.Business.Interfaces;
using HR.Entity;
using HR.Extensions;
using HR.Models;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace HR.Controllers
{
    [Authorize]
    public class EmploymentController : BaseController
    {
        public EmploymentController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: Employment
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: Employment/Create
        [HttpGet]
        public ActionResult Create(int personnelId)
        {
            var organisationId = UserOrganisationId;
            var reportsTo = HRBusinessService.RetrieveReportsToPersonnel(organisationId, personnelId);
            var employmentTypes = HRBusinessService.RetrieveEmploymentTypes(organisationId);
            var absencePolices = HRBusinessService.RetrieveAbsencePolices(organisationId);
            var publicHolidayPolices = HRBusinessService.RetrievePublicHolidayPolices(organisationId);
            var previousEmployment = HRBusinessService.RetrievePersonnelEmployments(organisationId, personnelId).OrderByDescending(s => s.StartDate).FirstOrDefault(); ;

            var companyBuilding = HRBusinessService.RetrieveEmploymentCompanyBuilding(organisationId).ToList();
            var defaultSelected = previousEmployment == null ? companyBuilding.FirstOrDefault() : companyBuilding.FirstOrDefault(e => e.CompanyId == previousEmployment.CompanyId && e.BuildingId == previousEmployment.BuildingId);
            var jobTitleJobgrades = HRBusinessService.RetrieveJobTitleJobGrade(organisationId).ToList();
            var defaultSelectedJobTitleJobgrades = previousEmployment == null ? jobTitleJobgrades.FirstOrDefault() : jobTitleJobgrades.FirstOrDefault(e => e.JobTitleId == previousEmployment.JobTitleId && e.JobGradeId == previousEmployment.JobGradeId);
            var employmentViewModel = new EmploymentViewModel
            {
                EmploymentCompanyBuildinglist = companyBuilding,
                SelectedBuildingId = defaultSelected.Building.BuildingId.ToString(),
                SelectedCompanyId = defaultSelected.Company.CompanyId.ToString(),
                JobTitleJobGrades = jobTitleJobgrades,
                SelectedJobTitleId = defaultSelectedJobTitleJobgrades.JobTitle.JobTitleId.ToString(),
                SelectedJobGradeId = defaultSelectedJobTitleJobgrades.JobGrade.JobGradeId.ToString(),

            };
            if (previousEmployment == null)
            {
                employmentViewModel.ReportsTo = new SelectList(reportsTo, "PersonnelId", "Fullname");
                employmentViewModel.EmploymentType = new SelectList(employmentTypes, "EmploymentTypeId", "Name");
                employmentViewModel.AbsencePolicy = new SelectList(absencePolices, "AbsencePolicyId", "Name");
                employmentViewModel.PublicHolidayPolicy = new SelectList(publicHolidayPolices, "PublicHolidayPolicyId", "Name");


                employmentViewModel.Employment = new Employment
                {
                    OrganisationId = organisationId,
                    StartDate = DateTime.Today,
                    PersonnelId = personnelId,
                    CompanyId = defaultSelected.Company.CompanyId,
                    BuildingId = defaultSelected.Building.BuildingId,
                    JobTitleId = defaultSelectedJobTitleJobgrades.JobTitle.JobTitleId,
                    JobGradeId = defaultSelectedJobTitleJobgrades.JobGrade.JobGradeId,
                };
                employmentViewModel.WorkingPatternDays = HRBusinessService.RetrieveDefaultWorkingPatternDays();
            }
            else
            {
                employmentViewModel.ReportsTo = new SelectList(reportsTo, "PersonnelId", "Fullname",
                    previousEmployment.ReportsToPersonnelId);
                employmentViewModel.EmploymentType = new SelectList(employmentTypes, "EmploymentTypeId", "Name", previousEmployment.EmploymentTypeId);
                employmentViewModel.AbsencePolicy = new SelectList(absencePolices, "AbsencePolicyId", "Name", previousEmployment.AbsencePolicyId);
                employmentViewModel.PublicHolidayPolicy = new SelectList(publicHolidayPolices, "PublicHolidayPolicyId", "Name", previousEmployment.PublicHolidayPolicyId);
                employmentViewModel.SelectedDepartmentIds = HRBusinessService.RetrieveEmploymentDepartments(organisationId, previousEmployment.EmploymentId)
                                                           .Select(e => e.DepartmentId)
                                                           .ToList();
                employmentViewModel.SelectedTeamIds = HRBusinessService.RetrieveEmploymentTeams(organisationId, previousEmployment.EmploymentId)
                                                           .Select(e => e.TeamId)
                                                           .ToList();

                employmentViewModel.Employment = new Employment
                {
                    StartDate = (previousEmployment.EndDate ?? new DateTime()).AddDays(1),
                    OrganisationId = organisationId,
                    PersonnelId = previousEmployment.PersonnelId,
                    BuildingId = previousEmployment.BuildingId,
                    ReportsToPersonnelId = previousEmployment.ReportsToPersonnelId,
                    EndEmploymentReasonId = previousEmployment.EndEmploymentReasonId,
                    WorkingPatternId = previousEmployment.WorkingPatternId,
                    CompanyId = previousEmployment.CompanyId,
                    AbsencePolicyId = previousEmployment.AbsencePolicyId,
                    PublicHolidayPolicyId = previousEmployment.PublicHolidayPolicyId,
                    EmploymentTypeId = previousEmployment.EmploymentTypeId,
                    JobGradeId = previousEmployment.JobGradeId,
                    JobTitleId = previousEmployment.JobTitleId,

                };

                employmentViewModel.PreviousEmployment = previousEmployment;
                employmentViewModel.WorkingPatternDays = previousEmployment.WorkingPattern.WorkingPatternDays.ToList();
                employmentViewModel.PreviousEmploymentEndDate = previousEmployment.EndDate;

            }
            return View(employmentViewModel);
        }

        // POST: Employment/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmploymentViewModel employmentViewModel)
        {
            var organisationId = UserOrganisationId;
            if (employmentViewModel.Employment.StartDate <= employmentViewModel.PreviousEmploymentEndDate)
                ModelState.AddModelError("EmploymentError", "Employment start date should be greater than previous employment End Date");

            if (ModelState.IsValid)
            {
                HRBusinessService.CreateEmployment(organisationId, employmentViewModel.Employment, employmentViewModel.WorkingPatternDays.ToList(), employmentViewModel.SelectedDepartmentIds, employmentViewModel.SelectedTeamIds);
                return RedirectToAction("Profile", "personnel", new { id = employmentViewModel.Employment.PersonnelId });
            }
            var previousEmployment = HRBusinessService.RetrievePersonnelCurrentEmployment(organisationId, employmentViewModel.Employment.PersonnelId);
            var absencePolices = HRBusinessService.RetrieveAbsencePolices(organisationId);
            var publicHolidayPolices = HRBusinessService.RetrievePublicHolidayPolices(organisationId);
            var companyBuilding = HRBusinessService.RetrieveEmploymentCompanyBuilding(organisationId).ToList();
            var employmentTypes = HRBusinessService.RetrieveEmploymentTypes(organisationId);
            var jobTitleJobgrades = HRBusinessService.RetrieveJobTitleJobGrade(organisationId).ToList();
            var defaultSelectedJobTitleJobgrades = previousEmployment == null ? jobTitleJobgrades.FirstOrDefault() : jobTitleJobgrades.FirstOrDefault(e => e.JobTitleId == employmentViewModel.PreviousEmployment.JobTitleId && e.JobGradeId == employmentViewModel.PreviousEmployment.JobGradeId);
            var viewModel = new EmploymentViewModel
            {
                EmploymentCompanyBuildinglist = companyBuilding,
                SelectedBuildingId = employmentViewModel.Employment.BuildingId.ToString(),
                SelectedCompanyId = employmentViewModel.Employment.CompanyId.ToString(),
                SelectedDepartmentIds = employmentViewModel.SelectedDepartmentIds,
                SelectedTeamIds = employmentViewModel.SelectedTeamIds,
                ReportsTo = new SelectList(HRBusinessService.RetrieveReportsToPersonnel(organisationId, employmentViewModel.Employment.PersonnelId), "PersonnelId", "Fullname"),
                EmploymentType = new SelectList(employmentTypes, "EmploymentTypeId", "Name"),
                AbsencePolicy = new SelectList(absencePolices, "AbsencePolicyId", "Name", previousEmployment.AbsencePolicyId),
                PublicHolidayPolicy = new SelectList(publicHolidayPolices, "PublicHolidayPolicyId", "Name", previousEmployment.PublicHolidayPolicyId),
                Employment = employmentViewModel.Employment,
                PreviousEmployment = previousEmployment,
                WorkingPatternDays = employmentViewModel.WorkingPatternDays,
                JobTitleJobGrades = jobTitleJobgrades,
                SelectedJobTitleId = defaultSelectedJobTitleJobgrades.JobTitle.JobTitleId.ToString(),
                SelectedJobGradeId = defaultSelectedJobTitleJobgrades.JobGrade.JobGradeId.ToString(),

            };
            return View(viewModel);
        }

        // GET: Employment/Edit
        [HttpGet]
        public ActionResult Edit(int personnelId, int? employmentId)
        {
            int organisationId = UserOrganisationId;
            if (employmentId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employment = HRBusinessService.RetrievePersonnelEmployment(organisationId, personnelId, employmentId.Value);

            if (employment == null)
            {
                return HttpNotFound();
            }
            var workingPatternDays = HRBusinessService.RetrieveWorkingPattern(UserOrganisationId, employment.WorkingPatternId.Value)?.WorkingPatternDays.ToList();
            var absencePolices = HRBusinessService.RetrieveAbsencePolices(organisationId);
            var publicHolidayPolices = HRBusinessService.RetrievePublicHolidayPolices(organisationId);
            var companyBuilding = HRBusinessService.RetrieveEmploymentCompanyBuilding(organisationId);
            var employmentTypes = HRBusinessService.RetrieveEmploymentTypes(organisationId);
            var jobTitleJobgrades = HRBusinessService.RetrieveJobTitleJobGrade(organisationId).ToList();
            var defaultSelectedJobTitleJobgrades = jobTitleJobgrades.FirstOrDefault(e => e.JobTitleId == employment.JobTitleId && e.JobGradeId == employment.JobGradeId);
            var viewModel = new EmploymentViewModel
            {
                AbsencePolicyName = absencePolices.FirstOrDefault(s => s.AbsencePolicyId == employment.AbsencePolicyId).Name,
                AbsencePolicy = new SelectList(absencePolices, "AbsencePolicyId", "Name", employment.AbsencePolicyId),
                PublicHolidayPolicy = new SelectList(publicHolidayPolices, "PublicHolidayPolicyId", "Name", employment.PublicHolidayPolicyId),
                EmploymentType = new SelectList(employmentTypes, "EmploymentTypeId", "Name", employment.EmploymentTypeId),
                ReportsTo = new SelectList(HRBusinessService.RetrieveReportsToPersonnel(organisationId, employment.PersonnelId), "PersonnelId", "Fullname", employment.ReportsToPersonnelId),
                Employment = employment,
                SelectedDepartmentIds = employment.EmploymentDepartments.Select(d => d.DepartmentId).ToList(),
                SelectedTeamIds = employment.EmploymentTeams.Select(t => t.TeamId).ToList(),
                SelectedCompanyId = employment.CompanyId.ToString(),
                SelectedBuildingId = employment.BuildingId.ToString(),
                EmploymentCompanyBuildinglist = companyBuilding,
                WorkingPatternDays = workingPatternDays,
                PreviousEmployment = HRBusinessService.RetrievePersonnelCurrentEmployment(organisationId, employment.PersonnelId),
                AbsencePolicyHasAbsence = HRBusinessService.AbsencePolicyPersonnelEmploymentHasAbsences(organisationId, employment.EmploymentId, employment.AbsencePolicyId),
                JobTitleJobGrades = jobTitleJobgrades,
                SelectedJobTitleId = defaultSelectedJobTitleJobgrades.JobTitle.JobTitleId.ToString(),
                SelectedJobGradeId = defaultSelectedJobTitleJobgrades.JobGrade.JobGradeId.ToString(),
                PreviousAbsencePolicyId = employment.AbsencePolicyId
            };
            return View(viewModel);
        }

        // POST: Employment/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmploymentViewModel employmentViewModel)
        {
            var organisationId = UserOrganisationId;
            if (ModelState.IsValid)
            {
                employmentViewModel.Employment = HRBusinessService.UpdateEmployment(organisationId, employmentViewModel.Employment, employmentViewModel.PreviousAbsencePolicyId, employmentViewModel.WorkingPatternDays, employmentViewModel.SelectedDepartmentIds,
                   employmentViewModel.SelectedTeamIds);
                return RedirectToAction("Profile", "Personnel", new { id = employmentViewModel.Employment.PersonnelId });
            }
            var employment = HRBusinessService.RetrievePersonnelEmployment(organisationId, employmentViewModel.Employment.PersonnelId, employmentViewModel.Employment.EmploymentId);
            employmentViewModel.Employment.Personnel = employment.Personnel;
            var absencePolices = HRBusinessService.RetrieveAbsencePolices(organisationId);
            var publicHolidayPolices = HRBusinessService.RetrievePublicHolidayPolices(organisationId);
            var companyBuilding = HRBusinessService.RetrieveEmploymentCompanyBuilding(organisationId);
            var employmentTypes = HRBusinessService.RetrieveEmploymentTypes(organisationId);
            var jobTitleJobgrades = HRBusinessService.RetrieveJobTitleJobGrade(organisationId).ToList();
            var defaultSelectedJobTitleJobgrades = jobTitleJobgrades.FirstOrDefault(e => e.JobTitleId == employmentViewModel.Employment.JobTitleId && e.JobGradeId == employmentViewModel.Employment.JobGradeId);
            var viewModel = new EmploymentViewModel
            {
                AbsencePolicyName = absencePolices.FirstOrDefault(s => s.AbsencePolicyId == employmentViewModel.Employment.AbsencePolicyId).Name,
                EmploymentCompanyBuildinglist = companyBuilding,
                SelectedBuildingId = employment.BuildingId.ToString(),
                SelectedCompanyId = employment.CompanyId.ToString(),
                AbsencePolicy = new SelectList(absencePolices, "AbsencePolicyId", "Name", employmentViewModel.Employment.AbsencePolicyId),
                PublicHolidayPolicy = new SelectList(publicHolidayPolices, "PublicHolidayPolicyId", "Name", employmentViewModel.Employment.PublicHolidayPolicyId),
                ReportsTo = new SelectList(HRBusinessService.RetrieveReportsToPersonnel(organisationId, employmentViewModel.Employment.PersonnelId), "PersonnelId", "Fullname", employmentViewModel.Employment.ReportsToPersonnelId),
                EmploymentType = new SelectList(employmentTypes, "EmploymentTypeId", "Name", employment.EmploymentTypeId),
                Employment = employmentViewModel.Employment,
                SelectedDepartmentIds = employmentViewModel.SelectedDepartmentIds,
                SelectedTeamIds = employmentViewModel.SelectedTeamIds,
                WorkingPatternDays = employmentViewModel.WorkingPatternDays,
                AbsencePolicyHasAbsence = HRBusinessService.AbsencePolicyPersonnelEmploymentHasAbsences(organisationId, employmentViewModel.Employment.EmploymentId, employment.AbsencePolicyId),
                JobTitleJobGrades = jobTitleJobgrades,
                SelectedJobTitleId = defaultSelectedJobTitleJobgrades.JobTitle.JobTitleId.ToString(),
                SelectedJobGradeId = defaultSelectedJobTitleJobgrades.JobGrade.JobGradeId.ToString(),
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UpdatePreviousEmployment(EmploymentViewModel employmentViewModel)
        {
            int organisationId = UserOrganisationId;
            if (ModelState.IsValid)
            {
                var employment = employmentViewModel.PreviousEmployment;
                var result = HRBusinessService.UpdateEmploymentEndDate(organisationId, employment);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                        return View(employmentViewModel);
                    }
                }
            }
            return RedirectToAction("Create", new { id = employmentViewModel.PreviousEmployment.PersonnelId }); ;
        }

        [HttpPost]
        public ActionResult List(int personnelId)
        {
            return this.JsonNet(HRBusinessService.RetrievePersonnelEmployments(UserOrganisationId, personnelId));
        }

        [HttpPost]
        public ActionResult CanDeletePersonnelEmployment(int personnelId, int employmentId)
        {
            return this.JsonNet(HRBusinessService.CanDeletePersonnelEmployment(UserOrganisationId, personnelId, employmentId));
        }

        [HttpPost]
        public ActionResult DeletePersonnelEmployment(int personnelId, int employmentId)
        {
            HRBusinessService.DeletePersonnelEmployment(UserOrganisationId, personnelId, employmentId);
            return this.JsonNet(true);
        }


        public PartialViewResult GetWorkingPatternRecord(int absencePolicyId, string htmlFieldPrefix)
        {
            var workingPatterns = HRBusinessService.RetrieveAbsencePolicy(UserOrganisationId, absencePolicyId).WorkingPattern;
            if (workingPatterns == null)
                return null;
            var workingPatternDays = HRBusinessService.RetrieveAbsencePolicy(UserOrganisationId, absencePolicyId)?.WorkingPattern.WorkingPatternDays;
            if (workingPatternDays == null)
                return null;
            ViewData.TemplateInfo.HtmlFieldPrefix = htmlFieldPrefix;
            return PartialView("_WorkingPattern", workingPatternDays.ToList());
        }
    }
}