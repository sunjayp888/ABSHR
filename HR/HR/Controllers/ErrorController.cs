using HR.Business.Interfaces;
using HR.Models;
using System.Web.Mvc;

namespace HR.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }
    }
}