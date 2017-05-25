using HR.Business.Interfaces;
using HR.Entity;
using HR.Models;
using System.Net;
using System.Web.Mvc;
using HR.Extensions;

namespace HR.Controllers
{
    public class EmergencyContactController : BaseController
    {
        public EmergencyContactController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }
        // GET: EmergencyContacts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var emergencyContact = HRBusinessService.RetrieveEmergencyContact(UserOrganisationId, id.Value);
            if (emergencyContact == null)
            {
                return HttpNotFound();
            }
            var viewModel = new EmergencyContactViewModel
            {
                EmergencyContact = emergencyContact
            };
            return View(viewModel);
        }

        // GET: EmergencyContacts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int id)
        {
            var viewModel = new EmergencyContactViewModel
            {
                Countries = new SelectList(HRBusinessService.RetrieveCountries(UserOrganisationId, null, null).Items, "CountryId", "Name"),

                EmergencyContact = new EmergencyContact
                {
                    PersonnelId = id,
                    OrganisationId = UserOrganisationId

                }

            };
            return View(viewModel);

        }

        // POST: EmergencyContacts/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmergencyContact emergencyContact)
        {
            if (ModelState.IsValid)
            {
                emergencyContact = HRBusinessService.CreateEmergencyContact(UserOrganisationId, emergencyContact);
                return RedirectToAction("Profile", "personnel", new { id = emergencyContact.PersonnelId });
            }
            var viewModel = new EmergencyContactViewModel
            {
                Countries = new SelectList(HRBusinessService.RetrieveCountries(UserOrganisationId, null, null).Items, "CountryId", "Name"),
                EmergencyContact = emergencyContact
            };
            return View(viewModel);
        }

        // GET: EmergencyContacts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var emergencyContact = HRBusinessService.RetrieveEmergencyContact(UserOrganisationId, id.Value);
            if (emergencyContact == null)
            {
                return HttpNotFound();
            }
            var viewModel = new EmergencyContactViewModel
            {
                Countries = new SelectList(HRBusinessService.RetrieveCountries(UserOrganisationId, null, null).Items, "CountryId", "Name"),
                EmergencyContact = emergencyContact
            };
            return View(viewModel);
        }

        // POST: EmergencyContacts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmergencyContact emergencyContact)
        {
            if (ModelState.IsValid)
            {
                emergencyContact = HRBusinessService.UpdateEmergencyContact(UserOrganisationId, emergencyContact);
                return RedirectToAction("Profile", "Personnel", new { id = emergencyContact.PersonnelId });
            }
            return View(emergencyContact);
        }

        // GET: EmergencyContacts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var emergencyContact = HRBusinessService.RetrieveEmergencyContact(UserOrganisationId, id.Value);
            if (emergencyContact == null)
            {
                return HttpNotFound();
            }
            var viewModel = new EmergencyContactViewModel
            {
                EmergencyContact = emergencyContact
            };
            return View(viewModel);

        }

        // POST: EmergencyContacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(EmergencyContactViewModel model)
        {

            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HRBusinessService.DeleteEmergencyContact(UserOrganisationId, model.EmergencyContact.EmergencyContactId);
            return RedirectToAction("Profile", "Personnel", new { id = model.EmergencyContact.PersonnelId });
        }
        [HttpPost]
        public ActionResult List(int personnelId)
        {
            return this.JsonNet(HRBusinessService.RetrieveEmergencyContactsbyPersonnelId(UserOrganisationId, personnelId));
        }

    }
}
