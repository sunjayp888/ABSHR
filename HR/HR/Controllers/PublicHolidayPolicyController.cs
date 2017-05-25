using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HR.Business.Interfaces;
using HR.Entity;
using HR.Entity.Dto;
using HR.Extensions;
using HR.Models;

namespace HR.Controllers
{
    public class PublicHolidayPolicyController : BaseController
    {
        public PublicHolidayPolicyController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new PublicHolidayPolicyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PublicHolidayPolicy publicHolidayPolicy)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreatePublicHolidayPolicy(UserOrganisationId, publicHolidayPolicy);
                if (result.Succeeded)
                {
                    return RedirectToAction("Edit", new { id = publicHolidayPolicy.PublicHolidayPolicyId });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new PublicHolidayPolicyViewModel
            {
                PublicHolidayPolicy = publicHolidayPolicy
            };
            return View(viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var publicHolidayPolicy = HRBusinessService.RetrievePublicHolidayPolicy(UserOrganisationId, id.Value);
            if (publicHolidayPolicy == null)
            {
                return HttpNotFound();
            }
            var viewModel = new PublicHolidayPolicyViewModel
            {
                PublicHolidayPolicy = publicHolidayPolicy
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PublicHolidayPolicy publicHolidayPolicy)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.UpdatePublicHolidayPolicy(UserOrganisationId, publicHolidayPolicy);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new PublicHolidayPolicyViewModel
            {
                PublicHolidayPolicy = publicHolidayPolicy
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CanDeletePublicHolidayPolicy(int id)
        {
            return this.JsonNet(HRBusinessService.CanDeletePublicHolidayPolicy(UserOrganisationId, id));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeletePublicHolidayPolicy(UserOrganisationId, id);
            return this.JsonNet("");
        }

        [HttpPost]
        public ActionResult RetrievePublicHolidays(int publicHolidayPolicyId,int year, List<OrderBy> orderBy, Paging paging)
        {
            return this.JsonNet(HRBusinessService.RetrievePublicHolidays(UserOrganisationId, publicHolidayPolicyId, year, orderBy, paging));
        }

        [HttpPost]
        public ActionResult CreatePublicHoliday(PublicHoliday publicHoliday)
        {
            if (ModelState.IsValid)
            {
                publicHoliday.OrganisationId = UserOrganisationId;
                var result = HRBusinessService.CreatePublicHoliday(UserOrganisationId, publicHoliday);
                if (result.Succeeded)
                {
                    return this.JsonNet(string.Empty);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
           return this.JsonNet(
                ModelState.Values.Where(e => e.Errors.Count > 0)
                    .Select(e => e.Errors.Select(d => d.ErrorMessage).FirstOrDefault())
                    .Distinct());
        }

        [HttpPost]
        public ActionResult UpdatePublicHoliday(PublicHoliday publicHoliday)
        {
            if (ModelState.IsValid)
            {
                publicHoliday.OrganisationId = UserOrganisationId;
                var result = HRBusinessService.UpdatePublicHoliday(UserOrganisationId, publicHoliday);
                if (result.Succeeded)
                {
                    return this.JsonNet(string.Empty);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return this.JsonNet(
                ModelState.Values.Where(e => e.Errors.Count > 0)
                    .Select(e => e.Errors.Select(d => d.ErrorMessage).FirstOrDefault())
                    .Distinct());
        }

        [HttpPost]
        public ActionResult DeletePublicHoliday(int publicHolidayId)
        {
            HRBusinessService.DeletePublicHoliday(UserOrganisationId, publicHolidayId);
            return this.JsonNet("");
        }
       
        public ActionResult GetYear(int publicHolidayPolicyId)
        {
            return this.JsonNet(HRBusinessService.RetrievePublicHolidayYear(UserOrganisationId, publicHolidayPolicyId));
        }

        [HttpPost]
        public ActionResult ClonePublicHolidayPolicy(int publicHolidayPolicyId)
        {
            var result = HRBusinessService.ClonePublicHolidayPolicy(UserOrganisationId, publicHolidayPolicyId);
            return this.JsonNet(result);
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrievePublicHolidayPolicies(UserOrganisationId, orderBy, paging));
        }
    }
}