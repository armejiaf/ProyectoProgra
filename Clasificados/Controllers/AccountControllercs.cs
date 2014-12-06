using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using Clasificados.Extensions;
using Clasificados.Mail;
using Clasificados.Models;
using Clasificados.Twilio;
using Domain.Entities;
using Domain.Services;
using NHibernate.Hql.Ast.ANTLR;

namespace Clasificados.Controllers
{
    public class AccountController:Controller
    {
        readonly IWriteOnlyRepository _writeOnlyRepository;
        readonly IReadOnlyRepository _readOnlyRepository;

        public AccountController(IWriteOnlyRepository writeOnlyRepository, IReadOnlyRepository readOnlyRepository)
        {
            _writeOnlyRepository = writeOnlyRepository;
            _readOnlyRepository = readOnlyRepository;
        }

        public ActionResult EditClasificado(long id)
        {
            if ((string)Session["User"] == "Anonymous")
            {
                this.AddNotification("No se puede realizar esta accion. Primero ingrese sesion", NotificationType.Info);
                return RedirectToAction("Login");
            }
            var clasificado = _readOnlyRepository.GetById<Classified>(id);
            var edit = new EditClassiffiedModel
            {
                IdClasificado = clasificado.Id,
            };
            return View(edit);
        }
        [HttpPost]
        public ActionResult EditClasificado(EditClassiffiedModel edit)
        {
            var clasificado = _readOnlyRepository.GetById<Classified>(edit.IdClasificado);
            if(edit.Negocio!=null)
                clasificado.Negocio = edit.Negocio;
            if(edit.Precio!=null)
                clasificado.Precio = edit.Precio;
            if(edit.Titulo!=null)
                clasificado.Titulo = edit.Titulo;
            if(edit.Descripcion!=null)
                clasificado.Descripcion = edit.Descripcion;
            if (edit.Categoria != null)
                clasificado.Categoria = edit.Categoria;
            _writeOnlyRepository.Update(clasificado);
            return RedirectToAction("UserProfile");
        }
        public ActionResult NewClassified()
        {
            if ((string) Session["User"] == "Anonymous")
            {
                this.AddNotification("Porfavor ingrese sesion antes de crear clasificado",NotificationType.Info);
                return RedirectToAction("Login");
            }
            var user = _readOnlyRepository.FirstOrDefault<User>(x => x.Nombre == (string) Session["User"]);
            if (user.Miembro == false && user.TotalClasificados >= 5 && (string)Session["Role"] == "user")
            {
                this.AddNotification("Ha llegado a su limite de creacion. Si le agrado nuestra pagina hagase miembro.", NotificationType.Info);
                return RedirectToAction("Index", "Home");
            }
            return View(new ClassifiedModel());
        }
        [HttpPost]
        public ActionResult NewClassified(ClassifiedModel clasificado)
        {
            if (ModelState.IsValid)
            {
                switch (ValidateImagesVideo(clasificado))
                {
                    case true:
                        break;
                    case false:
                        return View(clasificado);
                }
                var user = (string)Session["User"];
                var usuario = _readOnlyRepository.FirstOrDefault<User>(x => x.Nombre == user);
                clasificado.IdUsuario = usuario.Id;
                var classified = new Classified
                {
                    FechaCreacion = DateTime.Now.ToString("d"),
                    Titulo = clasificado.Titulo,
                    Categoria = clasificado.Categoria,
                    IdUsuario = clasificado.IdUsuario,
                    Negocio = clasificado.Negocio,
                    Descripcion = clasificado.Descripcion,
                    Precio = clasificado.Precio,
                    UrlVideo = clasificado.UrlVideo,
                    UrlImg0 = clasificado.UrlImg0,
                    UrlImg1 = clasificado.UrlImg1,
                    UrlImg2 = clasificado.UrlImg2,
                    UrlImg3 = clasificado.UrlImg3,
                    UrlImg4 = clasificado.UrlImg4,
                    UrlImg5 = clasificado.UrlImg5,
                    Recomendado = 1
                };

                _writeOnlyRepository.Create(classified);
                usuario.TotalClasificados += 1;
                _writeOnlyRepository.Update(usuario);
                var subscriptions = _readOnlyRepository.GetAll<Suscribtions>().ToList();
                foreach (var sus in subscriptions)
                {
                    var subs = _readOnlyRepository.GetById<User>(sus.IdUsuarioSuscrito);
                    TwilioService.SendSmsToSubscribers(subs.Nombre, classified.Titulo,usuario.Nombre);
                }
                
                this.AddNotification("Clasificado registrado.", NotificationType.Success);
                return RedirectToAction("Index","Home");
            }
            this.AddNotification("No se pudo crear clasificado.", NotificationType.Error);
            return View(clasificado);
            
        }

       
        public ActionResult UserProfile()
        {

            if ((string) Session["User"] != "Anonymous")
            {
                var user = _readOnlyRepository.FirstOrDefault<User>(x => x.Nombre == (string) Session["User"]);
                var clasificados = _readOnlyRepository.GetAll<Classified>().ToList();
                var clasif = clasificados.Where(x => x.IdUsuario == user.Id && x.Archived==false && x.DesactivadoPorAdmin==false).ToList();
                var clsarchive = clasificados.Where(x => x.IdUsuario == user.Id && x.Archived && x.DesactivadoPorAdmin==false).ToList();
                var profile = new UserProfileModel
                {
                    UserId = user.Id,
                    Correo = user.Correo,
                    Nombre = user.Nombre,
                    Clasificados = clasif,
                    ClasificadosArchive = clsarchive
                    
                };
                return View(profile);

            }
            this.AddNotification("Ingrese sesion para poder ver su perfil.",NotificationType.Info);
            return RedirectToAction("Login");
        }
     
