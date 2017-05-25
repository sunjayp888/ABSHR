using HR.Business.Interfaces;
using HR.Entity;
using HR.Entity.Dto;
using HR.Extensions;
using HR.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace HR.Controllers
{
    public class CompanyController : BaseController
    {
        public CompanyController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: Company
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            var colours = HRBusinessService.RetrieveColours().ToList();
            var viewModel = new CompanyViewModel
            {
                Company = new Company
                {
                    OrganisationId = UserOrganisationId,
                    ColourId = colours.FirstOrDefault()?.ColourId ?? 0
                },
                ColoursList = colours
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CompanyId,Name,ColourId")] Company company)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreateCompany(UserOrganisationId, company);

                if (result.Succeeded)
                    return RedirectToAction("Index");

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            var viewModel = new CompanyViewModel
            {
                Company = company,
                ColoursList = HRBusinessService.RetrieveColours().ToList()
            };
            return View(viewModel);
        }

        // GET: Company/Edit/{id}
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var company = HRBusinessService.RetrieveCompany(UserOrganisationId, id.Value);
            if (company == null)
            {
                return HttpNotFound();
            }
            var viewModel = new CompanyViewModel
            {
                Company = company,
                ColoursList = HRBusinessService.RetrieveColours().ToList()
            };
            return View(viewModel);
        }

        // POST: Company/Edit/{id}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompanyId,Name,ColourId")] Company company)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.UpdateCompany(UserOrganisationId, company);
                if(result.Succeeded)
                return RedirectToAction("Index");
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new CompanyViewModel
            {
                Company = company,
                ColoursList = HRBusinessService.RetrieveColours().ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveCompanies(UserOrganisationId, orderBy, paging));
        }

        [HttpPost]
        public ActionResult CanDeleteCompany(int id)
        {
            return this.JsonNet(HRBusinessService.CanDeleteCompany(UserOrganisationId, id));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteCompany(UserOrganisationId, id);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult CompanyBuilding(int companyId)
        {
            return this.JsonNet(HRBusinessService.RetrieveCompanyBuilding(UserOrganisationId, companyId));
        }
        [HttpPost]
        public ActionResult UnassignedSiteBuilding(int companyId)
        {
            var result =
                HRBusinessService.RetrieveBuildingsSitesUnassignedCompany(UserOrganisationId, companyId).ToList();
            return this.JsonNet(result);
        }
        [HttpPost]
        public ActionResult AssignCompanyBuilding(int companyId, int buildingId)
        {
            var result = HRBusinessService.CreateCompanyBuilding(UserOrganisationId, companyId, buildingId);
            return this.JsonNet(result);
        }
        [HttpPost]
        public ActionResult CanDeleteCompanyBuilding(int companyId,int buildingId)
        {
            var result = HRBusinessService.CanDeleteCompanyBuilding(UserOrganisationId, companyId,buildingId);
            return this.JsonNet(result);
        }
        [HttpPost]
        public ActionResult UnassignSiteBuilding(int companybuildingId)
        {
             HRBusinessService.DeleteCompanyBuilding(UserOrganisationId, companybuildingId);
            return this.JsonNet("");
        }
    }
}