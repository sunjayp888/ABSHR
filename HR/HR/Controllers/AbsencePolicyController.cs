using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using HR.Business;
using HR.Business.Interfaces;
using HR.Business.Models;
using HR.Entity;
using HR.Entity.Dto;
using HR.Extensions;
using HR.Models;

namespace HR.Controllers
{
    public class AbsencePolicyController : BaseController
    {

        public AbsencePolicyController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: AbsencePolicy
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveAbsencePolicies(UserOrganisationId, orderBy, paging));
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new AbsencePolicyViewModel() { AbsencePolicy = new AbsencePolicy() { OrganisationId = UserOrganisationId } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AbsencePolicyId,Name")] AbsencePolicy absencePolicy)
        {
            if (ModelState.IsValid)
            {
                absencePolicy.OrganisationId = UserOrganisationId;
                var result = HRBusinessService.CreateAbsencePolicy(UserOrganisationId, absencePolicy);
                if (result.Succeeded)
                {
                    return RedirectToAction("Edit", new { id = result.Entity.AbsencePolicyId });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new AbsencePolicyViewModel
            {
                AbsencePolicy = new AbsencePolicy()
            };
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var organisationId = UserOrganisationId;
            var absencePolicy = HRBusinessService.RetrieveAbsencePolicy(organisationId, id.Value);

            var workingPatternDays = absencePolicy.WorkingPatternId == null ? HRBusinessService.RetrieveDefaultWorkingPatternDays() :
                                      absencePolicy.WorkingPattern.WorkingPatternDays;
            var absenceTypes = HRBusinessService.RetrieveAbsenceTypes(organisationId, null, null);
            var absencePeriod = HRBusinessService.RetrieveAbsencePeriods(organisationId, null, null);

            var viewModel = new AbsencePolicyViewModel
            {
                Frequencies = new SelectList(EnumHelper.GetSelectList(typeof(Frequency.FrequencyType)), "Value", "Text", Frequency.FrequencyType.Yearly),
                AbsencePolicy = absencePolicy,
                SumOfDuration= (double) (workingPatternDays!=null? workingPatternDays.Sum(s => s.Duration):0),
                WorkingPatternDays = workingPatternDays.ToList(),
                AbsenceTypes = absenceTypes.Items.ToList(),
                AbsencePeriods = absencePeriod.Items.ToList(),
                AbsencePolicyEntitlement = new AbsencePolicyEntitlement()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(int? id, AbsencePolicyViewModel absencePolicyViewModel)
        {
            var organisationId = UserOrganisationId;
            var absencePolicy =
                HRBusinessService.RetrieveAbsencePolicies(organisationId, null, null)
                    .Items.FirstOrDefault(e => e.AbsencePolicyId == id);

        
            ValidationResult<WorkingPattern> result = null;
            absencePolicyViewModel.AbsencePolicy.OrganisationId = organisationId;
            if (absencePolicy.WorkingPatternId == null)
                result = HRBusinessService.CreateAbsencePolicyWorkingPattern(organisationId, absencePolicyViewModel.AbsencePolicy, absencePolicyViewModel.WorkingPatternDays);
            else
             
            result = HRBusinessService.UpdateAbsencePolicy(organisationId, absencePolicyViewModel.AbsencePolicy, absencePolicyViewModel.WorkingPatternDays);

            if (result.Succeeded)
                return RedirectToAction("Index");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            var viewModel = new AbsencePolicyViewModel
            {
                Frequencies = new SelectList(EnumHelper.GetSelectList(typeof(Frequency.FrequencyType)), "Value", "Text", Frequency.FrequencyType.Yearly),
                AbsencePolicy = absencePolicy,
                WorkingPatternDays = absencePolicyViewModel.WorkingPatternDays.ToList(),
                AbsenceTypes = absencePolicyViewModel.AbsenceTypes,
                AbsencePeriods = absencePolicyViewModel.AbsencePeriods,
                SumOfDuration = absencePolicyViewModel.SumOfDuration,
                AbsencePolicyEntitlement = new AbsencePolicyEntitlement()

            };
            return View(viewModel);
        }

        public ActionResult UnassignedAbsencePolicyAbsenceTypes(int absencePolicyId)
        {
            return this.JsonNet(HRBusinessService.RetrieveUnassignedAbsencePolicyAbsenceTypes(UserOrganisationId, absencePolicyId));
        }

        public ActionResult AbsencePolicyEntitlements(int absencePolicyId)
        {
            return this.JsonNet(HRBusinessService.RetrieveAbsencePolicyEntitlements(UserOrganisationId, absencePolicyId));
        }

        public ActionResult AssignAbsencePolicyAbsenceType(int absencePolicyId, int absenceTypeId)
        {
            return this.JsonNet(HRBusinessService.CreateAbsencePolicyEntitlement(UserOrganisationId, absencePolicyId, absenceTypeId));
        }

        [HttpPost]
        public PartialViewResult AbsencePolicyEntitlement(int absencePolicyId, int absencePolicyEntitlementId)
        {
            var organisationId = UserOrganisationId;
            var absencePolicyEntitlement =
                HRBusinessService.RetrieveAbsencePolicyEntitlements(organisationId, absencePolicyId)
                    .Items.FirstOrDefault(e => e.AbsencePolicyEntitlementId == absencePolicyEntitlementId);

            var model = new AbsencePolicyViewModel()
            {
                Frequencies = new SelectList(EnumHelper.GetSelectList(typeof(Frequency.FrequencyType)), "Value", "Text", Frequency.FrequencyType.Yearly),
                AbsencePolicyEntitlement = absencePolicyEntitlement
            };
            return PartialView("_AbsencePolicyEntitlement", model);
        }

        [HttpPost]
        public ActionResult UpdateAbsencePolicyEntitlement(AbsencePolicyViewModel absencePolicyViewModel)
        {
            if (ModelState.IsValid)
            {
                var organisationId = UserOrganisationId;
                HRBusinessService.UpdateAbsencePolicyEntitlement(organisationId, absencePolicyViewModel.AbsencePolicyEntitlement);
                return this.JsonNet(string.Empty);
            }
            var errors = ModelState.Values.Where(e => e.Errors.Count > 0)
                .Select(e => e.Errors.Select(d => d.ErrorMessage).FirstOrDefault())
                .Distinct();

            return this.JsonNet(errors);
        }

        [HttpPost]
        public ActionResult IsAbsencesAssignedToAbsencePolicyAbsenceType(int absencePolicyId, int absenceTypeId)
        {
            return this.JsonNet(HRBusinessService.IsAbsencesAssignedToAbsencePolicyAbsenceType(UserOrganisationId, absencePolicyId, absenceTypeId));
        }

        [HttpPost]
        public ActionResult UnassignAbsencePolicyAbsenceType(int absencePolicyId, int absenceTypeId)
        {
            HRBusinessService.DeleteAbsencePolicyAbsenceType(UserOrganisationId, absencePolicyId, absenceTypeId);
            return this.JsonNet("");
        }

        [HttpPost]
        public ActionResult Clone(int absencePolicyId)
        {
           var result=  HRBusinessService.CloneAbsencePolicy(UserOrganisationId, absencePolicyId);
            return this.JsonNet(result);
        }

        [HttpPost]
        public ActionResult CanDeleteAbsencePolicy(int absencePolicyId)
        {
            var result = HRBusinessService.CanDeleteAbsencePolicy(UserOrganisationId, absencePolicyId);
            return this.JsonNet(result);
        }

        [HttpPost]
        public ActionResult Delete(int absencePolicyId)
        {
            HRBusinessService.DeleteAbsencePolicy(UserOrganisationId, absencePolicyId);
            return this.JsonNet("");
        }

        [HttpPost]
        public ActionResult AbsencePeriods(int absencePolicyId, Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveAbsencePolicyAbsencePeriods(UserOrganisationId, absencePolicyId, orderBy, paging));
        }

        [HttpPost]
        public ActionResult AbsencePeriodsByPersonnel(int personnelId, Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveAbsencePolicyAbsencePeriodsByPersonnel(UserOrganisationId, personnelId));
        }

        [HttpPost]
        public ActionResult UnassignedAbsencePeriods(int absencePolicyId)
        {
            return this.JsonNet(HRBusinessService.RetrieveUnassignedAbsencePolicyPeriods(UserOrganisationId, absencePolicyId));
        }

        public ActionResult CreateAbsencePolicyPeriod(AbsencePolicyPeriod absencePolicyPeriod)
        {
            return this.JsonNet(HRBusinessService.CreateAbsencePolicyAbsencePeriod(UserOrganisationId, absencePolicyPeriod));
        }

        public ActionResult IsAbsencesAssignedToAbsencePolicyPeriod(int absencePolicyPeriodId)
        {
            return this.JsonNet(HRBusinessService.IsAbsencesAssignedToAbsencePolicyPeriod(UserOrganisationId, absencePolicyPeriodId));
        }

        public ActionResult UnassignAbsencePeriod(int absencePolicyPeriodId)
        {
            HRBusinessService.DeleteAbsencePolicyPeriod(UserOrganisationId, absencePolicyPeriodId);
            return this.JsonNet("");
        }
    }
}