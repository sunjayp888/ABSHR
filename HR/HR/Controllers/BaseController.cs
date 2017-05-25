using HR.Authorization.Models;
using HR.Business.Interfaces;
using HR.Entity.Dto;
using HR.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;

namespace HR.Controllers
{
    public class BaseController : Controller
    {
        private IHRBusinessService _hrBusinessService;
        private ApplicationUserManager _userManager;
        private ApplicationUser _applicationUser;

        protected IHRBusinessService HRBusinessService
        {
            get
            {
                return _hrBusinessService;
            }
        }
        
        public BaseController(IHRBusinessService hrBusinessService)
        {
            _hrBusinessService = hrBusinessService;
        }

       

        protected ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        protected ApplicationUser ApplicationUser
        {
            get
            {
                return _applicationUser ?? UserManager.FindById(User?.Identity?.GetUserId());
            }
            set
            {
                _applicationUser = value;
            }
        }

        protected TenantOrganisation Organisation => UserManager.TenantOrganisation;


        protected int UserOrganisationId => ApplicationUser?.OrganisationId ?? 0;
        protected int UserPersonnelId => ApplicationUser?.PersonnelId ?? 0;        

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewModel = filterContext.Controller.ViewData.Model as BaseViewModel;

            if (viewModel != null)
            {
                var organisation = UserManager.TenantOrganisation;
                viewModel.OrganisationName = organisation?.Name ?? string.Empty;
                viewModel.PersonnelId = UserPersonnelId;                                
            }

            base.OnActionExecuted(filterContext);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_hrBusinessService != null)
                    _hrBusinessService = null;

                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
                if (_applicationUser != null)
                    _applicationUser = null;

            }

            base.Dispose(disposing);
        }
        
        protected ActionResult NotFoundError()
        {
            return RedirectToAction("Index", "Error");
        }
    }
}