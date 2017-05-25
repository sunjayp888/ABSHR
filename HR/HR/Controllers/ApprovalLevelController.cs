using HR.Business.Interfaces;
using HR.Extensions;
using System.Web.Mvc;

namespace HR.Controllers
{
    [Authorize]
    public class ApprovalLevelController : BaseController
    {
        public ApprovalLevelController(IHRBusinessService hrBusinessService) : base(hrBusinessService)
        {
        }

        [HttpPost]
        public ActionResult List(int id)
        {
            return this.JsonNet(HRBusinessService.RetrieveApprovalLevels(UserOrganisationId, id));
        }

    }
}