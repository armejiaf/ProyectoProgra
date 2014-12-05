using System.Collections.Generic;
using Domain.Entities;

namespace Clasificados.Models
{
    public class CategryModel
    {
        public List<Classified> Clasificados { get; set; }
        public string Categoria { get; set; }
    }
}