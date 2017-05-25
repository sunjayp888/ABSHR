using HR.Business.Interfaces;
using HR.Entity;
using HR.Entity.Dto;
using HR.Extensions;
using HR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace HR.Controllers
{
    [Authorize]
    public class AbsenceTypeController : BaseController
    {
        private static string AbsenceName { get; set; }
        public AbsenceTypeController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: AbsenceType
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: AbsenceType/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var viewModel = new AbsenceTypeViewModel
            {
                AbsenceType = new AbsenceType()
                {
                    OrganisationId = UserOrganisationId,
                },
                ColoursList = HRBusinessService.RetrieveColours().ToList(),
               
            };
            var firstOrDefault = viewModel.ColoursList.FirstOrDefault();
            if (firstOrDefault != null)
                viewModel.AbsenceType.ColourId = firstOrDefault.ColourId;
            return View(viewModel);
        }

        // POST: AbsenceType/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //OrganisationId,Name,ReduceEntitlement
        public ActionResult Create([Bind(Include = "AbsenceTypeId,Name,ColourId")] AbsenceType absenceType)
        {
            if (ModelState.IsValid)
            {
                //Create AbsenceType               
                var result = HRBusinessService.CreateAbsenceType(UserOrganisationId, absenceType);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)    
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(new AbsenceTypeViewModel {AbsenceType = absenceType,
                ColoursList = HRBusinessService.RetrieveColours().ToList()
            });
        }

        // GET: AbsenceType/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var absenceType = HRBusinessService.RetrieveAbsenceType(UserOrganisationId, id.Value);
            AbsenceName = absenceType.Name;
            var viewmodel = new AbsenceTypeViewModel()
            {
                AbsenceType = absenceType,
                ColoursList = HRBusinessService.RetrieveColours().ToList()
            };

            return View(viewmodel);
        }

        // POST: AbsenceType/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AbsenceTypeId,Name,ColourId")] AbsenceType absenceType)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.UpdateAbsenceType(UserOrganisationId, absenceType);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

            }
            return View(new AbsenceTypeViewModel { AbsenceType = absenceType,
                ColoursList = HRBusinessService.RetrieveColours().ToList()
            });
        }

        // POST: AbsenceType/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteAbsenceType(UserOrganisationId, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveAbsenceTypes(UserOrganisationId, orderBy, paging));
        }

        [HttpPost]
        public ActionResult CanDeleteAbsenceType(int id)
        {
            return this.JsonNet(HRBusinessService.CanDeleteAbsenceType(UserOrganisationId, id));
        }
    }
}
