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
    public class TeamController : BaseController
    {
        public TeamController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: Team
        [HttpGet]
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: Team/Create
        [HttpGet]
        public ActionResult Create()
        {
            var organisationId = UserOrganisationId;
            var colours = HRBusinessService.RetrieveColours().ToList();
            var viewModel = new TeamViewModel
            {
                Team = new Team
                {
                    OrganisationId = organisationId,
                    ColourId = colours.FirstOrDefault()?.ColourId ?? 0
                },
                ColoursList = colours
            };
            return View(viewModel);
        }

        // POST: Team/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeamViewModel teamViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreateTeam(UserOrganisationId, teamViewModel.Team);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new TeamViewModel
            {
                Team = teamViewModel.Team,
                ColoursList = HRBusinessService.RetrieveColours().ToList()
            };
            return View(viewModel);
        }
        
        // GET: Team/Edit/{id}
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var team = HRBusinessService.RetrieveTeam(UserOrganisationId, id.Value);
            if (team == null)
                return HttpNotFound();

            var viewModel = new TeamViewModel
            {
                Team = team,
                ColoursList = HRBusinessService.RetrieveColours().ToList()

            };
            return View(viewModel);
        }

        // POST: Team/Edit/{id}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TeamViewModel teamViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.UpdateTeam(UserOrganisationId, teamViewModel.Team);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new TeamViewModel
            {
                Team = teamViewModel.Team,
                ColoursList = HRBusinessService.RetrieveColours().ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveTeam(UserOrganisationId, orderBy, paging));
        }

        [HttpPost]
        public ActionResult ListTeamFilters()
        {
            return this.JsonNet(HRBusinessService.RetrieveTeamFilters(UserOrganisationId));
        }

        [HttpPost]
        public ActionResult CanDeleteTeam(int id)
        {
            try
            {
                return this.JsonNet(HRBusinessService.CanDeleteTeam(UserOrganisationId, id));
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteTeam(UserOrganisationId, id);
            return RedirectToAction("Index");
        }

    }
}