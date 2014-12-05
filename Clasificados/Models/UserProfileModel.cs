using System.Collections.Generic;
using Domain.Entities;
using FluentNHibernate.Testing.Values;

namespace Clasificados.Models
{
    public class UserProfileModel
    {
        public long UserId { get; set; }
        public string Correo { get; set; }
        public string Nombre { get; set; }
        public List<Classified> Clasificados { get; set; }
    }
}