using System;
using System.Collections.Generic;
using System.Threading;
using AcklenAvenue.Data.NHibernate;
using Data;
using DatabaseDeployer.Seeders;
using DomainDrivenDatabaseDeployer;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace DatabaseDeployer
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConnectionStrings.Get();

            var databaseConfiguration = MsSqlConfiguration.MsSql2008.ShowSql().
                ConnectionString(x => x.Is(connectionString));

            DomainDrivenDatabaseDeployer.DatabaseDeployer dd = null;
            var sessionFactory = new SessionFactoryBuilder(new MappingScheme(), databaseConfiguration)
                .Build(cfg => { dd = new DomainDrivenDatabaseDeployer.DatabaseDeployer(cfg); });

            dd.Drop();
            Console.WriteLine("Database dropped.");
            Thread.Sleep(1000);

            dd.Create();
            Console.WriteLine("Database created.");

            var session = sessionFactory.OpenSession();
            using (var tx = session.BeginTransaction())
            {
                dd.Seed(new List<IDataSeeder>
                {
                    new QuestionAnswerSeeder(session),
                    new UserSeeder(session),
                    new ObjectSeeder(session),
                    new ClassifiedSeeder(session)
                });
                tx.Commit();
            }
            session.Close();
            sessionFactory.Close();
            Console.WriteLine("Seed data added.");
            Thread.Sleep(2000);
        }
    }

    public class ObjectSeeder : IDataSeeder
    {
        readonly ISession _session;

        public ObjectSeeder(ISession session)
        {
            _session = session;
        }

        public void Seed()
        {
            
        }
    }
}