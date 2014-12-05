using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using BotDetect.Web.UI.Mvc;
using CaptchaMvc.HtmlHelpers;
using Clasificados.Extensions;
using Clasificados.Mail;
using Clasificados.Models;
using Domain.Entities;
using Domain.Services;


namespace Clasificados.Controllers
{

    public class HomeController : Controller
    {
        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;

        public HomeController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }

        public ActionResult TermsConditions()
        {
            return View();
        }
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                Session["User"] = "Anonymous";
            }

            var index = new IndexModel
            {
                ClasificadosRecientes = new List<Classified>(5),
                ClasificadosDestacados = new List<Classified>(5),
                ClasificadosRecomendados = new List<Classified>(11)
            };

            var clasificados = _readOnlyRepository.GetAll<Classified>().ToArray();
            var desc = from s in clasificados
                orderby s.Visitas descending
                select s;
            var desc1 = from s in clasificados
                orderby s.Recomendado ascending
                select s;
         

            var cont = clasificados.Length;
            GetClasificadosRecientes(index,cont,clasificados);
            GetClasificadosDestacados(cont, desc, index);
            GetClasificadosRecomendados(desc1, index);


            return View(index);
        }

        public ActionResult Contact()
        {
            return View(new ContactModel());
        }

        [HttpPost]
        public ActionResult Contact(ContactModel contact)
        {
            if (!this.IsCaptchaValid("Captcha is not valid")) return View(contact);
            if (ModelState.IsValid)
            {
                var contt = new ContactInfo
                {
                    Nombre = contact.Nombre,
                    Correo = contact.Correo,
                    Mensaje = contact.Mensaje
                };
                _writeOnlyRepository.Create(contt);
                MailService.SendContactMessage(contact.Correo, contact.Nombre, contact.Mensaje);
                this.AddNotification("Se ha recibido el mensaje.", NotificationType.Success);
                return View(contact);
            }
            this.AddNotification("No se a podido enviar su pregunta!", NotificationType.Warning);
            return View(contact);

        }

        private static void GetClasificadosRecomendados(IEnumerable<Classified> desc1, IndexModel index)
        {
            Classified bienesRaices = null;
            Classified automovil = null;
            Classified nautica = null;
            Classified computacion = null;
            Classified joyeria = null;
            Classified musica = null;
            Classified arte = null;
            Classified hogar = null;
            Classified deportes = null;
            Classified telefonia = null;
            Classified animales = null;
            foreach (var s in desc1)
            {
                switch (s.Categoria)
                {
                    case "Bienes Raices":
                        if (bienesRaices == null)
                            bienesRaices = new Classified();
                        if (bienesRaices.Recomendado < s.Recomendado)
                            bienesRaices = s;

                        break;
                    case "Automovil":
                        if (automovil == null)
                            automovil = new Classified();
                        if (automovil.Recomendado < s.Recomendado)
                            automovil = s;

                        break;
                    case "Nautica":
                        if (nautica == null)
                            nautica = new Classified();
                        if (nautica.Recomendado < s.Recomendado)
                            nautica = s;

                        break;
                    case "Computación":
                        if (computacion == null)
                            computacion = new Classified();
                        if (computacion.Recomendado < s.Recomendado)
                            computacion = s;

                        break;
                    case "Joyería":
                        if (joyeria == null)
                            joyeria = new Classified();
                        if (joyeria.Recomendado < s.Recomendado)
                            joyeria = s;

                        break;
                    case "Música":
                        if (musica == null)
                            musica = new Classified();
                        if (musica.Recomendado < s.Recomendado)
                            musica = s;

                        break;
                    case "Arte":
                        if (arte == null)
                            arte = new Classified();
                        if (arte.Recomendado < s.Recomendado)
                            arte = s;

                        break;
                    case "Hogar":
                        if (hogar == null)
                            hogar = new Classified();
                        if (hogar.Recomendado < s.Recomendado)
                            hogar = s;

                        break;
                    case "Deportes":
                        if (deportes == null)
                            deportes = new Classified();
                        if (deportes.Recomendado < s.Recomendado)
                            deportes = s;

                        break;
                    case "Telefonía":
                        if (telefonia == null)
                            telefonia = new Classified();
                        if (telefonia.Recomendado < s.Recomendado)
                            telefonia = s;

                        break;
                    case "Animales":
                        if (animales == null)
                            animales = new Classified();
                        if (animales.Recomendado < s.Recomendado)
                            animales = s;

                        break;
                }
            }

            if (bienesRaices != null)
                index.ClasificadosRecomendados.Add(bienesRaices);
            if (automovil != null)
                index.ClasificadosRecomendados.Add(automovil);
            if (nautica != null)
                index.ClasificadosRecomendados.Add(nautica);
            if (computacion != null)
                index.ClasificadosRecomendados.Add(computacion);
            if (joyeria != null)
                index.ClasificadosRecomendados.Add(joyeria);
            if (musica != null)
                index.ClasificadosRecomendados.Add(musica);
            if (arte != null)
                index.ClasificadosRecomendados.Add(arte);
            if (deportes != null)
                index.ClasificadosRecomendados.Add(deportes);
            if (hogar != null)
                index.ClasificadosRecomendados.Add(hogar);
            if (telefonia != null)
                index.ClasificadosRecomendados.Add(telefonia);
            if (animales != null)
                index.ClasificadosRecomendados.Add(animales);
        }

        private static void GetClasificadosDestacados(int cont, IEnumerable<Classified> desc, IndexModel index)
        {
            if (cont < 5)
            {
                foreach (var s in desc)
                {
                    index.ClasificadosDestacados.Add(s);
                }
            }

            else
            {
                for (var i = 0; i < 5; i++)
                {
                    index.ClasificadosDestacados.Add(desc.ElementAtOrDefault(i));
                }
            }
        }

        public static void GetClasificadosRecientes(IndexModel index, int cont, Classified[] clasificados)
        {
            if (cont < 5)
            {
                foreach (var t in clasificados)
                {
                    index.ClasificadosRecientes.Add(clasificados[cont - 1]);
                    cont--;
                }
            }

            else
            {
                for (var i = 0; i < 5; i++)
                {
                    index.ClasificadosRecientes.Add(clasificados[cont - 1]);
                    cont--;
                }
            }

        }
    }
}