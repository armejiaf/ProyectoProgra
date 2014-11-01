using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace Clasificados.Models
{
    public class QuestionModel
    {
        public string Correo { get; set; }
        public string Nombre { get; set; }
        public string Pregunta { get; set; }

        public List<QuestionAnswer> PreguntasFrecuentes { get; set; } 
    }
}