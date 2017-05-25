using HR.Business.Interfaces;
using HR.Extensions;
using HR.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace HR.Controllers
{
    [Authorize]
    public class PersonnelAbsenceEntitlementController : BaseController
    {
        public PersonnelAbsenceEntitlementController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        [HttpGet]
        public ActionResult Edit(int personnelId, int? personnelAbsenceEntitlementId)
        {
            if (personnelAbsenceEntitlementId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var personnelAbsenceEntitlement = HRBusinessService.RetrievePersonnelAbsenceEntitlement(UserOrganisationId, personnelId, personnelAbsenceEntitlementId.Value);
            if (personnelAbsenceEntitlement == null)
            {
                return HttpNotFound();
            }
            var viewModel = new PersonnelAbsenceEntitlementViewModel
            {
                PersonnelAbsenceEntitlement = personnelAbsenceEntitlement
            };
            return View(viewModel);
        }

        // POST: Building/Edit/{id}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonnelAbsenceEntitlementViewModel personnelAbsenceEntitlementViewModel)
        {
            var organisationId = UserOrganisationId;
            var personnelAbsenceEntitlement = HRBusinessService.RetrievePersonnelAbsenceEntitlement(organisationId, personnelAbsenceEntitlementViewModel.PersonnelAbsenceEntitlement.PersonnelId, personnelAbsenceEntitlementViewModel.PersonnelAbsenceEntitlement.PersonnelAbsenceEntitlementId);

            if (ModelState.IsValid)
            {                
                personnelAbsenceEntitlement.Entitlement = personnelAbsenceEntitlementViewModel.PersonnelAbsenceEntitlement.Entitlement;
                personnelAbsenceEntitlement.CarriedOver = personnelAbsenceEntitlementViewModel.PersonnelAbsenceEntitlement.CarriedOver;
                personnelAbsenceEntitlement.Used = personnelAbsenceEntitlementViewModel.PersonnelAbsenceEntitlement.Used;
                personnelAbsenceEntitlement.Remaining = personnelAbsenceEntitlementViewModel.PersonnelAbsenceEntitlement.Remaining;
                personnelAbsenceEntitlement.MaximumCarryForward = personnelAbsenceEntitlementViewModel.PersonnelAbsenceEntitlement.MaximumCarryForward;

                 var result = HRBusinessService.UpdatePersonnelAbsenceEntitlement(organisationId, personnelAbsenceEntitlement);
                if (result.Succeeded)
                    return RedirectToAction("Profile", "Personnel", new { id = personnelAbsenceEntitlementViewModel.PersonnelAbsenceEntitlement.PersonnelId });

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new PersonnelAbsenceEntitlementViewModel
            {
                PersonnelAbsenceEntitlement = personnelAbsenceEntitlement
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult List(int personnelId, int absencePeriodId)
        {
            return this.JsonNet(HRBusinessService.RetrievePersonnelAbsenceEntitlements(UserOrganisationId, personnelId, absencePeriodId).Where(a => a.AbsenceTypeId != null));
        }

    }
}