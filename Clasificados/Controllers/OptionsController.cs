using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using Clasificados.Extensions;
using Clasificados.Mail;
using Clasificados.Models;
using Domain.Services;
using Domain.Entities;
using FluentNHibernate.Conventions.AcceptanceCriteria;
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

        public ActionResult CategoryInfo()
        {

            var clas = new CategryModel
            {
                Clasificados = _readOnlyRepository.GetAll<Classified>().ToList()
            };
            return View(clas);
        }

        [HttpPost]
        public ActionResult CategoryInfo(CategryModel info)
        {
            var clas = _readOnlyRepository.GetAll<Classified>().ToList();
            info.Clasificados = new List<Classified>(clas.Count + 1);
            foreach (var c in clas.Where(c => c.Categoria == info.Categoria))
            {
                if (String.IsNullOrEmpty(c.UrlImg0))
                    c.UrlImg0 =
                        "none";

                if (String.IsNullOrEmpty(c.UrlImg1))
                    c.UrlImg1 =
                        "none";

                if (String.IsNullOrEmpty(c.UrlImg2))
                    c.UrlImg2 =
                        "none";

                if (String.IsNullOrEmpty(c.UrlImg3))
                    c.UrlImg3 =
                        "none";

                if (String.IsNullOrEmpty(c.UrlImg4))
                    c.UrlImg4 =
                        "none";

                if (String.IsNullOrEmpty(c.UrlImg5))
                    c.UrlImg5 =
                        "none";
                if (String.IsNullOrEmpty(c.UrlVideo))
                    c.UrlVideo = "none";

                info.Clasificados.Add(c);
                {

                    if (info.Clasificados == null)
                    {
                        var clsif = new Classified
                        {
                            Categoria = "none",
                            FechaCreacion = "none",
                            Id = 0,
                            Archived = false,
                            IdUsuario = 0,
                            Descripcion = "none",
                            Negocio = "none",
                            Recomendado = 0,
                            UrlImg0 = "none",
                            Precio = "none",
                            Titulo = "none",
                            UrlImg1 = "nonce",
                            UrlVideo = "none",
                            UrlImg2 = "none",
                            UrlImg3 = "none",
                            UrlImg4 = "none",
                            UrlImg5 = "none",
                            Visitas = 0
                        };
                        info.Clasificados.Add(clsif);

                        this.AddNotification("No existen clasificados de esa categoria.", NotificationType.Info);
                        return View(info);
                    }
                    
                }
            }
            return View(info);
        }

     

        public ActionResult DetalleCategory(int id)
        {
            if (id ==0)
            {
                this.AddNotification("No se puede obtener detalle de objeto que no existe!",NotificationType.Warning);
                return RedirectToAction("CategoryInfo");
            }
            var detalle = _readOnlyRepository.GetById<Classified>(id);
            detalle.Visitas += 1;
            _writeOnlyRepository.Update(detalle);
            var detail = new DetalleCategoryModel
            {
                IdClasificado = detalle.Id,
                Archived = detalle.Archived,
                IdUsuario =detalle.IdUsuario,
                Categoria = detalle.Categoria,
                FechaCreacion = detalle.FechaCreacion,
                Descripcion = detalle.Descripcion,
                Negocio = detalle.Negocio,
                Precio = detalle.Precio,
                Recomendado = detalle.Recomendado,
                Titulo = detalle.Titulo,
                Visitas = detalle.Visitas
            };
     
            const string defaultUrl = "http://www.theinvestigativemusicologist.com/image.axd?picture=2012%2F7%2Fcreative-commons-public-domain.png";
            ValidarCampos(detalle, detail, defaultUrl);
            if(detail.UrlVideo!="none")
                detail.UrlVideo=detail.UrlVideo.Replace("watch?v=", "embed/");
            detail.Usuario = _readOnlyRepository.GetById<User>(detail.IdUsuario);

            return View(detail);
        }

        [HttpPost]
        public ActionResult DetalleCategory(DetalleCategoryModel detalle)
        {
            switch (ValidateContact(detalle))
            {
                case true:
                    break;
                case false:
                    return RedirectToAction("DetalleCategory",detalle.IdClasificado);
            }

            var contt = new ContactInfo
            {
                Nombre = detalle.Nombre,
                Correo = detalle.Correo,
                Mensaje = detalle.Mensaje
            };
            _writeOnlyRepository.Create(contt);
            var clas = _readOnlyRepository.GetById<Classified>(detalle.IdClasificado);
            detalle.Usuario = _readOnlyRepository.GetById<User>(clas.IdUsuario);
            MailService.SendContactMessageToUser(detalle.Correo, detalle.Nombre, detalle.Mensaje,detalle.Usuario.Correo);
            this.AddNotification("Se ha enviado el mensaje.", NotificationType.Success);
            return RedirectToAction("DetalleCategory", detalle.IdClasificado);
        }
        public ActionResult SimpleSearch()
        {
            
            var ss = new SimpleSearchModel
            {
                Clasificados = _readOnlyRepository.GetAll<Classified>().ToList()
            };

            
            return View(ss);
        }

        [HttpPost]
        public ActionResult SimpleSearch(SimpleSearchModel ss)
        {
            var simple = _readOnlyRepository.GetAll<Classified>().ToList();
            if (!String.IsNullOrEmpty(ss.Search))
            {
                foreach (var classified in simple.Where(classified => classified.Titulo.Contains(ss.Search)))
                {
                    if(ss.Clasificados==null)
                        ss.Clasificados = new List<Classified>(simple.Count);
                    ss.Clasificados.Add(classified);
                }
            }
            if (ss.Clasificados != null)
            {
                return View(ss);
            }
            this.AddNotification("No se encontro ningun Clasificado por ese Titulo", NotificationType.Info);
            ss.Clasificados = _readOnlyRepository.GetAll<Classified>().ToList();
            return View(ss);

        }

        public ActionResult AdvanzedSearch()
        {
            var advanz = new AdvanzedSearchModel
            {
                Clasificados = _readOnlyRepository.GetAll<Classified>().ToList()
            };

            return View(advanz);
        }

        [HttpPost]
        public ActionResult AdvanzedSearch(AdvanzedSearchModel advanz)
        {
            var simple = _readOnlyRepository.GetAll<Classified>().ToList();
            
            if (!String.IsNullOrEmpty(advanz.Search))
            {
                foreach (var classified in simple.Where(classified => classified.Categoria.Contains(advanz.Categoria)))
                {
                    foreach (var classi in simple.Where(classi => classi.Titulo.Contains(advanz.Search)))
                    {
                        foreach (var clas in simple.Where(clas => clas.Descripcion.Contains(advanz.Descripcion)))
                        {
                            if (advanz.Clasificados == null)
                                advanz.Clasificados = new List<Classified>(simple.Count());
                            advanz.Clasificados.Add(classified);
                        }
                    }
                    
                }             
            }
            if (advanz.Clasificados != null)
            {
                return View(advanz);
            }
            this.AddNotification("No se encontro ningun Clasificado!", NotificationType.Info);
            advanz.Clasificados = _readOnlyRepository.GetAll<Classified>().ToList();
            return View(advanz);
        }


        private bool ValidateContact(DetalleCategoryModel detalle)
        {
            if (String.IsNullOrEmpty(detalle.Correo) || String.IsNullOrEmpty(detalle.Nombre) ||
                String.IsNullOrEmpty(detalle.Mensaje))
            {
                this.AddNotification("Todos los campos son requeridos!", NotificationType.Error);
                return false;
            }
            var x = detalle.Nombre.Replace(" ", string.Empty);
            if (x.Any(t => !Char.IsLetter(t)))
            {
                this.AddNotification("El Nombre solo puede llevar letras!", NotificationType.Error);
                return false;
            }
            if (detalle.Nombre.Length < 3 || detalle.Nombre.Length > 50)
            {
                this.AddNotification("El nombre debe tener mas de 3 caracteres y menos de 50!", NotificationType.Error);
                return false;
            }
            var y = detalle.Mensaje.Split(null);
            var count = y.Length;
            if (count < 3 || detalle.Mensaje.Length > 250)
            {
                this.AddNotification("La pregunta debe tener al menos 3 palabras y menos de 250 caracteres!",
                    NotificationType.Error);
                return false;
            }
            switch (ValidateEmail(detalle.Correo))
            {
                case true:
                    break;
                case false:
                    this.AddNotification("Correo Eletrónico no valido!", NotificationType.Error);
                    return false;
            }
            return true;
        }

        private void ValidarCampos(Classified detalle, DetalleCategoryModel detail, string defaultUrl)
        {
            switch (RemoteFileExists(detalle.UrlImg0))
            {
                case true:
                    detail.UrlImg0 = detalle.UrlImg0;
                    break;
                case false:
                    detail.UrlImg0 = defaultUrl;
                    break;
            }
            switch (RemoteFileExists(detalle.UrlImg1))
            {
                case true:
                    detail.UrlImg1 = detalle.UrlImg1;
                    break;
                case false:
                    detail.UrlImg1 = defaultUrl;
                    break;
            }
            switch (RemoteFileExists(detalle.UrlImg2))
            {
                case true:
                    detail.UrlImg2 = detalle.UrlImg2;
                    break;
                case false:
                    detail.UrlImg2 = defaultUrl;
                    break;
            }
            switch (RemoteFileExists(detalle.UrlImg3))
            {
                case true:
                    detail.UrlImg3 = detalle.UrlImg3;
                    break;
                case false:
                    detail.UrlImg3 = defaultUrl;
                    break;
            }
            switch (RemoteFileExists(detalle.UrlImg4))
            {
                case true:
                    detail.UrlImg4 = detalle.UrlImg4;
                    break;
                case false:
                    detail.UrlImg4 = defaultUrl;
                    break;
            }
            switch (RemoteFileExists(detalle.UrlImg5))
            {
                case true:
                    detail.UrlImg5 = detalle.UrlImg5;
                    break;
                case false:
                    detail.UrlImg5 = defaultUrl;
                    break;
            }
            switch (RemoteFileExists(detalle.UrlVideo))
            {
                case true:
                    detail.UrlVideo = detalle.UrlVideo;
                    break;
                case false:
                    detail.UrlVideo = "none";
                    break;
            }

        }

        private bool ValidateQuestion(QuestionModel question)
        {
            if (String.IsNullOrEmpty(question.Correo) || String.IsNullOrEmpty(question.Nombre) ||
                String.IsNullOrEmpty(question.Pregunta))
            {
                this.AddNotification("Todos los campos son requeridos!", NotificationType.Error);
                return false;
            }
            var x = question.Nombre.Replace(" ", string.Empty);
            if (x.Any(t => !Char.IsLetter(t)))
            {
                this.AddNotification("El nombre solo puede llevar letras!", NotificationType.Error);
                return false;
            }
            if (question.Nombre.Length < 3 || question.Nombre.Length > 50)
            {
                this.AddNotification("El nombre debe tener mas de 3 caracteres y menos de 50!", NotificationType.Error);
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
        private bool RemoteFileExists(string url)
        {
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                var response = request.GetResponse() as HttpWebResponse;
                return response != null && (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }
        [HttpPost]
        public ActionResult Recommend(long id)
        {
            var css = _readOnlyRepository.GetById<Classified>(id);
            css.Recomendado += 1;
            _writeOnlyRepository.Update(css);
            return RedirectToAction("Index","Home");
        }
    }
}