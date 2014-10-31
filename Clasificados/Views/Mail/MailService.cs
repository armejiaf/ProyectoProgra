using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Clasificados.Models;
using RestSharp;

namespace Clasificados.Views.Mail
{
    public class MailService
    {
        public static void SendGreetingMessage(UserRegisterModel user)
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
            request.AddParameter("from", "Clasificados <postmaster@app030ec7dc101b40c48a43d1e02635ebc0.mailgun.org>");
            request.AddParameter("to", user.Correo);
            request.AddParameter("bcc", "Allan Mejia <mejia561@unitec.edu>");
            request.AddParameter("subject", "Bienvenido");
            var message = "Bienvenido a Clasificados " + user.Nombre +
                          " para ingresar a nuestra pagina ve a (http://proyectoprogra.apphb.com/Home/Login)";
            request.AddParameter("html", "<html>" + message + "<BR><BR>Email: " + user.Correo + "<BR>Password: " + user.Password);
            request.Method = Method.POST;
            client.Execute(request);
        }

        internal static void SendRestorePassMessage(UserRecoverPasswordModel user)
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
            request.AddParameter("from", "Clasificados <postmaster@app030ec7dc101b40c48a43d1e02635ebc0.mailgun.org>");
            request.AddParameter("to", user.Correo);
            request.AddParameter("bcc", "Allan Mejia <mejia561@unitec.edu>");
            request.AddParameter("subject", "Nueva Contraseña");
            var message = " " + user.Nombre + " puede ingresar nuevamente a nuestra pagina con esta contraseña: " + user.Password;
            request.AddParameter("html", "<html>" + message);
            request.Method = Method.POST;
            client.Execute(request);
        }
    }
}