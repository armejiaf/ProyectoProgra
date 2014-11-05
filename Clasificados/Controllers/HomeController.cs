using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Windows.Forms;
using BotDetect.Web.UI.Mvc;
using Clasificados.Extensions;
using Clasificados.Mail;
using Clasificados.Models;
using Domain.Entities;
using Domain.Services;
using RestSharp.Extensions;


namespace Clasificados.Controllers
{

    public class HomeController : Controller
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;

        public HomeController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
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
            Classified bienesRaices = null;
            Classified automovil=null;
            Classified nautica=null;
            Classified computacion=null;
            Classified joyeria=null;
            Classified musica=null;
            Classified arte=null;
            Classified hogar=null;
            Classified deportes=null;
            Classified telefonia=null;
            Classified animales=null;

            var cont = clasificados.Length;
            if (cont < 5)
            {
                foreach (var t in clasificados)
                {
                    index.ClasificadosRecientes.Add(clasificados[cont - 1]);
                    cont--;
                }
                foreach (var s in desc)
                {
                    index.ClasificadosDestacados.Add(s);
                }
               
            }

            else
            {
                for (var i = 0; i < 5; i++)
                {
                    index.ClasificadosRecientes.Add(clasificados[cont - 1]);
                    cont--;
                }
                for (var i = 0; i < 5; i++)
                {
                    index.ClasificadosDestacados.Add(desc.ElementAtOrDefault(i));
                }
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
            }
            if(bienesRaices!=null)
                index.ClasificadosRecomendados.Add(bienesRaices);
            if (automovil != null)
                index.ClasificadosRecomendados.Add(automovil);
            if(nautica!=null)
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


