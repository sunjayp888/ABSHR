using HR.Business.Interfaces;
using HR.Entity;
using HR.Extensions;
using HR.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HR.Controllers
{
    [Authorize]
    public class ApprovalLevelUserController : BaseController
    {
        public ApprovalLevelUserController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        [HttpPost]
        public ActionResult Create(ApprovalLevelUser approvalLevelUser)
        {
            var result = HRBusinessService.CreateApprovalLevelUser(UserOrganisationId, approvalLevelUser);
            List<string> errorList = new List<string>();
            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return this.JsonNet(errorList);
        }

        [HttpPost]
        public ActionResult List(int id)
        {
            return this.JsonNet(HRBusinessService.RetrieveApprovalUsers(UserOrganisationId, id));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteApprovalLevelUser(UserOrganisationId, id);
            return this.JsonNet("");
        }

        [HttpPost]
        public ActionResult ListAvailableApprovalUsers(int id)
        {
            try
            {
                return this.JsonNet(HRBusinessService.RetrieveAvailableApprovalUsers(UserOrganisationId, id));
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
            
        }

    }
}