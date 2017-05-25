using HR.Business.Interfaces;
using HR.Entity.Dto;
using HR.Extensions;
using HR.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HR.Controllers
{
    [Authorize]
    public class OvertimeTransactionController : BaseController
    {
        public OvertimeTransactionController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }
        
        [HttpGet]
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }
        
        [HttpPost]
        public ActionResult List(OvertimeFilter overtimeFilter, Paging paging, List<OrderBy> orderBy)
        {
            try
            {
                return this.JsonNet(HRBusinessService.RetrieveOvertimes(UserOrganisationId, overtimeFilter, orderBy, paging));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
    }
}