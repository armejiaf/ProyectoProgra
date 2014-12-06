using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Clasificados.Extensions;
using Clasificados.Mail;
using Clasificados.Models;
using Domain.Services;
using Domain.Entities;

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

        public ActionResult FrequentQuestions()
        {
            var questions = new QuestionModel
            {
                PreguntasFrecuentes = _readOnlyRepository.GetAll<QuestionAnswer>().Where(x=>x.Archived==false).ToList()
            };
            if(questions.PreguntasFrecuentes.Count==0)
                this.AddNotification("En este momento no tenemos preguntas frecuentes en nuestra base de datos, pero puedes enviarnos"+
                                     " una y con gusto trabajaremos para contestartela.",NotificationType.Info);

            return View(questions);
        }

        [HttpPost]
        public ActionResult FrequentQuestions(QuestionModel question)
        {
            if (ModelState.IsValid)
            {
                var qA = new QuestionAnswer
                {
                    Fecha = DateTime.Now.ToString("d"),
                    Correo = question.Correo,
                    Nombre = question.Nombre,
                    Pregunta = question.Pregunta,
                    Respuesta = "none"
                };
                _writeOnlyRepository.Create(qA);
                this.AddNotification("Hemos recibido su pregunta. Le contestaremos con la mayor brevedad posible.", NotificationType.Success);
                MailService.SendQuestionMessage(question.Correo, question.Nombre, question.Pregunta);
                question.PreguntasFrecuentes = _readOnlyRepository.GetAll<QuestionAnswer>().ToList();
                return View(question);
            }
            this.AddNotification("Pregunta Invalida!",NotificationType.Warning);
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
            if (ModelState.IsValid)
            {
                var contt = new ContactUserInfo
                {
                    Nombre = detalle.Nombre,
                    Correo = detalle.Correo,
                    Mensaje = detalle.Mensaje
                };
                _writeOnlyRepository.Create(contt);
                var clas = _readOnlyRepository.GetById<Classified>(detalle.IdClasificado);
                detalle.Usuario = _readOnlyRepository.GetById<User>(clas.IdUsuario);
                MailService.SendContactMessageToUser(detalle.Correo, detalle.Nombre, detalle.Mensaje, detalle.Usuario.Correo);
                this.AddNotification("Se ha enviado el mensaje.", NotificationType.Success);
                return RedirectToAction("DetalleCategory", detalle.IdClasificado);
            }

            this.AddNotification("No se pudo enviar mensaje a vendedor.", NotificationType.Warning);
            return RedirectToAction("DetalleCategory", detalle.IdClasificado);
        }
        [HttpPost]
        public ActionResult ReportDenunciar(DetalleCategoryModel detalle,int id)
        {      
                var clas = _readOnlyRepository.GetById<Classified>(id);
                var usuario = _readOnlyRepository.GetById<User>(clas.IdUsuario);
                MailService.SendReportMessageToAdmin(usuario.Nombre, detalle.Categoria,clas.Id);
                this.AddNotification("Se ha enviado la denuncia.", NotificationType.Success);
                return RedirectToAction("DetalleCategory", new {id});    
        }
        public ActionResult SimpleSearch(string query)
        {
            
            var ss = new SimpleSearchModel
            {
                Clasificados = _readOnlyRepository.GetAll<Classified>().ToList()
            };
            var cls = ss.Clasificados.Where(x => x.Titulo.Contains(query) && x.Archived==false).ToList();
            ss.Clasificados = cls;
            if (ss.Clasificados.Count == 0 || query == null)
            {
                this.AddNotification("No se encontro ningun Clasificado con ese titulo!", NotificationType.Info);
                return RedirectToAction("Index", "Home");
            }
            
            return View(ss);
        }


        public ActionResult AdvanzedSearch()
        {
            var advanz = new AdvanzedSearchModel
            {
                Clasificados = _readOnlyRepository.GetAll<Classified>().ToList()
            };
            var filtro = advanz.Clasificados.Where(x => x.Archived == false).ToList();
            advanz.Clasificados = filtro;
            return View(advanz);
        }

        [HttpPost]
        public ActionResult AdvanzedSearch(AdvanzedSearchModel advanz)
        {
            var simple = _readOnlyRepository.GetAll<Classified>().ToList();
            var filtro = simple.Where(x => x.Archived == false).ToList();

            if (!String.IsNullOrEmpty(advanz.Search))
            {
                foreach (var classified in filtro.Where(classified => classified.Categoria.Contains(advanz.Categoria) &&
                    classified.Titulo.Contains(advanz.Search) && classified.Descripcion.Contains(advanz.Descripcion)))
                {
                              if (advanz.Clasificados == null)
                                advanz.Clasificados = new List<Classified>(filtro.Count());
                            advanz.Clasificados.Add(classified);                    
                }             
            }
            if (advanz.Clasificados != null)
            {
                return View(advanz);
            }
            this.AddNotification("No se encontro ningun Clasificado!", NotificationType.Info);
            advanz.Clasificados = filtro;
            return View(advanz);
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
            var client = _readOnlyRepository.FirstOrDefault<User>(x => x.Nombre == (string) Session["User"]);
            if (css.IdUsuario == client.Id)
            {
                this.AddNotification("No se puede recomendar su propio clasificado!",NotificationType.Warning);
                return RedirectToAction("Index", "Home");
            }
            css.Recomendado += 1;
            _writeOnlyRepository.Update(css);
            return RedirectToAction("Index","Home");
        }
    }
}