        public ActionResult Register()
        {
            return View(new UserRegisterModel());
        }

        [HttpPost]
        public ActionResult Register(UserRegisterModel register)
        {
            if (ModelState.IsValid)
            {
                var usuario = new User();

                var user = _readOnlyRepository.FirstOrDefault<User>(x => x.Correo == register.Correo);
                if (user != null)
                {
                    this.AddNotification("Usuario ya existe!", NotificationType.Warning);
                    return View(register);
                }
                MailService.SendGreetingMessage(register.Correo, register.Nombre, register.Password);
                EncryptRegister(register);
                usuario.Nombre = register.Nombre;
                usuario.Correo = register.Correo;
                usuario.Password = register.Password;
                usuario.Role = "user";
                usuario.Salt = register.Salt;
                _writeOnlyRepository.Create(usuario);
                this.AddNotification("Se ha registrado exitosamente.", NotificationType.Success);
                return RedirectToAction("Login");     
            }
            this.AddNotification("Errores en el formulario.", NotificationType.Error);
            return View(register);
        }

        public ActionResult Login()
        {
            return View(new UserLoginModel());
        }

        [HttpPost]
        public ActionResult Login(UserLoginModel login, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var usuario = _readOnlyRepository.FirstOrDefault<User>(x => x.Correo == login.Correo);
                if (usuario == null)
                {
                    this.AddNotification("Usuario no Existe!", NotificationType.Error);
                    return View(login);
                }

             

                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                           && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }

                var verification = new UserRegisterModel
                {
                    Nombre = usuario.Nombre,
                    Password = login.Password,
                    Correo = login.Correo,
                };
            
                EncryptRegister(verification);
                login.Password = verification.Password;
                if (usuario.Password != login.Password)
                {
                    this.AddNotification("Usuario o Contraseña incorrectos!", NotificationType.Error);
                    return View(login);
                }
                Session["User"] = usuario.Nombre;
                Session["Role"] = usuario.Role;
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [Authorize]
        public ActionResult RecoverPassword()
        {
            return View(new UserRecoverPasswordModel());
        }

        [HttpPost]
        public ActionResult RecoverPassword(UserRecoverPasswordModel userRecover)
        {
            if (ModelState.IsValid)
            {
                var user = _readOnlyRepository.FirstOrDefault<User>(x => x.Correo == userRecover.Correo);
                if (user == null)
                {
                    this.AddNotification("Correo Electrónico incorrecto!", NotificationType.Error);
                    return View(userRecover);
                }
                userRecover.Password = Guid.NewGuid().ToString().Substring(0, 8);
                userRecover.Nombre = user.Nombre;
                var verification = new UserRegisterModel
                {
                    Nombre = user.Nombre,
                    Password = userRecover.Password,
                    Correo = user.Correo
                };
                EncryptRegister(verification);
                user.Password = verification.Password;
                MailService.SendRestorePassMessage(userRecover.Correo, userRecover.Nombre, userRecover.Password);
                _writeOnlyRepository.Update(user);

                this.AddNotification("SMS y Correo Enviado.", NotificationType.Success);
                return RedirectToAction("Login");
            }
            this.AddNotification("Correo Electrónico incorrecto!", NotificationType.Error);
            return View(userRecover);
        }

        public ActionResult Logout()
        {
            Session["User"] = "Anonymous";
            return RedirectToAction("Index","Home");
        }

