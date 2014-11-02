using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Clasificados.Extensions;
using Clasificados.Models;
using Domain.Entities;
using Domain.Services;
using FluentNHibernate.Conventions;

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

        
        public ActionResult NewClassified()
        {
            return View(new ClassifiedModel());
        }
        [HttpPost]
        public ActionResult NewClassified(ClassifiedModel clasificado)
        {
            switch(ValidateClasificado(clasificado))
            {
                case true:
                    break;
                case false:
                    return View(clasificado);
            }
            var user = (string) Session["User"];
            var usuario = _readOnlyRepository.FirstOrDefault<User>(x => x.Nombre == user);
            clasificado.IdUsuario = usuario.Id;
            var classified = new Classified
            {
                Titulo = clasificado.Titulo,
                Categoria = clasificado.Categoria,
                IdUsuario = clasificado.IdUsuario,
                Negocio = clasificado.Negocio,
                Descripcion = clasificado.Descripcion,
                Precio = clasificado.Precio,
                UrlVideo = clasificado.UrlVideo
            };
           
            _writeOnlyRepository.Create(classified);
            var cls = _readOnlyRepository.FirstOrDefault<Classified>(x => x.Titulo == classified.Titulo);
            clasificado.IdClasificado = cls.Id;
            
            if (!String.IsNullOrEmpty(clasificado.UrlImg0))
            {
                var clsimage = new ClassifiedImage
                {
                    IdClassified = clasificado.IdClasificado,
                    UrlIamgen = clasificado.UrlImg0,
                };
                _writeOnlyRepository.Create(clsimage);
            }
            if (!String.IsNullOrEmpty(clasificado.UrlImg1))
            {
                var clsimage = new ClassifiedImage
                {
                    IdClassified = clasificado.IdClasificado,
                    UrlIamgen = clasificado.UrlImg1,
                };
                _writeOnlyRepository.Create(clsimage);
            }
            if (!String.IsNullOrEmpty(clasificado.UrlImg2))
            {
                var clsimage = new ClassifiedImage
                {
                    IdClassified = clasificado.IdClasificado,
                    UrlIamgen = clasificado.UrlImg2,
                };
                _writeOnlyRepository.Create(clsimage);
            }
            if (!String.IsNullOrEmpty(clasificado.UrlImg3))
            {
                var clsimage = new ClassifiedImage
                {
                    IdClassified = clasificado.IdClasificado,
                    UrlIamgen = clasificado.UrlImg3,
                };
                _writeOnlyRepository.Create(clsimage);
            }
            if (!String.IsNullOrEmpty(clasificado.UrlImg4))
            {
                var clsimage = new ClassifiedImage
                {
                    IdClassified = clasificado.IdClasificado,
                    UrlIamgen = clasificado.UrlImg4,
                };
                _writeOnlyRepository.Create(clsimage);
            }
            if (!String.IsNullOrEmpty(clasificado.UrlImg5))
            {
                var clsimage = new ClassifiedImage
                {
                    IdClassified = clasificado.IdClasificado,
                    UrlIamgen = clasificado.UrlImg5,
                };
                _writeOnlyRepository.Create(clsimage);
            }

            this.AddNotification("Clasificado registrado.",NotificationType.Success);
            return View(clasificado);
        }

        

        public ActionResult UserProfile()
        {
            return View();
        }
        private bool ValidateClasificado(ClassifiedModel clasificado)
        {
            if (String.IsNullOrEmpty(clasificado.Titulo) || String.IsNullOrEmpty(clasificado.Categoria) ||
                String.IsNullOrEmpty(clasificado.Precio) || String.IsNullOrEmpty(clasificado.Negocio) ||
                String.IsNullOrEmpty(clasificado.Descripcion))
            {
                this.AddNotification("Los campos requeridos son Titulo, Categoria, Precio, Negocio y Descripcion!", NotificationType.Warning);
                return false;
            }
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
                        this.AddNotification("Url de imagenes no existe o no es valido!", NotificationType.Error);
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
                        this.AddNotification("Url de imagenes no existe o no es valido!", NotificationType.Error);
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
                        this.AddNotification("Url de imagenes no existe o no es valido!", NotificationType.Error);
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
                        this.AddNotification("Url de imagenes no existe o no es valido!", NotificationType.Error);
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
                        this.AddNotification("Url de imagenes no existe o no es valido!", NotificationType.Error);
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
                        this.AddNotification("Url de imagenes no existe o no es valido!", NotificationType.Error);
                        return false;
                }
            }
            
            if (clasificado.Titulo.Length < 5 || clasificado.Titulo.Length > 100)
            {
                this.AddNotification("Titulo debe tener al menos una palabra y menos de 100 caracteres!", NotificationType.Error);
                return false;
            }

            if (clasificado.Precio.Any(t => !Char.IsDigit(t)))
            {
                this.AddNotification("Valor del precio solo puede ser un valor exacto y numerico!", NotificationType.Error);
                return false;
            }
            var y = clasificado.Descripcion.Split(null);
            var count = y.Length;
            if (count < 3 || clasificado.Descripcion.Length > 255)
            {
                this.AddNotification("La descripcion debe tener al menos 3 palabras y menos de 250 caracteres!",
                    NotificationType.Error);
                return false;
            }
            return true;
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
    }
}