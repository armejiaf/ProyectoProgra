using System.Collections.Generic;
using Domain.Entities;

namespace Clasificados.Models
{
    public class AdminModel
    {
        public List<Classified> Clasificados { get; set; }
        public List<Classified> ClasificadosDesactivados { get; set; }
        public List<QuestionAnswer> Preguntas { get; set; }
        public List<QuestionAnswer> PreguntasDesactivadas { get; set; }
        public List<ContactUserInfo> PreguntasUsuarios { get; set; }
        
 
    }
}