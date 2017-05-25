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
    public class OrganisationalChartController : BaseController
    {

        public OrganisationalChartController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }
        

        [HttpPost]
        public ActionResult List(PersonnelFilter personnelFilter, int showColourBy)
        {
            try
            {
                var organisationId = UserOrganisationId;
                var personnelId = UserPersonnelId;
                var permissions = HRBusinessService.RetrievePersonnelPermissions(User.IsInRole("Admin"), organisationId, personnelId);
                return this.JsonNet(HRBusinessService.RetrieveOrganisationalChart(UserOrganisationId, personnelId, permissions, personnelFilter, showColourBy));
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }

        public ActionResult OrganisationalChartViewModel()
        {
            try
            {
                var organisationId = UserOrganisationId;
                var personnelId = UserPersonnelId;
                var permissions = HRBusinessService.RetrievePersonnelPermissions(User.IsInRole("Admin"), organisationId, personnelId);
                var viewmodel = new OrganisationalChartViewModel
                {
                    PersonnelDetailFilter = HRBusinessService.RetrievePersonnelDetailFilters(UserOrganisationId, personnelId, permissions.IsAdmin)
                };
                return this.JsonNet(viewmodel);
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }

    }
}