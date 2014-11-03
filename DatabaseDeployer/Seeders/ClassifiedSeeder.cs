using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using DomainDrivenDatabaseDeployer;
using FizzWare.NBuilder;
using NHibernate;

namespace DatabaseDeployer.Seeders
{
    class ClassifiedSeeder:IDataSeeder
    {
        readonly ISession _session;

        public ClassifiedSeeder(ISession session)
        {
            _session = session;
        }

        public void Seed()
        {
            var clas = Builder<Classified>.CreateNew().Build();
            clas.FechaCreacion = DateTime.Now.ToString("d");
            clas.Titulo = "Nuevo Clasificado";
            clas.Precio = "1000";
            clas.UrlImg0 = "http://www.reusableart.com/miwp/wp-content/uploads/2014/03/church-drawings-04.jpg";
            clas.IdUsuario = 1;
            clas.Negocio = "Venta";
            clas.Categoria = "Arte";
            clas.Descripcion = "Venta de una obra de arte.";
            clas.UrlVideo = "https://www.youtube.com/watch?v=guZeAjdYw_c";
            _session.Save(clas);
        }
    }
}
