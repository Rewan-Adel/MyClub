using System.Web.Mvc;
using System.Web.Routing;

namespace MyClub.UI
{
    public class RouteConfig
    {
        public static void AccountRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Sigunp",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "Login",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
           name: "ChangePassword",
           url: "{controller}/{action}/{id}",
           defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
       );
        }

        public static void ServiceRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "services",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "sercices", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
