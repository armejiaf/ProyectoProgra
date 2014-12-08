namespace Domain.Entities
{
    public class User : IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool Archived { get; set; }
        public virtual void Archive()
        {
            Archived = true;
        }

        public virtual void Activate()
        {
            Archived = false;
        }

        public virtual string Nombre{get; set; }
        public virtual string Correo { get; set; }
        public virtual string Telefono { get; set; }
        public virtual string Password { get; set; }
        public virtual string Role { get; set; }
        public virtual bool Miembro { get; set; }
        public virtual int TotalClasificados { get; set; }
        public virtual string Salt { get; set; }

    }
}