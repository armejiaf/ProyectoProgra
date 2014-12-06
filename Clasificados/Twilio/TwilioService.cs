using System;
using Twilio;

namespace Clasificados.Twilio
{
    public class TwilioService
    {
        public static void SendSms(string mensaje,string correo,string nombre,string titulo)
        {
            const string accountSid = "AC336a01b1ce1e2083e9cb78c84af11637";
            const string authToken = "6badca1178ff995d0a38fa3424d8f6df";
            var twilio = new TwilioRestClient(accountSid, authToken);

            var message = twilio.SendMessage("+17476002179", "+504 3191-8027", nombre+" le ha enviado una pregunta. Clasificado: "+titulo+
                ". Pregunta: "+mensaje+". Correo: "+correo);
            Console.WriteLine(message.Sid); 
        }

        public static void SendSmsToSubscribers(string nombresuscrito, string titulo, string nombreusuario)
        {
            const string accountSid = "AC336a01b1ce1e2083e9cb78c84af11637";
            const string authToken = "6badca1178ff995d0a38fa3424d8f6df";
            var twilio = new TwilioRestClient(accountSid, authToken);

            var message = twilio.SendMessage("+17476002179", "+504 3191-8027",nombresuscrito+". "+nombreusuario+" ha creado un nuevo clasificado: "+titulo );
            Console.WriteLine(message.Sid);
        }
    }
}