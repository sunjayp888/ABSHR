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
    public class ApprovalModelController : BaseController
    {
        public ApprovalModelController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: ApprovalModel
        [HttpGet]
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: ApprovalModel/Create
        [HttpGet]
        public ActionResult Create()
        {
            var organisationId = UserOrganisationId;
            var viewModel = new ApprovalModelViewModel
            {
                ApprovalModel = new ApprovalModel
                {
                    OrganisationId = organisationId
                }
            };
            return View(viewModel);
        }

        // POST: ApprovalModel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApprovalModelViewModel approvalModelViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreateApprovalModel(UserOrganisationId, approvalModelViewModel.ApprovalModel);
                if (result.Succeeded)
                {
                    return RedirectToAction("Edit", new { id = result.Entity.ApprovalModelId });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new ApprovalModelViewModel
            {
                ApprovalModel = approvalModelViewModel.ApprovalModel
            };
            return View(viewModel);
        }
        
        // GET: ApprovalModel/Edit/{id}
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var approvalModel = HRBusinessService.RetrieveApprovalModel(UserOrganisationId, id.Value);
            if (approvalModel == null)
                return HttpNotFound();

            var viewModel = new ApprovalModelViewModel
            {
                ApprovalModel = approvalModel

            };
            return View(viewModel);
        }

        // POST: ApprovalModel/Edit/{id}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApprovalModelViewModel approvalModelViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.UpdateApprovalModel(UserOrganisationId, approvalModelViewModel.ApprovalModel);
                if (result.Succeeded)
                    return RedirectToAction("Edit");
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new ApprovalModelViewModel
            {
                ApprovalModel = approvalModelViewModel.ApprovalModel
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveApprovalModels(UserOrganisationId, orderBy, paging));
        }

        [HttpPost]
        public ActionResult ListAll()
        {
            return this.JsonNet(HRBusinessService.RetrieveApprovalModels(UserOrganisationId));
        }

        [HttpPost]
        public ActionResult CanDeleteApprovalModel(int id)
        {
            try
            {
                return this.JsonNet(HRBusinessService.CanDeleteApprovalModel(UserOrganisationId, id));
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                HRBusinessService.DeleteApprovalModel(UserOrganisationId, id);
                return this.JsonNet("");
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }

    }
}