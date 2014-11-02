using System;
using Domain.Entities;
using DomainDrivenDatabaseDeployer;
using FizzWare.NBuilder;
using NHibernate;

namespace DatabaseDeployer.Seeders
{
    class QuestionAnswerSeeder : IDataSeeder
    {
        readonly ISession _session;

        public QuestionAnswerSeeder(ISession session)
        {
            _session = session;
        }

        public void Seed()
        {
            var questionAnswer = Builder<QuestionAnswer>.CreateNew().Build();

            questionAnswer.Fecha = DateTime.Now.ToString("d");
            questionAnswer.Pregunta = "Como me puedo registrar a la pagina?";
            questionAnswer.Respuesta = "Te vas a la pagina de inicio. Luego en la esquina superior derecha encontraras" +
                                       " un boton que dice 'Iniciar Sesion' y adentro hay una opcion que dice 'Registrar'.";
            _session.Save(questionAnswer);
        }
    }
}