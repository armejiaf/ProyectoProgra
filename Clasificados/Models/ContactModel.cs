using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clasificados.Models
{
    public class ContactModel
    {
        public string Mensaje { get; set; }
        public string Correo { get; set; }
        public string Nombre { get; set; }
        public string CaptchaCode { get; set; }
    }
}