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
    public class JobTitleController : BaseController
    {
        public JobTitleController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: JobTitle
        [HttpGet]
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: JobTitle/Create
        [HttpGet]
        public ActionResult Create()
        {
            var organisationId = UserOrganisationId;
            var viewModel = new JobTitleViewModel
            {
                JobTitle = new JobTitle
                {
                    OrganisationId = organisationId
                }
            };
            return View(viewModel);
        }

        // POST: JobTitle/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobTitleViewModel JobTitleViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreateJobTitle(UserOrganisationId, JobTitleViewModel.JobTitle);
                if (result.Succeeded)
                {
                    return RedirectToAction("Edit",new { id = result.Entity.JobTitleId }); 
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new JobTitleViewModel
            {
                JobTitle = JobTitleViewModel.JobTitle
            };
            return View(viewModel);
        }
        
        // GET: JobTitle/Edit/{id}
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();
            //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            var JobTitle = HRBusinessService.RetrieveJobTitle(UserOrganisationId, id.Value);
            if (JobTitle == null)
                return HttpNotFound();

            var viewModel = new JobTitleViewModel
            {
                JobTitle = JobTitle

            };
            return View(viewModel);
        }

        // POST: JobTitle/Edit/{id}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JobTitleViewModel JobTitleViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.UpdateJobTitle(UserOrganisationId, JobTitleViewModel.JobTitle);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new JobTitleViewModel
            {
                JobTitle = JobTitleViewModel.JobTitle
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveJobTitle(UserOrganisationId, orderBy, paging));
        }

        [HttpPost]
        public ActionResult UnassignedJobGrades(int jobtitleId)
        {
            return this.JsonNet(HRBusinessService.RetrieveUnassignedJobGrades(UserOrganisationId, jobtitleId));
        }

        [HttpPost]
        public ActionResult assignJobTitleJobGrade(int jobTitleId, int jobGradeId)
        {
            return this.JsonNet(HRBusinessService.CreateJobTitleJobGrade (UserOrganisationId, jobTitleId, jobGradeId));
        }

        [HttpPost]
        public ActionResult JobTitleJobGrades(int jobTitleId)
        {
            return this.JsonNet(HRBusinessService.RetrieveJobTitleJobGrade(UserOrganisationId, jobTitleId));
        }

        [HttpPost]
        public ActionResult UnassignJobTitleJobGrade(int jobTitleId, int jobGradeId)
        {
            HRBusinessService.DeleteJobTitleJobGrade(UserOrganisationId, jobTitleId, jobGradeId);
            return this.JsonNet("");
        }


        [HttpPost]
        public ActionResult CanDeleteJobTitle(int id)
        {
            try
            {
                return this.JsonNet(HRBusinessService.CanDeleteJobTitle(UserOrganisationId, id));
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }
        
        [HttpPost]
        public ActionResult CanDeleteJobTitleJobGrade(int jobTitleId,int  jobGradeId)
        {
            try
            {
                return this.JsonNet(HRBusinessService.CanDeleteJobTitleJobGrade(UserOrganisationId, jobTitleId, jobGradeId));
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteJobTitle(UserOrganisationId, id);
            return RedirectToAction("Index");
        }

    }
}