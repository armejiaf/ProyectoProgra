using System.Collections.Generic;
using Domain.Entities;
using FluentNHibernate.Testing.Values;

namespace Clasificados.Models
{
    public class SimpleSearchModel
    {
        public string Search { get; set; }
        public List<Classified> Clasificados;

    }
}