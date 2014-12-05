using System.Diagnostics;
using System.Text;
using System.Web.Mvc;

namespace Clasificados.Filters
{
    
    public class CustomActionAttribute : FilterAttribute, IActionFilter
    {
        #region Logging
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string StopwatchKey = "DebugLoggingStopWatch";
        #endregion
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Log.IsDebugEnabled)
            {
                var loggingWatch = Stopwatch.StartNew();
                filterContext.HttpContext.Items.Add(StopwatchKey, loggingWatch);

                var message = new StringBuilder();
                message.Append(string.Format("Executing controller {0}, action {1}", 
                    filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, 
                    filterContext.ActionDescriptor.ActionName));

                Log.Debug(message);
            }
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Log.IsDebugEnabled && filterContext.HttpContext.Items[StopwatchKey] != null)
            {
                var loggingWatch = (Stopwatch) filterContext.HttpContext.Items[StopwatchKey];
                loggingWatch.Stop();

                var timeSpent = loggingWatch.ElapsedMilliseconds;

                var message = new StringBuilder();
                message.Append(string.Format("Finished executing controller {0}, action {1} - time spent {2}",
                    filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    filterContext.ActionDescriptor.ActionName,
                    timeSpent));

                Log.Debug(message);
                filterContext.HttpContext.Items.Remove(StopwatchKey);
            }
        }
    }
}