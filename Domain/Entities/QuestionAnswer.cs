using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    class QuestionAnswer:IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool Archived { get; set; }
        public virtual string Fecha { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Pregunta { get; set; }
        public virtual string Respuesta { get; set; }
        public virtual string Correo { get; set; }
        public virtual void Archive()
        {
            Archived = true;
        }

        public virtual void Activate()
        {
            Archived = false;
        }
    }
}
