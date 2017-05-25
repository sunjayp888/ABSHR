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
    public class EmploymentTypeController : BaseController
    {
        public EmploymentTypeController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: EmploymentType
        [HttpGet]
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: EmploymentType/Create
        [HttpGet]
        public ActionResult Create()
        {
            var organisationId = UserOrganisationId;
            var viewModel = new EmploymentTypeViewModel
            {
                EmploymentType = new EmploymentType
                {
                    OrganisationId = organisationId
                }
            };
            return View(viewModel);
        }

        // POST: EmploymentType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmploymentTypeViewModel employmentTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreateEmploymentType(UserOrganisationId, employmentTypeViewModel.EmploymentType);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new EmploymentTypeViewModel
            {
                EmploymentType = employmentTypeViewModel.EmploymentType
            };
            return View(viewModel);
        }
        
        // GET: EmploymentType/Edit/{id}
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var employmentType = HRBusinessService.RetrieveEmploymentType(UserOrganisationId, id.Value);
            if (employmentType == null)
                return HttpNotFound();

            var viewModel = new EmploymentTypeViewModel
            {
                EmploymentType = employmentType

            };
            return View(viewModel);
        }

        // POST: EmploymentType/Edit/{id}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmploymentTypeViewModel employmentTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.UpdateEmploymentType(UserOrganisationId, employmentTypeViewModel.EmploymentType);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new EmploymentTypeViewModel
            {
                EmploymentType = employmentTypeViewModel.EmploymentType
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveEmploymentType(UserOrganisationId, orderBy, paging));
        }

        [HttpPost]
        public ActionResult CanDeleteEmploymentType(int id)
        {
            try
            {
                return this.JsonNet(HRBusinessService.CanDeleteEmploymentType(UserOrganisationId, id));
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteEmploymentType(UserOrganisationId, id);
            return RedirectToAction("Index");
        }

    }
}