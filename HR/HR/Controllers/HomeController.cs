using HR.Business.Interfaces;
using HR.Entity;
using HR.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HR.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        public ActionResult Index()
        {
            var organisationId = UserOrganisationId;
            var personnelId = UserPersonnelId;
            var permissions = HRBusinessService.RetrievePersonnelPermissions(User.IsInRole("Admin"), organisationId, personnelId);

            if (User.IsInRole("User") && !permissions.IsManager)
                return RedirectToAction("Profile", "Personnel", new { id = personnelId });


            var personnelDetailFilter = HRBusinessService.RetrievePersonnelDetailFilters(UserOrganisationId, personnelId, permissions.IsAdmin);
            var managerPersonnel = HRBusinessService.RetrievePersonnelChildrenPersonnel(organisationId, personnelId);

            //var absencesRequiringApproval = (managerPersonnel != null) ? HRBusinessService.RetrieveManagerAbsencesRequiringApproval(organisationId, managerPersonnel?.Select(personnel => personnel.PersonnelId).ToList()) : null;

            var absencesRequiringApproval = HRBusinessService.RetrieveAbsenceForApprovals(organisationId, ApplicationUser.Id, User.IsInRole("Admin"), null, null);
            var overtimeRequiringApproval = HRBusinessService.RetrieveOvertimeForApprovals(organisationId, ApplicationUser.Id, User.IsInRole("Admin"), null, null);
            var viewModel = new HomeViewModel
            {
                Permissions = permissions,
                AbsencesRequiringApproval = absencesRequiringApproval?.Items.Count() ?? 0,
                OvertimeRequiringApproval = overtimeRequiringApproval?.Items.Count() ?? 0,
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View(new BaseViewModel());
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View(new BaseViewModel());
        }
    }
}