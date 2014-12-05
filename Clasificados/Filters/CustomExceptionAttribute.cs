using System.Web.Mvc;
using log4net;

namespace Clasificados.Filters
{
    public class CustomExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CustomExceptionAttribute));
        void IExceptionFilter.OnException(ExceptionContext filterContext)
        {
            Log.Error("Unhandeled Exception", filterContext.Exception);
        }
    }
}