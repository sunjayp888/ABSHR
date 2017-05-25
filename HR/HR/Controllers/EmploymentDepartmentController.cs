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
    public class EmploymentDepartmentController : BaseController
    {
        public EmploymentDepartmentController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        [HttpPost]
        public ActionResult Create(EmploymentDepartment employmentDepartment)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreateEmploymentDepartment(UserOrganisationId, employmentDepartment);
                if (result.Succeeded)
                {
                    return this.JsonNet(result);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return this.JsonNet(employmentDepartment);
        }

        [HttpPost]
        public ActionResult List(int employmentId)
        {
            return this.JsonNet(HRBusinessService.RetrieveEmploymentDepartments(UserOrganisationId, employmentId));
        }

        [HttpPost]
        public ActionResult Delete(int employmentId, int departmentId)
        {
            HRBusinessService.DeleteEmploymentDepartment(UserOrganisationId, employmentId, departmentId);
            return this.JsonNet("");
        }
    }
}