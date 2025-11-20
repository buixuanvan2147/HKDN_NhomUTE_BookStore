using System.Web;
using System.Web.Mvc;

namespace BCCK_CSharp_BookShop
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
