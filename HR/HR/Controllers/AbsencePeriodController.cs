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
    public class AbsencePeriodController : BaseController
    {

        public AbsencePeriodController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: AbsencePeriod
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: AbsencePeriod/Create
        [HttpGet]
        public ActionResult Create()
        {
            var defaultDate = DateTime.Today.Date;
            var viewModel = new AbsencePeriodViewModel { 
                AbsencePeriod = new AbsencePeriod {
                                    StartDate = defaultDate,
                                    EndDate = defaultDate.AddYears(1).AddDays(-1),
                                    OrganisationId = UserOrganisationId
                }
            };
            return View(viewModel);
        }

        // POST: AbsencePeriod/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "AbsencePeriodId,StartDate,EndDate")] AbsencePeriod absencePeriod)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreateAbsencePeriod(UserOrganisationId, absencePeriod);
                if (result.Succeeded)
                    return RedirectToAction("Index");

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new AbsencePeriodViewModel
            {
                AbsencePeriod = absencePeriod
            };
            return View(viewModel);
        }

        // GET: AbsencePeriod/Edit/{id}
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var absencePeriod = HRBusinessService.RetrieveAbsencePeriod(UserOrganisationId, id.Value);
            if (absencePeriod == null)
            {
                return HttpNotFound();
            }
            var viewModel = new AbsencePeriodViewModel
            {
                AbsencePeriod = absencePeriod
            };
            return View(viewModel);
        }

        // POST: AbsencePeriod/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AbsencePeriodId,StartDate,EndDate")] AbsencePeriod absencePeriod)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.UpdateAbsencePeriod(UserOrganisationId, absencePeriod);
                if (result.Succeeded)
                    return RedirectToAction("Index");

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new AbsencePeriodViewModel
            {
                AbsencePeriod = absencePeriod
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CanDeleteAbsencePeriod(int id)
        {
            return this.JsonNet(HRBusinessService.CanDeleteAbsencePeriod(UserOrganisationId, id));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteAbsencePeriod(UserOrganisationId, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveAbsencePeriods(UserOrganisationId, orderBy, paging));
        }
    }
}