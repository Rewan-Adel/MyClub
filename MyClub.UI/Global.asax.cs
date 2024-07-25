using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMatrix.WebData;

namespace MyClub.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection(
                    connectionStringName: "MyclubSecurity",
                    userTableName: "User_Profile",
                    userIdColumn: "UserId",
                    userNameColumn: "UserName",
                    autoCreateTables: true);

               
            }

            AreaRegistration.RegisterAllAreas();
            FilterConfig.AccountGlobalFilters(GlobalFilters.Filters);
            RouteConfig.AccountRoutes(RouteTable.Routes);
            BundleConfig.AccountBundles(BundleTable.Bundles);
        }
    }
}
