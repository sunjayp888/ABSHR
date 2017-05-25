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
    public class EmploymentTeamController : BaseController
    {
        public EmploymentTeamController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        [HttpPost]
        public ActionResult Create(EmploymentTeam employmentTeam)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreateEmploymentTeam(UserOrganisationId, employmentTeam);
                if (result.Succeeded)
                {
                    return this.JsonNet(result);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return this.JsonNet(employmentTeam);
        }

        [HttpPost]
        public ActionResult List(int employmentId)
        {
            return this.JsonNet(HRBusinessService.RetrieveEmploymentTeams(UserOrganisationId, employmentId));
        }

        [HttpPost]
        public ActionResult Delete(int employmentId, int teamId)
        {
            HRBusinessService.DeleteEmploymentTeam(UserOrganisationId, employmentId, teamId);
            return this.JsonNet("");
        }

    }
}