using HR.Business.Interfaces;
using HR.Entity;
using HR.Entity.Dto;
using HR.Extensions;
using HR.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;

namespace HR.Controllers
{
    public class DepartmentController : BaseController
    {
        private static string DepartmentName { get; set; }
        public DepartmentController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: Department
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            var colours = HRBusinessService.RetrieveColours().ToList();
            var viewModel = new DepartmentViewModel
            {
                Department = new Department
                {
                    OrganisationId = UserOrganisationId,
                    ColourId = colours.FirstOrDefault()?.ColourId ?? 0
                },
                ColoursList = colours
            };
            return View(viewModel);
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentId,Name,ColourId")] Department department)
        {
            if (ModelState.IsValid)
            {
                
                var result = HRBusinessService.CreateDepartment(UserOrganisationId, department);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new DepartmentViewModel
            {
                Department = department,
                ColoursList = HRBusinessService.RetrieveColours().ToList()
            };
            return View(viewModel);
        }

        // GET: Department/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var department = HRBusinessService.RetrieveDepartment(UserOrganisationId, id.Value);
            if (department == null)
            {
                return HttpNotFound();
            }
            DepartmentName = department.Name;
            var viewModel = new DepartmentViewModel
            {
                Department = department,
                ColoursList = HRBusinessService.RetrieveColours().ToList()
            };
            return View(viewModel);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentId,Name,ColourId")] Department department)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.UpdateDepartment(UserOrganisationId, department);
                if (result.Succeeded)
                    return RedirectToAction("Index");

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

            }
            var viewModel = new DepartmentViewModel
            {
                Department = department,
                ColoursList = HRBusinessService.RetrieveColours().ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CanDeleteDepartment(int id)
        {
            return this.JsonNet(HRBusinessService.CanDeleteDepartment(UserOrganisationId, id));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteDepartment(UserOrganisationId, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveDepartments(UserOrganisationId, orderBy, paging));
        }

        [HttpPost]
        public ActionResult ListTeamFilters()
        {
            return this.JsonNet(HRBusinessService.RetrieveDepartmentFilters(UserOrganisationId));
        }
    }
}
