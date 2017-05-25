using HR.Controllers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HR
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
        }

        protected void Application_EndRequest()
        {
            //this handles custom error returned
            int statusCode = Context.Response.StatusCode;
            if (statusCode == 404 || statusCode == 500 || statusCode == 400)
            {
                HttpContext.Current.Response.Redirect("~/Error/Index");
            }
        }
    }
}