        private bool ValidateImagesVideo(ClassifiedModel clasificado)
        {
            if (clasificado.UrlVideo != null)
            {
                switch (RemoteFileExists(clasificado.UrlVideo))
                {
                    case true:
                        break;
                    case false:
                        this.AddNotification("Url de Video no existe o no es valido!", NotificationType.Error);
                        return false;
                }
            }
            if (!String.IsNullOrEmpty(clasificado.UrlImg0))
            {
                switch (RemoteFileExists(clasificado.UrlImg0))
                {
                    case true:
                        break;
                    case false:
                        this.AddNotification("Url de imagen 0 no existe o no es valido!", NotificationType.Error);
                        return false;
                }
            }
            if (!String.IsNullOrEmpty(clasificado.UrlImg1))
            {
                switch (RemoteFileExists(clasificado.UrlImg1))
                {
                    case true:
                        break;
                    case false:
                        this.AddNotification("Url de imagen 1 no existe o no es valido!", NotificationType.Error);
                        return false;
                }
            }
            if (!String.IsNullOrEmpty(clasificado.UrlImg2))
            {
                switch (RemoteFileExists(clasificado.UrlImg2))
                {
                    case true:
                        break;
                    case false:
                        this.AddNotification("Url de imagen 2 no existe o no es valido!", NotificationType.Error);
                        return false;
                }
            }
            if (!String.IsNullOrEmpty(clasificado.UrlImg3))
            {
                switch (RemoteFileExists(clasificado.UrlImg3))
                {
                    case true:
                        break;
                    case false:
                        this.AddNotification("Url de imagen 4 no existe o no es valido!", NotificationType.Error);
                        return false;
                }
            }
            if (!String.IsNullOrEmpty(clasificado.UrlImg4))
            {
                switch (RemoteFileExists(clasificado.UrlImg4))
                {
                    case true:
                        break;
                    case false:
                        this.AddNotification("Url de imagen 4 no existe o no es valido!", NotificationType.Error);
                        return false;
                }
            }
            if (!String.IsNullOrEmpty(clasificado.UrlImg5))
            {
                switch (RemoteFileExists(clasificado.UrlImg5))
                {
                    case true:
                        break;
                    case false:
                        this.AddNotification("Url de imagen 5 no existe o no es valido!", NotificationType.Error);
                        return false;
                }
            }

            return true;
        }

        private static bool RemoteFileExists(string url)
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

        public ActionResult ArchiveClasificado(long id)
        {
            if ((string)Session["User"] == "Anonymous")
            {
                this.AddNotification("Porfavor ingrese sesion!", NotificationType.Info);
                return RedirectToAction("Login");
            }
            var clasificado = _readOnlyRepository.GetById<Classified>(id);
            clasificado.Archive();
            _writeOnlyRepository.Update(clasificado);
            return RedirectToAction("UserProfile");
        }

        public ActionResult RestoreClasificado(long id)
        {
            if ((string)Session["User"] == "Anonymous")
            {
                this.AddNotification("Porfavor ingrese sesion!", NotificationType.Info);
                return RedirectToAction("Login");
            }
            var clasificado = _readOnlyRepository.GetById<Classified>(id);
            clasificado.Activate();
            _writeOnlyRepository.Update(clasificado);
            return RedirectToAction("UserProfile");
        }

        public ActionResult VerPerfil(long id)
        {
            
            var usuario = _readOnlyRepository.GetById<User>(id);
            if (usuario.Nombre == null)
            {
                this.AddNotification("No existe usuario.",NotificationType.Info);
                return RedirectToAction("Index", "Home");
            }
            var clasificados = _readOnlyRepository.GetAll<Classified>().Where(x => x.IdUsuario == id &&
                x.Archived==false && x.DesactivadoPorAdmin==false).ToList();
            var perfil = new VerPerfilModel
            {
                Usuario = usuario,
                Clasificados = clasificados
            };


            return View(perfil);
        }

        public ActionResult Suscribe(long id)
        {
            var userclas = _readOnlyRepository.GetById<User>(id);
            var usersus = _readOnlyRepository.FirstOrDefault<User>(x => x.Nombre == (string) Session["User"]);
            if (userclas.Id == usersus.Id)
            {
                this.AddNotification("No se puede suscribir a su propio usuario.", NotificationType.Info);
                return RedirectToAction("VerPerfil", new{id});
            }
            var suscribe = _readOnlyRepository.GetAll<Suscribtions>().ToList();
            if (suscribe.Any(check => check.IdUsuarioClasificado == userclas.Id && check.IdUsuarioSuscrito == usersus.Id))
            {
                this.AddNotification("Ya esta suscrito.",NotificationType.Info);
                return RedirectToAction("Index", "Home");
            }
            var sus = new Suscribtions
            {
                IdUsuarioClasificado = userclas.Id,
                IdUsuarioSuscrito = usersus.Id
            };
            _writeOnlyRepository.Create(sus);
            this.AddNotification("Se ha suscrito.",NotificationType.Info);
            return RedirectToAction("VerPerfil", new {id});

        }
    }
}