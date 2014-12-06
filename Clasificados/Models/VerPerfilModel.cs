using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace Clasificados.Models
{
    public class VerPerfilModel
    {
        public User Usuario { get; set; }
        public List<Classified> Clasificados { get; set; } 
    }
}