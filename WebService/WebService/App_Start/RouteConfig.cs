using System.Web.Mvc;
using System.Web.Routing;

namespace WebService
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "commands", action = "Index" }
            );
            routes.MapRoute("commands", "commands/{deviceId}/{timeout}", new {controller = "commands", action = "commands"});
        }
    }
}