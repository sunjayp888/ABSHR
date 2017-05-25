using HR.Attributes;
using HR.Business.Interfaces;
using HR.Entity;
using HR.Entity.Dto;
using HR.Extensions;
using HR.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace HR.Controllers
{
    [Authorize]
    public class OvertimeController : BaseController
    {
        public OvertimeController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        [HttpGet]
        [AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult Create(int? personnelId)
        {
            var overtimePreferences = HRBusinessService.RetrieveOvertimePreferences(UserOrganisationId);
            var organisationId = UserOrganisationId;
            if (personnelId == null)
                personnelId = UserPersonnelId;
            var personnel = HRBusinessService.RetrievePersonnel(organisationId, personnelId.Value);
            var overtimeSummary = HRBusinessService.RetrieveOvertimeSummary(UserOrganisationId, personnelId.Value);
            var viewModel = new OvertimeViewModel
            {
                Overtime = new Overtime
                {
                    OrganisationId = organisationId,
                    PersonnelId = personnelId.Value,
                    Date = DateTime.Today,
                    CreatedBy = ApplicationUser.UserName,
                    Personnel = personnel
                },
                OvertimePreferences = new SelectList(overtimePreferences, "OvertimePreferenceId", "Name"),
                OvertimeSummary = overtimeSummary ?? new OvertimeSummary()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult Create(OvertimeViewModel overtimeViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreateOvertime(UserOrganisationId, overtimeViewModel.Overtime);
                if (result.Succeeded)
                    return RedirectToAction("profile", "personnel", new { id = overtimeViewModel.PersonnelId });

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var overtimePreferences = HRBusinessService.RetrieveOvertimePreferences(UserOrganisationId);
            var overtimeSummary = HRBusinessService.RetrieveOvertimeSummary(UserOrganisationId, overtimeViewModel.PersonnelId);
            var personnel = HRBusinessService.RetrievePersonnel(UserOrganisationId, overtimeViewModel.PersonnelId);
            overtimeViewModel.Overtime.Personnel = personnel;
            var viewModel = new OvertimeViewModel
            {
                Overtime = overtimeViewModel.Overtime,
                OvertimePreferences = new SelectList(overtimePreferences, "OvertimePreferenceId", "Name"),
                OvertimeSummary = overtimeSummary

            };
            return View(viewModel);
        }

        [HttpPost]
        [AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult CreateOvertimeAdjustment(OvertimeSummary overtimeSummary, string comment)
        {
            try
            {
                HRBusinessService.CreateOvertimeAdjustment(UserOrganisationId, overtimeSummary, ApplicationUser.UserName, comment);
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
            return this.JsonNet(string.Empty);
        }

        [HttpPost]
        public ActionResult Approve(int personnelId, int id)
        {
            try
            {
                var isAdmin = User.IsInAnyRoles("Admin");
                HRBusinessService.ApproveOvertime(UserOrganisationId, id, ApplicationUser.Id, isAdmin);
                return Content(true.ToString());
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        [HttpPost]
        public ActionResult ApproveOvertime(Overtime overtime)
        {
            try
            {
                var isAdmin = User.IsInAnyRoles("Admin");
                overtime.UpdatedBy = ApplicationUser.UserName;
                overtime.UpdatedDateUtc = DateTime.UtcNow;
                HRBusinessService.ApproveOvertime(UserOrganisationId, overtime, ApplicationUser.Id, isAdmin);
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
            return this.JsonNet(string.Empty);
        }

        [HttpPost]
        public ActionResult DeclineOvertime(Overtime overtime)
        {
            try
            {
                var isAdmin = User.IsInAnyRoles("Admin");
                overtime.UpdatedBy = ApplicationUser.UserName;
                overtime.UpdatedDateUtc = DateTime.UtcNow;
                HRBusinessService.DeclineOvertime(UserOrganisationId, overtime, ApplicationUser.Id, isAdmin);
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
            return this.JsonNet(string.Empty);
        }

        [HttpPost]
        public ActionResult Decline(int personnelId, int id)
        {
            var isAdmin = User.IsInAnyRoles("Admin");
            HRBusinessService.DeclineOvertime(UserOrganisationId, id, ApplicationUser.Id, isAdmin);
            return Content(true.ToString());
        }

        // GET: Overtime/Edit/{id}
        [HttpGet]
        public ActionResult Edit(int? personnelId, int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var overtime = HRBusinessService.RetrieveOvertime(UserOrganisationId, id.Value);
            if (overtime == null)
                return HttpNotFound();

            bool isAdmin = User.IsInRole("Admin");

            var overtimePreferences = HRBusinessService.RetrieveOvertimePreferences(UserOrganisationId);
            var overtimeSummary = HRBusinessService.RetrieveOvertimeSummary(UserOrganisationId, personnelId.Value);
            var permissions = HRBusinessService.RetrievePersonnelPermissions(isAdmin, UserOrganisationId, personnelId.Value);
            bool canApproveOvertime = HRBusinessService.CanApproveOvertime(UserOrganisationId, id.Value, isAdmin, ApplicationUser.Id);
            var viewModel = new OvertimeViewModel
            {
                Overtime = overtime,
                OvertimePreferences = new SelectList(overtimePreferences, "OvertimePreferenceId", "Name"),
                OvertimeSummary = overtimeSummary,
                CanApprovedOvertime = canApproveOvertime,
                Permissions = permissions   
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OvertimeViewModel overtimeViewModel)
        {
            var overtime = overtimeViewModel.Overtime;
            if (ModelState.IsValid)
            {
                overtime.UpdatedBy = ApplicationUser.UserName;
                overtime.UpdatedDateUtc = DateTime.UtcNow;

                HRBusinessService.UpdateOvertime(UserOrganisationId, overtimeViewModel.Overtime);
                return RedirectToAction("profile", "personnel", new { id = overtimeViewModel.PersonnelId });
            }
            var overtimePreferences = HRBusinessService.RetrieveOvertimePreferences(UserOrganisationId);
            var overtimeSummary = HRBusinessService.RetrieveOvertimeSummary(UserOrganisationId, overtimeViewModel.PersonnelId);
            var viewModel = new OvertimeViewModel
            {
                Overtime = overtime,
                OvertimePreferences = new SelectList(overtimePreferences, "OvertimePreferenceId", "Name"),
                OvertimeSummary = overtimeSummary
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult List(int? personnelId, Paging paging, List<OrderBy> orderBy)
        {
            try
            {
                if (personnelId == null)
                    personnelId = UserPersonnelId;
                return this.JsonNet(HRBusinessService.RetrieveOvertimes(UserOrganisationId, personnelId.Value, orderBy, paging));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost]
        public ActionResult ListOvertimeForApprovals(int? personnelId, Paging paging, List<OrderBy> orderBy)
        {
            try
            {
                if (personnelId == null)
                    personnelId = UserPersonnelId;

                var isAdmin = User.IsInAnyRoles("Admin");
                return this.JsonNet(HRBusinessService.RetrieveOvertimeForApprovals(UserOrganisationId, ApplicationUser.Id, isAdmin, orderBy, paging));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost]
        public ActionResult ListOvertimeSummaries(IEnumerable<int> companyIds, IEnumerable<int> departmentIds, IEnumerable<int> teamIds, List<OrderBy> orderBy, Paging paging)
        {
            try
            {
                return this.JsonNet(HRBusinessService.RetrieveOvertimeSummaries(UserOrganisationId, companyIds, departmentIds, teamIds, orderBy, paging));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteOvertime(UserOrganisationId, id);
            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult CanApproveOvertime(int personnelId, int id)
        {
            try
            {
                var organisationId = UserOrganisationId;
                var isAdmin = User.IsInAnyRoles("Admin");
                return this.JsonNet(HRBusinessService.CanApproveOvertime(organisationId, id, isAdmin, ApplicationUser.Id));
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }
    }
}