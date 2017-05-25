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
    [Authorize]
    public class CountryController : BaseController
    {
        public CountryController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: Country
        public ActionResult Index()
        {
            return View(new CountryViewModel());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new CountryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CountryId, Name")] Country country)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.CreateCountry(UserOrganisationId, country);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            var viewModel = new CountryViewModel
            {
                Country = country
            };
            return View(viewModel);
        }

        // GET: Country/Edit/{id}
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var country = HRBusinessService.RetrieveCountry(UserOrganisationId, id.Value);
            if (country == null)
            {
                return HttpNotFound();
            }
            var viewModel = new CountryViewModel
            {
                Country = country
            };
            return View(viewModel);
        }


        // POST: Country/Edit/{id}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude = "Country.Name")]CountryViewModel countryViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = HRBusinessService.UpdateCountry(UserOrganisationId, countryViewModel.Country);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return View(countryViewModel);
        }

        [HttpPost]
        public ActionResult CanDeleteCountry(int id)
        {
            return this.JsonNet(HRBusinessService.CanDeleteCountry(UserOrganisationId, id));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            HRBusinessService.DeleteCountry(UserOrganisationId, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrieveCountries(UserOrganisationId, orderBy, paging));
        }

    }
}