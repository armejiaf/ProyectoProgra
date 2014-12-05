using System.Web;
using System.Web.Mvc;
using Clasificados.Filters;

namespace Clasificados
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomActionAttribute());
            filters.Add(new CustomAuthorizationAttribute());
            filters.Add(new CustomResultAttribute());
            filters.Add(new CustomExceptionAttribute());
        }
    }
}
