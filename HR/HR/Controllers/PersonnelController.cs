using HR.Attributes;
using HR.Authorization.Models;
using HR.Business.Interfaces;
using HR.Entity;
using HR.Entity.Dto;
using HR.Extensions;
using HR.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace HR.Controllers
{
    [Authorize]
    public class PersonnelController : BaseController
    {
        private DateTime _defaultDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public PersonnelController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        // GET: Personnel
        [AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult Index()
        {
            return View(new BaseViewModel());
        }

        // GET: Personnel/Profile/{id}
        [AuthorizePersonnel(Roles = "Admin,User")]
        public new ActionResult Profile(int id)
        {
            var organisationId = UserOrganisationId;
            var personnelId = UserPersonnelId;
            var personnel = HRBusinessService.RetrievePersonnel(organisationId, id);
            if (personnel == null)
            {
                return HttpNotFound();
            }

            var isAdmin = User.IsInAnyRoles("Admin");
            var firstOrDefault = personnel.Employments.FirstOrDefault();
            if (firstOrDefault != null && (firstOrDefault.TerminationDate.HasValue  && firstOrDefault.TerminationDate<=_defaultDate))
                ModelState.AddModelError("TerminatedError", "Employee terminated  , please contact your HR administrator.");
            if (personnel.CurrentAbsenceTypeEntitlements == null || !personnel.CurrentAbsenceTypeEntitlements.Any())
                ModelState.AddModelError("EntitlementError", "Absence entitlements may not been correctly configured, please contact your HR administrator.");

            var viewModel = new PersonnelProfileViewModel
            {
                Personnel = personnel,
                Permissions = HRBusinessService.RetrievePersonnelPermissions(isAdmin, organisationId, UserPersonnelId, id)
            };
            return View(viewModel);
        }

        // GET: Personnel/Photo/{id}
        [AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult Photo(int id)
        {
            byte[] image;
            string bytes = null;
            try
            {
                bytes = HRBusinessService.RetrievePhoto(UserOrganisationId, id);
                if (string.IsNullOrEmpty(bytes))
                {
                    return File("~/Content/images/user.png", "image/png");
                }
                image = Convert.FromBase64String(bytes);
                return File(image, "image/png");
            }
            catch (Exception)
            {
                return File("~/Content/images/user.png", "image/png");
            }
        }

        // GET: Personnel/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var isAdmin = User.IsInAnyRoles("Admin");

            var organisationId = UserOrganisationId;
            var countries = HRBusinessService.RetrieveCountries(organisationId, null, null).Items;
            var reportsTo = HRBusinessService.RetrieveReportsToPersonnel(organisationId, 0);
            var employmentTypes = HRBusinessService.RetrieveEmploymentTypes(organisationId);
            var absencePolices = HRBusinessService.RetrieveAbsencePolices(organisationId);
            var publicHolidayPolices = HRBusinessService.RetrievePublicHolidayPolices(organisationId);
            var companyBuilding = HRBusinessService.RetrieveEmploymentCompanyBuilding(organisationId);
            var defaultSelected = HRBusinessService.RetrieveEmploymentCompanyBuilding(organisationId).FirstOrDefault();
            var jobTitleJobgrades = HRBusinessService.RetrieveJobTitleJobGrade(organisationId).ToList();
            var defaultSelectedJobTitleJobgrades = jobTitleJobgrades.FirstOrDefault();
            var approvalEntityTypeAssignments = HRBusinessService.ApprovalEntityTypeAssignments(UserOrganisationId, 0);
            var approvalModels = HRBusinessService.RetrieveApprovalModels(UserOrganisationId);
            var permissions = HRBusinessService.RetrievePersonnelPermissions(isAdmin, UserOrganisationId, UserPersonnelId, 0);
            if (defaultSelected == null)
            {

                ModelState.AddModelError("EmploymentError", "Assign Site Building to Company");
            }
            if (defaultSelectedJobTitleJobgrades == null)
            {

                ModelState.AddModelError("EmploymentError", "Assign JobTitle to JobGrade");
            }
            var viewModel = new PersonnelProfileViewModel
            {
                Countries = new SelectList(countries, "CountryId", "Name"),
                Personnel = new Personnel
                {
                    OrganisationId = organisationId,
                    DOB = DateTime.Today,
                    //Title = "Mr",
                    //Forenames = "A",
                    //Surname = "B",
                    //Email = string.Format("{0}@hr.com", Guid.NewGuid()),
                    //Address1 = "Address1",
                    //Postcode = "POST CODE",
                    //Telephone = "12345678",
                    //NINumber = "NZ1234567",
                },
                EmploymentViewModel = new EmploymentViewModel
                {
                    ReportsTo = new SelectList(reportsTo, "PersonnelId", "Fullname"),
                    AbsencePolicy = new SelectList(absencePolices, "AbsencePolicyId", "Name"),
                    PublicHolidayPolicy = new SelectList(publicHolidayPolices, "PublicHolidayPolicyId", "Name"),
                    Employment = new Employment
                    {
                        OrganisationId = UserOrganisationId,
                        StartDate = DateTime.Today,
                        CompanyId = defaultSelected?.Company.CompanyId ?? 0,
                        BuildingId = defaultSelected?.Building.BuildingId ?? 0,
                        JobGradeId = defaultSelectedJobTitleJobgrades?.JobGradeId ?? 0,
                        JobTitleId = defaultSelectedJobTitleJobgrades?.JobTitleId ?? 0

                    },
                    EmploymentType = new SelectList(employmentTypes, "EmploymentTypeId", "Name"),
                    EmploymentCompanyBuildinglist = companyBuilding,
                    SelectedBuildingId = defaultSelected?.Company.CompanyId.ToString() ?? string.Empty,
                    JobTitleJobGrades = jobTitleJobgrades,
                    SelectedCompanyId = defaultSelected?.Building.BuildingId.ToString() ?? string.Empty,
                    SelectedJobTitleId = defaultSelectedJobTitleJobgrades?.JobTitle.JobTitleId.ToString() ?? string.Empty,
                    SelectedJobGradeId = defaultSelectedJobTitleJobgrades?.JobGrade.JobGradeId.ToString() ?? string.Empty,
                    SelectedDepartmentIds = new List<int> { }, // HERE AS AN EXAMPLE OF PRE SELECTED DATA (ie when editing)
                    SelectedTeamIds = new List<int> { } // HERE AS AN EXAMPLE OF PRE SELECTED DATA (ie when editing)
                },
                ApprovalEntityTypeAssignments = approvalEntityTypeAssignments,
                ApprovalModels = approvalModels,
                Permissions = permissions
            };
            return View(viewModel);
        }

        // POST: Personnel/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PersonnelProfileViewModel personnelViewModel)
        {
            try
            {
                var organisationId = UserOrganisationId;
                // check if user with this email already exists for the current organisation
                var userExists = UserManager.FindByEmail(personnelViewModel.Personnel.Email);
                if (userExists != null)
                    ModelState.AddModelError("", string.Format("An account already exists for the email address {0}", personnelViewModel.Personnel.Email));

                if (ModelState.IsValid)
                {
                    personnelViewModel.Personnel = HRBusinessService.CreatePersonnel(organisationId, personnelViewModel.Personnel, personnelViewModel.EmploymentViewModel.Employment, personnelViewModel.EmploymentViewModel.WorkingPatternDays.ToList(), personnelViewModel.EmploymentViewModel.SelectedDepartmentIds, personnelViewModel.EmploymentViewModel.SelectedTeamIds);
                    var result = CreateUserAndRole(personnelViewModel.Personnel);
                    if (personnelViewModel.ApprovalEntityTypeAssignments != null)
                    foreach (var approvalEntityTypeAssignment in personnelViewModel.ApprovalEntityTypeAssignments)
                        {
                            PersonnelApprovalModel personnelApprovalModel = new PersonnelApprovalModel
                            {
                            PersonnelId = personnelViewModel.Personnel.PersonnelId,
                            ApprovalEntityTypeId = approvalEntityTypeAssignment.ApprovalEntityId,
                            ApprovalModelId = approvalEntityTypeAssignment.ApprovalModelId,
                            PersonnelApprovalModelId = approvalEntityTypeAssignment.PersonnelApprovalModelId
                            };
                            HRBusinessService.CreatePersonnelApprovalModel(UserOrganisationId, personnelApprovalModel);
                        }
                    if (result.Succeeded)
                        return RedirectToAction("Index");

                    //delete the orphaned personnel & employment records
                    HRBusinessService.DeletePersonnel(organisationId, personnelViewModel.Personnel.PersonnelId);
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                var jobTitleJobgrades = HRBusinessService.RetrieveJobTitleJobGrade(organisationId);
                var defaultSelectedJobTitleJobgrades = jobTitleJobgrades.FirstOrDefault();
                var countries = HRBusinessService.RetrieveCountries(organisationId, null, null).Items;
                var reportsTo = HRBusinessService.RetrieveReportsToPersonnel(organisationId, 0);
                var absencePolices = HRBusinessService.RetrieveAbsencePolices(organisationId);
                var publicHolidayPolices = HRBusinessService.RetrievePublicHolidayPolices(organisationId);
                var companyBuilding = HRBusinessService.RetrieveEmploymentCompanyBuilding(organisationId);
                var employmentTypes = HRBusinessService.RetrieveEmploymentTypes(organisationId);
                var approvalEntityTypeAssignments = HRBusinessService.ApprovalEntityTypeAssignments(UserOrganisationId, 0);
                var approvalModels = HRBusinessService.RetrieveApprovalModels(UserOrganisationId);
                var viewModel = new PersonnelProfileViewModel
                {
                    Countries = new SelectList(countries, "CountryId", "Name"),
                    Personnel = personnelViewModel.Personnel,
                    EmploymentViewModel = new EmploymentViewModel
                    {
                        ReportsTo = new SelectList(reportsTo, "PersonnelId", "Fullname", personnelViewModel.EmploymentViewModel.Employment.ReportsToPersonnelId),
                        EmploymentType = new SelectList(employmentTypes, "EmploymentTypeId", "Name", personnelViewModel.EmploymentViewModel.Employment.EmploymentTypeId),
                        AbsencePolicy = new SelectList(absencePolices, "AbsencePolicyId", "Name"),
                        PublicHolidayPolicy = new SelectList(publicHolidayPolices, "PublicHolidayPolicyId", "Name"),
                        JobTitleJobGrades = jobTitleJobgrades,
                        Employment = personnelViewModel.EmploymentViewModel.Employment,
                        WorkingPatternDays = personnelViewModel.EmploymentViewModel.WorkingPatternDays,
                        EmploymentCompanyBuildinglist = companyBuilding,
                        SelectedDepartmentIds = personnelViewModel.EmploymentViewModel.SelectedDepartmentIds,
                        SelectedTeamIds = personnelViewModel.EmploymentViewModel.SelectedTeamIds,
                        SelectedBuildingId = personnelViewModel.EmploymentViewModel.Employment.BuildingId.ToString(),
                        SelectedCompanyId = personnelViewModel.EmploymentViewModel.Employment.CompanyId.ToString(),
                        SelectedJobTitleId = defaultSelectedJobTitleJobgrades.JobTitle.JobTitleId.ToString(),
                        SelectedJobGradeId = defaultSelectedJobTitleJobgrades.JobGrade.JobGradeId.ToString()
                    },
                    ApprovalEntityTypeAssignments = approvalEntityTypeAssignments,
                    ApprovalModels = approvalModels
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }
        }

        private IdentityResult CreateUserAndRole(Personnel personnel)
        {
            var createUser = new ApplicationUser
            {
                UserName = personnel.Email,
                Email = personnel.Email,
                OrganisationId = UserOrganisationId,
                PersonnelId = personnel.PersonnelId
            };

            var roleId = RoleManager.Roles.FirstOrDefault(r => r.Name == "User").Id;
            createUser.Roles.Add(new IdentityUserRole { UserId = createUser.Id, RoleId = roleId });

            var result = UserManager.Create(createUser, "Password1!");
            return result;
        }

        // GET: Personnel/Edit/{id}
        [AuthorizePersonnel(Roles = "Admin,User")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var personnel = HRBusinessService.RetrievePersonnel(UserOrganisationId, id.Value);
            if (personnel == null)
            {
                return HttpNotFound();
            }
            personnel.Email = UserManager.FindByPersonnelId(personnel.PersonnelId)?.Email;
            var isAdmin = User.IsInAnyRoles("Admin");
            var employment = HRBusinessService.RetrieveEmployment(UserOrganisationId, id.Value, _defaultDate);
            var approvalEntityTypeAssignments = HRBusinessService.ApprovalEntityTypeAssignments(UserOrganisationId, id.Value);
            var approvalModels = HRBusinessService.RetrieveApprovalModels(UserOrganisationId);
            var permissions = HRBusinessService.RetrievePersonnelPermissions(isAdmin, UserOrganisationId, UserPersonnelId, id);
            var viewModel = new PersonnelProfileViewModel
            {
                Countries = new SelectList(HRBusinessService.RetrieveCountries(UserOrganisationId, null, null).Items, "CountryId", "Name", personnel.CountryId),
                Personnel = personnel,
                SelectedDepartmentIds = employment?.EmploymentDepartments.Select(d => d.DepartmentId).ToList(),
                SelectedTeamIds = employment?.EmploymentTeams.Select(t => t.TeamId).ToList(),
                ApprovalEntityTypeAssignments = approvalEntityTypeAssignments,
                ApprovalModels = approvalModels,
                Permissions = permissions
            };
            return View(viewModel);
        }

        // POST: Personnels/Edit/{id}
        [AuthorizePersonnel(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PersonnelProfileViewModel personnelViewModel)
        {
            if (ModelState.IsValid)
            {
                personnelViewModel.Personnel = HRBusinessService.UpdatePersonnel(UserOrganisationId, personnelViewModel.Personnel);
                if (personnelViewModel.ApprovalEntityTypeAssignments != null)
                    foreach (var approvalEntityTypeAssignment in personnelViewModel.ApprovalEntityTypeAssignments)
                    {
                        PersonnelApprovalModel personnelApprovalModel = new PersonnelApprovalModel
                        {
                            PersonnelId = personnelViewModel.Personnel.PersonnelId,
                            ApprovalEntityTypeId = approvalEntityTypeAssignment.ApprovalEntityId,
                            ApprovalModelId = approvalEntityTypeAssignment.ApprovalModelId,
                            PersonnelApprovalModelId = approvalEntityTypeAssignment.PersonnelApprovalModelId
                        };
                        if (personnelApprovalModel.PersonnelApprovalModelId == 0)
                        {
                            HRBusinessService.CreatePersonnelApprovalModel(UserOrganisationId, personnelApprovalModel);
                        }
                        else if (personnelApprovalModel.ApprovalModelId == 0)
                        {
                            HRBusinessService.DeletePersonnelApprovalModel(UserOrganisationId, personnelApprovalModel.PersonnelApprovalModelId);
                        }
                        else
                        {
                            HRBusinessService.UpdatePersonnelApprovalModel(UserOrganisationId, personnelApprovalModel);
                        }
                    }
                var editUser = UserManager.FindByPersonnelId(personnelViewModel.Personnel.PersonnelId);
                editUser.Email = personnelViewModel.Personnel.Email;

                var result = UserManager.Update(editUser);
                if (result.Succeeded)
                {
                    if (User.IsInRole("User"))
                        return RedirectToAction("Profile", "Personnel", new { id = personnelViewModel.Personnel.PersonnelId });
                    else
                        return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            var approvalEntityTypeAssignments = HRBusinessService.ApprovalEntityTypeAssignments(UserOrganisationId, personnelViewModel.Personnel.PersonnelId);
            var approvalModels = HRBusinessService.RetrieveApprovalModels(UserOrganisationId);

            var viewModel = new PersonnelProfileViewModel
            {
                Countries = new SelectList(HRBusinessService.RetrieveCountries(UserOrganisationId, null, null).Items, "CountryId", "Name"),
                Personnel = personnelViewModel.Personnel,
                ApprovalEntityTypeAssignments = approvalEntityTypeAssignments,
                ApprovalModels = approvalModels
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UploadPhoto(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {

                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }
                        HRBusinessService.UploadPhoto(UserOrganisationId, id.Value, fileData);
                    }
                }
                return this.JsonNet("");
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }

        }

        [HttpPost]
        public ActionResult DeletePhoto(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                HRBusinessService.DeletePhoto(UserOrganisationId, id.Value);
                return this.JsonNet("");
            }
            catch (Exception ex)
            {
                return this.JsonNet(ex);
            }

        }

        [HttpPost]
        public ActionResult List(Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrievePersonnel(UserOrganisationId, orderBy, paging));
        }

        [HttpPost]
        public ActionResult Search(string searchKeyword, Paging paging, List<OrderBy> orderBy)
        {
            return this.JsonNet(HRBusinessService.RetrievePersonnelBySearchKeyword(UserOrganisationId, searchKeyword, orderBy, paging));
        }

        [HttpPost]
        public ActionResult Listdepartments()
        {
            return this.JsonNet(HRBusinessService.RetrieveDepartments(UserOrganisationId));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}