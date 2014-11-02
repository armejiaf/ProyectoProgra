using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Classified:IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool Archived { get; set; }
        public virtual long IdUsuario { get; set; }
        public virtual string Titulo { get; set; }
        public virtual string Categoria { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Precio { get; set; }
        public virtual string Negocio { get; set; }
        public virtual string UrlVideo { get; set; }
        public virtual int Visitas { get; set; }
        public virtual int Recomendado { get; set; }
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
