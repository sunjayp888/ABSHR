using HR.Business.Interfaces;
using HR.Entity;
using HR.Entity.Dto;
using HR.Extensions;
using HR.Models;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace HR.Controllers
{
    [Authorize]
    public class BuildingController : BaseController
    {
        public BuildingController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: Building
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: Building/Create
        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new BuildingViewModel
            {
                Building = new Building
                {
                    OrganisationId = UserOrganisationId
                },
                Sites = new SelectList(HRBusinessService.RetrieveSites(UserOrganisationId), "SiteId", "Name")
            };
            return View(viewModel);
        }

        // POST: Building/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BuildingId,Address1,SiteId,Name")] Building building)
        {
            if (ModelState.IsValid)
            {
                building = HRBusinessService.CreateBuilding(UserOrganisationId, building);
                return RedirectToAction("Index");
            }
            var viewModel = new BuildingViewModel
            {
                Sites = new SelectList(HRBusinessService.RetrieveSites(UserOrganisationId), "SiteId", "Name"),
                Building = building
            };
            return View(viewModel);
        }

        // GET: Building/Edit/{id}
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var building = HRBusinessService.RetrieveBuilding(UserOrganisationId, id.Value);
            if (building == null)
            {
                return HttpNotFound();
            }
            var viewModel = new BuildingViewModel
            {
                Sites = new SelectList(HRBusinessService.RetrieveSites(UserOrganisationId), "SiteId", "Name", building.SiteId),
                Building = building
            };
            return View(viewModel);
        }

        // POST: Building/Edit/{id}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BuildingId,Address1,SiteId,Name")] Building building)
        {
            if (ModelState.IsValid)
            {
                HRBusinessService.UpdateBuilding(UserOrganisationId, building);
                return RedirectToAction("Index");
            }
            var viewModel = new BuildingViewModel
            {
                Sites = new SelectList(HRBusinessService.RetrieveSites(UserOrganisationId), "SiteId", "Name", building.SiteId),
                Building = building
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CanDeleteBuilding(int id)
        {
            return this.JsonNet(HRBusinessService.CanDeleteBuilding(UserOrganisationId, id));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteBuilding(UserOrganisationId, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveBuildings(UserOrganisationId, orderBy, paging));
        }
       

    }
}