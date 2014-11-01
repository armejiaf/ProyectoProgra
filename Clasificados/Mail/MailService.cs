using RestSharp;

namespace Clasificados.Mail
{
    public class MailService
    {
        public static void SendGreetingMessage(string correo,string nombre,string password)
        {

            var client = new RestClient
            {
                BaseUrl = "https://api.mailgun.net/v2",
                Authenticator =
                    new HttpBasicAuthenticator("api",
                    "key-f32fda75e27073a696d42c8ed5c999d2")
            };

            var request = new RestRequest();
            request.AddParameter("domain",
                                "app030ec7dc101b40c48a43d1e02635ebc0.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Clasificados <mejia561@hotmail.com>");
            var email="<"+correo+">";
            request.AddParameter("to", email);
            request.AddParameter("subject", "Bienvenido");
            var message = "Bienvenido a Clasificados " + nombre +
                          " para ingresar a nuestra pagina ve a (http://proyectoprogra.apphb.com/Home/Login)";
            request.AddParameter("html", "<html>" + message + "<BR><BR>Email: " + email + "<BR>Password: " +password);
            request.Method = Method.POST;
            client.Execute(request);
        }

        public static void SendRestorePassMessage(string correo,string nombre,string password)
        {
            var client = new RestClient
            {
                BaseUrl = "https://api.mailgun.net/v2",
                Authenticator =
                    new HttpBasicAuthenticator("api",
                    "key-f32fda75e27073a696d42c8ed5c999d2")
            };

            var request = new RestRequest();
            request.AddParameter("domain",
            "app030ec7dc101b40c48a43d1e02635ebc0.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Clasificados <mejia561@hotmail.com>");
            var email = "<" + correo + ">";
            request.AddParameter("to", email);
            request.AddParameter("subject", "Nueva Contraseña");
            var message = " " + nombre + " puede ingresar nuevamente a nuestra pagina con esta contraseña: " + password;
            request.AddParameter("html", "<html>" + message);
            request.Method = Method.POST;
            client.Execute(request);
        }

        public static void SendQuestionMessage(string correo, string nombre, string pregunta)
        {
            var client = new RestClient
            {
                BaseUrl = "https://api.mailgun.net/v2",
                Authenticator =
                    new HttpBasicAuthenticator("api",
                    "key-f32fda75e27073a696d42c8ed5c999d2")
            };

            var request = new RestRequest();
            request.AddParameter("domain",
            "app030ec7dc101b40c48a43d1e02635ebc0.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            var email = "<" + correo + ">";
            request.AddParameter("from", nombre+email);
            request.AddParameter("to", "mejia561@hotmail.com");
            request.AddParameter("subject", "Nueva Pregunta");
            var message = pregunta;
            request.AddParameter("html", "<html>" + message);
            request.Method = Method.POST;
            client.Execute(request);
        }
    }
}