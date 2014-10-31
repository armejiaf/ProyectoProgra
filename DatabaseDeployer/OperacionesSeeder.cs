using Domain.Entities;
using DomainDrivenDatabaseDeployer;
using FizzWare.NBuilder;
using NHibernate;

namespace DatabaseDeployer
{
    class OperacionesSeeder : IDataSeeder
    {
        readonly ISession _session;

        public OperacionesSeeder(ISession session)
        {
            _session = session;
        }

        public void Seed()
        {

        }
    }
}