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
    [Authorize]
    public class SiteController : BaseController
    {
        private static string SiteName { get; set; }
        public SiteController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: Site
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: Site/Create
        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new SiteViewModel
            {
                Site = new Site
                {
                    OrganisationId = UserOrganisationId
                },
                Countries = new SelectList(HRBusinessService.RetrieveCountries(UserOrganisationId, null, null).Items, "CountryId", "Name")
            };
            return View(viewModel);
        }

        // POST: Site/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SiteId,Name,CountryId")] Site site)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreateSite(UserOrganisationId, site);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new SiteViewModel
            {
                Countries = new SelectList(HRBusinessService.RetrieveCountries(UserOrganisationId, null, null).Items, "CountryId", "Name"),
                Site = site
            };
            return View(viewModel);
        }


        // GET: Site/Edit/{id}
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var site = HRBusinessService.RetrieveSite(UserOrganisationId, id.Value);
            if (site == null)
            {
                return HttpNotFound();
            }
            SiteName = site.Name;
            var viewModel = new SiteViewModel
            {
                Countries = new SelectList(HRBusinessService.RetrieveCountries(UserOrganisationId, null, null).Items, "CountryId", "Name", site.CountryId),
                Site = site
            };
            return View(viewModel);
        }

        // POST: Site/Edit/{id}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SiteId,Name,CountryId,OrganisationId")] Site site)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.UpdateSite(UserOrganisationId, site);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new SiteViewModel
            {
                Countries = new SelectList(HRBusinessService.RetrieveCountries(UserOrganisationId, null, null).Items, "CountryId", "Name", site.CountryId),
                Site = site
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CanDeleteSite(int id)
        {
            return this.JsonNet(HRBusinessService.CanDeleteSite(UserOrganisationId, id));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteSite(UserOrganisationId, id);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveSites(UserOrganisationId, orderBy, paging));
        }
    }
}