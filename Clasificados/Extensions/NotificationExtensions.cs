using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clasificados.Extensions
{
    /*                                                                      *
     *      This extension was derive from Brad Christie's answer           *
     *      on StackOverflow.                                               *
     *                                                                      *
     *      The original code can be found at:                              *
     *      http://stackoverflow.com/a/18338264/998328                      *
     *                                                                      */

    public static class NotificationExtensions
    {
        private static readonly IDictionary<String, String> NotificationKey = new Dictionary<String, String>
        {
            { "Error",      "App.Notifications.Error" }, 
            { "Warning",    "App.Notifications.Warning" },
            { "Success",    "App.Notifications.Success" },
            { "Info",       "App.Notifications.Info" }
        };


        public static void AddNotification(this ControllerBase controller, String message, String notificationType)
        {
            var notificationKe = GetNotificationKeyByType(notificationType);
            var messages = controller.TempData[notificationKe] as ICollection<String>;

            if (messages == null)
            {
                controller.TempData[notificationKe] = (messages = new HashSet<String>());
            }

            messages.Add(message);
        }

        public static IEnumerable<String> GetNotifications(this HtmlHelper htmlHelper, String notificationType)
        {
            string notificationKe = GetNotificationKeyByType(notificationType);
            return htmlHelper.ViewContext.Controller.TempData[notificationKe] as ICollection<String> ?? null;
        }

        private static string GetNotificationKeyByType(string notificationType)
        {
            try
            {
                return NotificationKey[notificationType];
            }
            catch (IndexOutOfRangeException e)
            {
                var exception = new ArgumentException("Key is invalid", "notificationType", e);
                throw exception;
            }
        }
    }

    public static class NotificationType
    {
        public const string Error = "Error";
        public const string Warning = "Warning";
        public const string Success = "Success";
        public const string Info = "Info";

    }
}