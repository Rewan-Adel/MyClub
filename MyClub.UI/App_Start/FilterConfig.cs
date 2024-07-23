using System.Web;
using System.Web.Mvc;

namespace MyClub.UI
{
    public class FilterConfig
    {
        public static void AccountGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
