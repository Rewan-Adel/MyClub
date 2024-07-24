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
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Sigunp",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
           name: "ChangePassword",
           url: "{controller}/{action}/{id}",
           defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
       );



            routes.MapRoute(
               name: "Home",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "Service",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Service", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
             name: "Member",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "Member", action = "Index", id = UrlParameter.Optional }
         );

            routes.MapRoute(
           name: "Offer",
           url: "{controller}/{action}/{id}",
           defaults: new { controller = "MemberOffer", action = "Index", id = UrlParameter.Optional }
       );
        }

       
    }
}
