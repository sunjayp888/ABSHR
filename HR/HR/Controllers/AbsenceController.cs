using HR.Attributes;
using HR.Business.Interfaces;
using HR.Entity;
using HR.Entity.Dto;
using HR.Extensions;
using HR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HR.Controllers
{
    [Authorize]
    public class AbsenceController : BaseController
    {
        private DateTime _defaultDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        public AbsenceController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: Absence/Create/{id}
        [AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult Create(int? personnelId, int? id)
        {
            var organisationId = UserOrganisationId;

            if (personnelId == null)
                personnelId = UserPersonnelId;
            var employment = HRBusinessService.RetrieveEmployment(organisationId, personnelId.Value, _defaultDate);
            var personnelAbsenceEntitlements = HRBusinessService.RetrievePersonnelCurrentAbsenceEntitlements(organisationId, personnelId.Value, employment.EmploymentId);
            if (personnelAbsenceEntitlements == null || !personnelAbsenceEntitlements.Any())
            {
                ModelState.AddModelError("",
                    "Absence entitlements may not been correctly configured, please contact your HR administrator.");
            }

            var employmentStartDate = HRBusinessService.RetrievePersonnelEmployments(organisationId, personnelId.Value).Min(e => e.StartDate);
            var employmentEndDate = personnelAbsenceEntitlements.Max(e => e.EndDate);
            var personnel = HRBusinessService.RetrievePersonnel(organisationId, personnelId.Value);

            var absenceTypes = employment.AbsencePolicy.AbsencePolicyEntitlements.Select(item => item.AbsenceType).ToList();

            if (id == null)
                id = absenceTypes.FirstOrDefault()?.AbsenceTypeId ?? 0;

            var personnelAbsenceEntitlement = personnelAbsenceEntitlements
                .FirstOrDefault(p => p.AbsenceTypeId == id.Value &&
                     p.StartDate <= _defaultDate &&
                     p.EndDate >= _defaultDate);

            if (personnelAbsenceEntitlement == null)
                personnelAbsenceEntitlement = personnelAbsenceEntitlements
                    .OrderByDescending(p => p.EndDate)
                .FirstOrDefault(p =>
                     p.StartDate <= _defaultDate &&
                     p.EndDate >= _defaultDate);

            var periodDates = HRBusinessService.RetrieveAbsencePolicyAbsencePeriods(organisationId, employment.AbsencePolicyId, null, null);
            var periodsBeginDate = periodDates.Items.Min(a => a.AbsencePeriod.StartDate).Date;
            var periodsEndDate = periodDates.Items.Max(a => a.AbsencePeriod.EndDate).Date;

            var viewModel = new AbsenceViewModel
            {
                BeginDate = periodsBeginDate,
                EndDate = periodsEndDate,
                EmploymentBeginDate = employmentStartDate,
                EmploymentEndDate = employmentEndDate,
                Absence = new AbsenceRange
                {
                    OrganisationId = organisationId,
                    BeginDateUtc = _defaultDate,
                    EndDateUtc = _defaultDate,
                    PersonnelId = personnelId.Value,
                },
                Period = personnelAbsenceEntitlement.Period,
                AbsenceTypes = new SelectList(absenceTypes, "AbsenceTypeId", "Name"),
                PersonnelAbsenceEntitlement = personnelAbsenceEntitlement
            };
            return View(viewModel);
        }

        // POST: Absence/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult Create(int personnelId, AbsenceViewModel absenceViewModel)
        {
            var organisationId = UserOrganisationId;

            if (ModelState.IsValid)
            {
                absenceViewModel.Absence.ApprovalStateId = ApprovalStates.Requested.GetHashCode();
                absenceViewModel.Absence.AbsenceStatusByUser = ApplicationUser.Id;
                absenceViewModel.Absence.AbsenceStatusDateTimeUtc = DateTime.UtcNow;
                absenceViewModel.Absence.OrganisationId = organisationId;
                var result = HRBusinessService.CreateAbsence(organisationId, absenceViewModel.Absence);
                if (result.Succeeded)
                    return RedirectToAction("Profile", "Personnel", new { id = absenceViewModel.Absence.PersonnelId });

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var employmentStartDate = HRBusinessService.RetrievePersonnelEmployments(organisationId, absenceViewModel.Absence.PersonnelId).Min(e => e.StartDate);
            var employmentEndDate = HRBusinessService.RetrievePersonnelAbsencePeriods(organisationId, absenceViewModel.Absence.PersonnelId).Max(e => e.EndDate);
            var personnelAbsenceEntitlement = HRBusinessService.RetrievePersonnelAbsenceEntitlement(organisationId, absenceViewModel.Absence.PersonnelId, absenceViewModel.Absence.PersonnelAbsenceEntitlementId);
            absenceViewModel.BeginDate = absenceViewModel.Absence.BeginDateUtc;
            absenceViewModel.EndDate = absenceViewModel.Absence.EndDateUtc;
            absenceViewModel.Period = personnelAbsenceEntitlement.Period;
            absenceViewModel.AbsenceTypes = new SelectList(HRBusinessService.RetrieveAbsencePolicyEntitlements(organisationId, personnelAbsenceEntitlement.AbsencePolicyPeriod.AbsencePolicyId).Items.Select(s => s.AbsenceType).ToList(), "AbsenceTypeId", "Name", absenceViewModel.Absence.AbsenceTypeId);
            absenceViewModel.PersonnelAbsenceEntitlement = personnelAbsenceEntitlement;

            absenceViewModel.EmploymentBeginDate = employmentStartDate;
            absenceViewModel.EmploymentEndDate = employmentEndDate;



            return View(absenceViewModel);
        }

        // GET: Absence/Edit/5
        //[AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult Edit(int personnelId, int id)
        {
            var organisationId = UserOrganisationId;
            var personnel = HRBusinessService.RetrievePersonnel(organisationId, personnelId);
            var absence = HRBusinessService.RetrieveAbsenceRange(organisationId, id);
            bool canApproveAbsence = HRBusinessService.CanApproveAbsence(organisationId, id, User.IsInRole("Admin"), ApplicationUser.Id);
            absence.CanApproveAbsence = canApproveAbsence;
            if (absence == null)
                return HttpNotFound();

            var personnelAbsenceEntitlement = HRBusinessService.RetrievePersonnelAbsenceEntitlement(organisationId, personnelId, absence.PersonnelAbsenceEntitlementId);
            if (personnelAbsenceEntitlement == null)
                throw new Exception("Absence entitlements may not been correctly configured, please contact your HR administrator.");

            personnelAbsenceEntitlement.Personnel = personnel;
            var periodDates = HRBusinessService.RetrieveAbsencePolicyAbsencePeriods(organisationId,
                personnelAbsenceEntitlement.AbsencePolicyPeriod.AbsencePolicyId, null, null);
            var periodsBeginDate = periodDates.Items.Min(a => a.AbsencePeriod.StartDate).Date;
            var periodsEndDate = periodDates.Items.Max(a => a.AbsencePeriod.EndDate).Date;
            var employmentStartDate = HRBusinessService.RetrievePersonnelEmployments(organisationId, personnelId).Min(e => e.StartDate);
            var employmentEndDate = HRBusinessService.RetrievePersonnelAbsencePeriods(organisationId, personnelId).Max(e => e.EndDate);
            var viewModel = new AbsenceViewModel
            {
                BeginDate = periodsBeginDate,
                EndDate = periodsEndDate,
                Period = personnelAbsenceEntitlement.Period,
                Absence = absence,
                AbsenceTypes = new SelectList(HRBusinessService.RetrieveAbsencePolicyEntitlements(organisationId, personnelAbsenceEntitlement.AbsencePolicyPeriod.AbsencePolicyId).Items.Select(s => s.AbsenceType).ToList(), "AbsenceTypeId", "Name", absence.AbsenceTypeId),
                PersonnelAbsenceEntitlement = personnelAbsenceEntitlement,
                Permissions = HRBusinessService.RetrievePersonnelPermissions(User.IsInRole("Admin"), organisationId, UserPersonnelId, personnelId),

                EmploymentBeginDate = employmentStartDate,
                EmploymentEndDate = employmentEndDate,
                PersonnelId = personnelId,

            };
            return View(viewModel);
        }

        // POST: Absence/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult Edit(int personnelId, AbsenceViewModel absenceViewModel)
        {
            var organisationId = UserOrganisationId;
            absenceViewModel.Absence.OrganisationId = organisationId;
            if (ModelState.IsValid)
            {
                absenceViewModel.Absence.AbsenceStatusByUser = ApplicationUser.Id;
                absenceViewModel.Absence.AbsenceStatusDateTimeUtc = DateTime.UtcNow;

                var result = HRBusinessService.UpdateAbsence(organisationId, absenceViewModel.Absence);
                if (result.Succeeded)
                    return RedirectToAction("Profile", "Personnel", new { id = absenceViewModel.Absence.PersonnelId });

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            var personnelAbsenceEntitlement = HRBusinessService.RetrievePersonnelAbsenceEntitlement(organisationId, absenceViewModel.Absence.PersonnelId, absenceViewModel.Absence.PersonnelAbsenceEntitlementId);
            var employmentStartDate = HRBusinessService.RetrievePersonnelEmployments(organisationId, absenceViewModel.Absence.PersonnelId).Min(e => e.StartDate);
            var employmentEndDate = HRBusinessService.RetrievePersonnelAbsencePeriods(organisationId, absenceViewModel.Absence.PersonnelId).Max(e => e.EndDate);
            absenceViewModel.Period = personnelAbsenceEntitlement.Period;
            absenceViewModel.AbsenceTypes = new SelectList(HRBusinessService.RetrieveAbsencePolicyEntitlements(organisationId, personnelAbsenceEntitlement.AbsencePolicyPeriod.AbsencePolicyId).Items.Select(s => s.AbsenceType).ToList(), "AbsenceTypeId", "Name", absenceViewModel.Absence.AbsenceTypeId);
            absenceViewModel.PersonnelAbsenceEntitlement = personnelAbsenceEntitlement;
            absenceViewModel.Permissions = HRBusinessService.RetrievePersonnelPermissions(User.IsInRole("Admin"), organisationId, UserPersonnelId, personnelId);
            absenceViewModel.EmploymentBeginDate = employmentStartDate;
            absenceViewModel.EmploymentEndDate = employmentEndDate;
            return View(absenceViewModel);
        }

        // POST: Absence/Cancel/5
        [HttpPost]
        [AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult Cancel(int personnelId, int id)
        {
            HRBusinessService.DeleteAbsence(UserOrganisationId, id);
            return Content(true.ToString());
        }

        [HttpPost]
        [AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult Approve(int personnelId, int id)
        {
            try
            {
                var isAdmin = User.IsInAnyRoles("Admin");
                HRBusinessService.ApproveAbsence(UserOrganisationId, id, ApplicationUser.Id, isAdmin);
                return Content(true.ToString());
            }
            catch(Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        [HttpPost]
        public ActionResult ApproveAbsence(Absence absence)
        {
            try
            {
                var isAdmin = User.IsInAnyRoles("Admin");
                HRBusinessService.ApproveAbsence(UserOrganisationId, absence, ApplicationUser.Id, isAdmin);
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
            return this.JsonNet(string.Empty);
        }

        [HttpPost]
        public ActionResult DeclineAbsence(Absence absence)
        {
            try
            {
                var isAdmin = User.IsInAnyRoles("Admin");
                HRBusinessService.DeclineAbsence(UserOrganisationId, absence, ApplicationUser.Id, isAdmin);
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
            return this.JsonNet(string.Empty);
        }

        [HttpPost]
        [AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult Decline(int personnelId, int id)
        {
            var isAdmin = User.IsInAnyRoles("Admin");
            HRBusinessService.DeclineAbsence(UserOrganisationId, id, ApplicationUser.Id, isAdmin);
            return Content(true.ToString());
        }

        [HttpPost]
        public ActionResult List(int personnelId, int absencePeriodId)
        {
            return this.JsonNet(HRBusinessService.RetrieveAbsences(UserOrganisationId, personnelId, absencePeriodId, User.IsInAnyRoles("Admin")));
        }

        [HttpPost]
        public ActionResult ListAbsenceForApprovals(int? personnelId, Paging paging, List<OrderBy> orderBy)
        {
            try
            {
                if (personnelId == null)
                    personnelId = UserPersonnelId;

                var isAdmin = User.IsInAnyRoles("Admin");
                return this.JsonNet(HRBusinessService.RetrieveAbsenceForApprovals(UserOrganisationId, ApplicationUser.Id, isAdmin, orderBy, paging));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost]
        public ActionResult Periods(int personnelId)
        {
            return this.JsonNet(HRBusinessService.RetrievePersonnelAbsencePeriods(UserOrganisationId, personnelId));
        }

        [HttpPost]
        public ActionResult AbsenceRequest(AbsenceRange absenceRange)
        {
            absenceRange.OrganisationId = UserOrganisationId;
            return this.JsonNet(HRBusinessService.RetrieveAbsenceRequest(UserOrganisationId, absenceRange));
        }

        [HttpPost]
        public ActionResult Schedule(DateTime beginDate, PersonnelFilter personnelFilter, int showColourBy)
        {
            try
            {
                var organisationId = UserOrganisationId;
                var personnelId = UserPersonnelId;
                var permissions = HRBusinessService.RetrievePersonnelPermissions(User.IsInRole("Admin"), organisationId, personnelId);
                return this.JsonNet(HRBusinessService.RetrieveAbsenceSchedule(organisationId, beginDate, beginDate.AddDays(6), permissions, UserPersonnelId, personnelFilter, showColourBy, ApplicationUser.Id));
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }

        [HttpPost]
        public ActionResult CanApproveAbsence(int personnelId, int id)
        {
            try
            {
                var organisationId = UserOrganisationId;
                var isAdmin = User.IsInAnyRoles("Admin");
                return this.JsonNet(HRBusinessService.CanApproveAbsence(organisationId, id, isAdmin, ApplicationUser.Id));
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }
    }
}
