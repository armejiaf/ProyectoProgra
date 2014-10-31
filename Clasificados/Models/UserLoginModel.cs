using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clasificados.Models
{
    public class UserLoginModel
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        
    }
}