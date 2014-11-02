using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clasificados.Models
{
    public class ClassifiedModel
    {
        public long IdClasificado { get; set; }
        public long IdUsuario { get; set; }
        public string Titulo { get; set; }
        public string Categoria { get; set; }
        public string Descripcion { get; set; }
        public string Precio { get; set; }
        public string Negocio { get; set; }
        public string UrlImg0 { get; set; }
        public string UrlImg1 { get; set; }
        public string UrlImg2 { get; set; }
        public string UrlImg3 { get; set; }
        public string UrlImg4 { get; set; }
        public string UrlImg5 { get; set; }

        public string UrlVideo { get; set; }
    }
}