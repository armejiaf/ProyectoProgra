using System.Configuration;

namespace Data
{
    public class ConnectionStrings
    {
        public static string Get()
        {
            var environment = (ConfigurationManager.AppSettings["Environment"] ?? "").ToLower();
            var connectionStringToUse = string.Empty;

            switch (environment)
            {
                case "remote":
                    connectionStringToUse = ConfigurationManager.ConnectionStrings["remote"].ToString();
                    break;
                case "local":
                    connectionStringToUse=ConfigurationManager.ConnectionStrings["local"].ToString();
                    break;
            }

            return connectionStringToUse;
        }
    }
}