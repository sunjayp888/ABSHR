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
    public class JobGradeController : BaseController
    {
        public JobGradeController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: JobGrade
        [HttpGet]
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: JobGrade/Create
        [HttpGet]
        public ActionResult Create()
        {
            var organisationId = UserOrganisationId;
            var viewModel = new JobGradeViewModel
            {
                JobGrade = new JobGrade
                {
                    OrganisationId = organisationId
                }
            };
            return View(viewModel);
        }

        // POST: JobGrade/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobGradeViewModel jobGradeViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreateJobGrade(UserOrganisationId, jobGradeViewModel.JobGrade);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new JobGradeViewModel
            {
                JobGrade = jobGradeViewModel.JobGrade
            };
            return View(viewModel);
        }
        
        // GET: JobGrade/Edit/{id}
        [HttpGet]
        public ActionResult Edit(int? id, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var jobGrade = HRBusinessService.RetrieveJobGrade(UserOrganisationId, id.Value);
            if (jobGrade == null)
                return HttpNotFound();

            var viewModel = new JobGradeViewModel
            {
                JobGrade = jobGrade

            };
            return View(viewModel);
        }

        // POST: JobGrade/Edit/{id}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JobGradeViewModel jobGradeViewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.UpdateJobGrade(UserOrganisationId, jobGradeViewModel.JobGrade);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new JobGradeViewModel
            {
                JobGrade = jobGradeViewModel.JobGrade
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveJobGrade(UserOrganisationId, orderBy, paging));
        }

        [HttpPost]
        public ActionResult CanDeleteJobGrade(int id)
        {
            try
            {
                return this.JsonNet(HRBusinessService.CanDeleteJobGrade(UserOrganisationId, id));
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteJobGrade(UserOrganisationId, id);
            return RedirectToAction("Index");
        }

    }
}