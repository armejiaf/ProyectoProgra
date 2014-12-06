namespace Domain.Entities
{
    public class Suscribtions:IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool Archived { get; set; }
        public virtual long IdUsuarioClasificado { get; set; }
        public virtual long IdUsuarioSuscrito { get; set; }
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