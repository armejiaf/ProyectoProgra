using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace Clasificados.Models
{
    public class IndexModel
    {
        public List<Classified> ClasificadosRecientes { get; set; }
        public List<Classified> ClasificadosDestacados { get; set; }
        public List<Classified> ClasificadosRecomendados { get; set; }
    }
}