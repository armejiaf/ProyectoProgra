using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using Clasificados.Extensions;
using Clasificados.Mail;
using Clasificados.Models;
using Domain.Services;
using Domain.Entities;
using NHibernate.Hql.Ast.ANTLR;

namespace Clasificados.Controllers
{
    public class OptionsController:Controller
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;

        public OptionsController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }

        public ActionResult TermsConditions()
        {
            return View();
        }

        public ActionResult FrequentQuestions()
        {
            var questions = new QuestionModel
            {
                PreguntasFrecuentes = _readOnlyRepository.GetAll<QuestionAnswer>().ToList()
            };
            if(questions.PreguntasFrecuentes == null)
                this.AddNotification("En este momento no tenemos preguntas frecuentes en nuestra base de datos, pero puedes enviarnos"+
                                     " una y con gusto trabajaremos para contestartela.",NotificationType.Info);

            return View(questions);
        }

        [HttpPost]
        public ActionResult FrequentQuestions(QuestionModel question)
        {
            switch (ValidateQuestion(question))
            {
                case true:
                    break;
                case false:
                    question.PreguntasFrecuentes = _readOnlyRepository.GetAll<QuestionAnswer>().ToList();
                    return View(question);
            }
            var qA = new QuestionAnswer
            {
                Fecha = DateTime.Now.ToString("d"),
                Correo = question.Correo,
                Nombre = question.Nombre,
                Pregunta = question.Pregunta,
                Respuesta="none"
            };
            _writeOnlyRepository.Create(qA);
            this.AddNotification("Hemos recibido su pregunta. Le contestaremos con la mayor brevedad posible.",NotificationType.Success);
            MailService.SendQuestionMessage(question.Correo, question.Nombre, question.Pregunta);
            question.PreguntasFrecuentes = _readOnlyRepository.GetAll<QuestionAnswer>().ToList();
            return View(question);
        }
        public ActionResult Detalle(long id)
        {
            var question = _readOnlyRepository.GetById<QuestionAnswer>(id);
            var pregunta = new QuestionDetailModel
            {
                Id=id,
                Fecha = question.Fecha,
                Pregunta = question.Pregunta,
                Respuesta = question.Respuesta
            };
            if(pregunta.Respuesta=="none")
                this.AddNotification("La pregunta no ha sido contestada e intentaramos resolverla lo mas pronto posible.",NotificationType.Info);

            return View(pregunta);
        }
        private bool ValidateQuestion(QuestionModel question)
        {
            if (String.IsNullOrEmpty(question.Correo) || String.IsNullOrEmpty(question.Nombre) ||
                String.IsNullOrEmpty(question.Pregunta))
            {
                this.AddNotification("Todos los campos son requeridos!",NotificationType.Error);
                return false;
            }
            var x = question.Nombre.Replace(" ", string.Empty);
            if (x.Any(t => !Char.IsLetter(t)))
            {
                this.AddNotification("El Nombre solo puede llevar letras!",NotificationType.Error);
                return false;
            }
            if (question.Nombre.Length < 3 || question.Nombre.Length > 50)
            {
                this.AddNotification("El nombre debe tener mas de 3 caracteres y menos de 50!",NotificationType.Error);
                return false;
            }
            var y = question.Pregunta.Split(null);
            var count = y.Length;
            if (count < 3 || question.Pregunta.Length > 250)
            {
                this.AddNotification("La pregunta debe tener al menos 3 palabras y menos de 250 caracteres!", NotificationType.Error);
                return false;
            }
            switch (ValidateEmail(question.Correo))
            {
                case true:
                    break;
                case false:
                    this.AddNotification("Correo Eletrónico no valido!", NotificationType.Error);
                    return false;
            }
            return true;
        }

        private static Boolean ValidateEmail(string email)
        {
            const string expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                return Regex.Replace(email, expresion, String.Empty).Length == 0;
            }
            return false;
        }

        
    }
}