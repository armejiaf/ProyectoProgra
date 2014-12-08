using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace Clasificados.Models
{
    public class MangeProfileModel
    {
        
        [RegularExpression("[0-9]*", ErrorMessage = "El telefono solo puede tener numeros!")]
        public string Telefono { get; set; }
        public bool Miembro { get; set; }
      
    }
}