            return View(index);
        }

        public ActionResult Contact()
        {
            return View(new ContactModel());
        }

        [HttpPost]
        [CaptchaValidation("CaptchaCode", "SampleCaptcha", "Incorrect CAPTCHA code!")]
        public ActionResult Contact(ContactModel contact)
        {

            if (ModelState.IsValid)
            {
                switch (ValidateContact(contact))
                {
                    case true:
                        break;
                    case false:
                        return View(contact);
                }

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
            this.AddNotification("Captcha incorrecto!",NotificationType.Error);
            return View(contact);

        }


        public ActionResult Register()
        {
            return View(new UserRegisterModel());
        }

        [HttpPost]
        public ActionResult Register(UserRegisterModel register)
        {
            var usuario = new User();
            var valid = ValidateNewUser(register);
            
            switch (valid)
            {
                case true:
                    break;
                case false:
                    return View(register);
            }
            var user = _readOnlyRepository.FirstOrDefault<User>(x => x.Correo == register.Correo);
            if (user != null)
            {
                this.AddNotification("Usuario ya existe!", NotificationType.Warning);
                return View(register);
            }
            MailService.SendGreetingMessage(register.Correo,register.Nombre,register.Password);
            EncryptRegister(register);
            usuario.Nombre = register.Nombre;
            usuario.Correo = register.Correo;
            usuario.Password = register.Password;
            usuario.Salt = register.Salt;
            _writeOnlyRepository.Create(usuario);
            this.AddNotification("Se ha registrado exitosamente.", NotificationType.Success);
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View(new UserLoginModel());
        }

        [HttpPost]
        public ActionResult Login(UserLoginModel login)
        {
            var usuario = _readOnlyRepository.FirstOrDefault<User>(x => x.Correo == login.Correo);
            if (usuario == null)
            {
                this.AddNotification("Correo Electrónico incorrecto!", NotificationType.Error);
                return View();
            }
            var verification = new UserRegisterModel
            {
                Nombre = usuario.Nombre,
                Password = login.Password,
                Correo = login.Correo,
            };
            switch (ValidateLogin(verification))
            {
                case true:
                    break;
                case false:
                    return View();
            };
            EncryptRegister(verification);
            login.Password = verification.Password;
            if (usuario.Password != login.Password)
            {
                this.AddNotification("Usuario o Contraseña incorrectos!", NotificationType.Error);
                return View();
            }
            Session["User"] = usuario.Nombre;
            return RedirectToAction("Index");
        }

        public ActionResult RecoverPassword()
        {
            return View(new UserRecoverPasswordModel());
        }

        [HttpPost]
        public ActionResult RecoverPassword(UserRecoverPasswordModel userRecover)
        {
            var user = _readOnlyRepository.FirstOrDefault<User>(x=>x.Correo==userRecover.Correo);
            if (user == null)
            {
                this.AddNotification("Correo Electrónico incorrecto!", NotificationType.Error);
                return View();
            }
            userRecover.Password = Guid.NewGuid().ToString().Substring(0, 8);
            userRecover.Nombre = user.Nombre;
            var verification = new UserRegisterModel
            {
                Nombre = user.Nombre,
                Password =userRecover.Password,
                Correo=user.Correo
            };
            EncryptRegister(verification);
            user.Password = verification.Password;
            MailService.SendRestorePassMessage(userRecover.Correo,userRecover.Nombre,userRecover.Password);
            _writeOnlyRepository.Update(user);

            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session["User"] = "Anonymous";
            return RedirectToAction("Index");
        }
        private bool ValidateContact(ContactModel contact)
        {
            if (String.IsNullOrEmpty(contact.Correo) || String.IsNullOrEmpty(contact.Nombre) ||
                String.IsNullOrEmpty(contact.Mensaje))
            {
                this.AddNotification("Todos los campos son requeridos!", NotificationType.Error);
                return false;
            }
            var x = contact.Nombre.Replace(" ", string.Empty);
            if (x.Any(t => !Char.IsLetter(t)))
            {
                this.AddNotification("El Nombre solo puede llevar letras!", NotificationType.Error);
                return false;
            }
            if (contact.Nombre.Length < 3 || contact.Nombre.Length > 50)
            {
                this.AddNotification("El nombre debe tener mas de 3 caracteres y menos de 50!", NotificationType.Error);
                return false;
            }
            var y = contact.Mensaje.Split(null);
            var count = y.Length;
            if (count < 3 || contact.Mensaje.Length > 250)
            {
                this.AddNotification("La pregunta debe tener al menos 3 palabras y menos de 250 caracteres!",
                    NotificationType.Error);
                return false;
            }
            switch (ValidateEmail(contact.Correo))
            {
                case true:
                    break;
                case false:
                    this.AddNotification("Correo Eletrónico no valido!", NotificationType.Error);
                    return false;
            }
            return true;
        }
        private bool ValidateLogin(UserRegisterModel verification)
        {
            if (String.IsNullOrEmpty(verification.Correo) || String.IsNullOrEmpty(verification.Password))
            {
                this.AddNotification("Todos los campos son requeridos!", NotificationType.Error);
                return false;
            }
            switch (ValidateEmail(verification.Correo))
            {
                case true:
                    break;
                case false:
                    this.AddNotification("Correo Electrónico no valido!", NotificationType.Error);
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

        private bool ValidateNewUser(UserRegisterModel register)
        {
            if (string.IsNullOrEmpty(register.Nombre) || string.IsNullOrEmpty(register.Correo)
                || string.IsNullOrEmpty(register.Password) || string.IsNullOrEmpty(register.ConfirmPassword))
            {
                this.AddNotification("Todo los campos son requeridos!", NotificationType.Error);
                return false;
            }
            switch (ValidateEmail(register.Correo))
            {
                case true:
                    break;
                case false:
                    this.AddNotification("Correo Electrónico no valido!", NotificationType.Error);
                    return false;
            }
            if (register.Nombre.Length < 3 || register.Nombre.Length > 50)
            {
                this.AddNotification("El nombre debe tener mas de 3 caracteres y menos de 50!", NotificationType.Error);
                return false;
            }
            if (register.Password != register.ConfirmPassword)
            {
                this.AddNotification("Las contraseñas no coinciden!", NotificationType.Error);
                return false;
            }
            if (register.Password.Length < 8 || register.Password.Length > 20)
            {
                this.AddNotification("La contraseña debe tener mas de 8 caracteres y menos de 20!", NotificationType.Error);
                return false;
            }
            if (register.Password.Any(t => !Char.IsLetterOrDigit(t)))
            {
                this.AddNotification("La contraseña solo puede tener numeros y letras!", NotificationType.Error);
                return false;
            }
            return true;
        }
        private static void EncryptRegister(UserRegisterModel register)
        {
            var hashtool = SHA512.Create();
            var pass1 = hashtool.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
            var pass = BitConverter.ToString(pass1);
            var salt1 = hashtool.ComputeHash(Encoding.UTF8.GetBytes(register.Correo + register.Nombre));
            var salt = BitConverter.ToString(salt1);
            var pass2 = hashtool.ComputeHash(Encoding.UTF8.GetBytes(pass.Replace("-", "") + salt.Replace("-", "")));
            var passFinal = BitConverter.ToString(pass2);
            register.Password = passFinal.Replace("-", "");
            register.Salt = salt.Replace("-", "");
        }
    }
}