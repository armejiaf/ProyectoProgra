using System.Web.Mvc;
using log4net;

namespace Clasificados.Filters
{
    public class CustomResultAttribute : ActionFilterAttribute, IResultFilter
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        void IResultFilter.OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().ToString());

            // Recogemos el resultado
            var result = filterContext.Result;
            Log.Debug("ActionResult: " + result.ToString());
        }

        void IResultFilter.OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().ToString());

            // Recogemos el resultado
            var result = filterContext.Result;
            Log.Debug("ActionResult: " + result.ToString());
        }
    }
}