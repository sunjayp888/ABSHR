using HR.Constraints;
using HR.Interfaces;
using System.Web.Mvc;
using System.Web.Routing;

namespace HR.Extensions
{
    public static class RouteExtensions
    {
        public static void MapRouteWithTenantConstraint(this RouteCollection routes, string name, string url, object defaults)
        {
            routes.MapRoute(
                name,
                url,
                defaults,
                new { TenantAccess = new TenantRouteConstraint(DependencyResolver.Current.GetService<ITenantsService>()) }
            );
        }
    }
}