using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clasificados.Models
{
    public class QuestionDetailModel
    {
        public long Id { get; set; }
        public string Pregunta { get; set; }
        public string Respuesta { get; set; }
        public string Fecha { get; set; }

    }
}
