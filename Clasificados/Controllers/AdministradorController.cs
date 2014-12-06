using System.Linq;
using System.Web.Mvc;
using Clasificados.Extensions;
using Clasificados.Models;
using Domain.Entities;
using Domain.Services;

namespace Clasificados.Controllers
{
    public class AdministradorController:Controller
    {
        readonly IWriteOnlyRepository _writeOnlyRepository;
        readonly IReadOnlyRepository _readOnlyRepository;

        public AdministradorController(IWriteOnlyRepository writeOnlyRepository, IReadOnlyRepository readOnlyRepository)
        {
            _writeOnlyRepository = writeOnlyRepository;
            _readOnlyRepository = readOnlyRepository;
        }

        public ActionResult Administrar()
        {
            if ((string) Session["User"] != "Anonymous" && (string) Session["Role"] == "admin")
            {
                var clasificados = _readOnlyRepository.GetAll<Classified>().ToList();
                var clasif = clasificados.Where(x => x.DesactivadoPorAdmin == false).ToList();
                var clasifar = clasificados.Where(x => x.DesactivadoPorAdmin).ToList();
                var preguntas = _readOnlyRepository.GetAll<QuestionAnswer>().ToList();
                var preg = preguntas.Where(x => x.Archived == false).ToList();
                var pregar = preguntas.Where(x => x.Archived).ToList();
                var preguntasUsuarios = _readOnlyRepository.GetAll<ContactUserInfo>().ToList();
                var preguser = preguntasUsuarios;
                var admin = new AdminModel
                {
                    Clasificados = clasif,
                    ClasificadosDesactivados = clasifar,
                    Preguntas = preg,
                    PreguntasDesactivadas = pregar,
                    PreguntasUsuarios = preguser,
              
                };
                return View(admin);
            }
            this.AddNotification("Pagina no existe!", NotificationType.Error);
            return RedirectToAction("Login","Account");
        }

        public ActionResult DesactivarClasificado(long id)
        {
            if ((string)Session["User"] == "Anonymous")
            {
                this.AddNotification("Pagina no Existe!", NotificationType.Error);
                return RedirectToAction("Login","Account");
            }
            var clasificado = _readOnlyRepository.GetById<Classified>(id);
            clasificado.DesactivadoPorAdmin=true;
            _writeOnlyRepository.Update(clasificado);
            return RedirectToAction("Administrar");
        }

        public ActionResult ActivarClasificado(long id)
        {
            if ((string)Session["User"] == "Anonymous")
            {
                this.AddNotification("Pagina no Existe!", NotificationType.Error);
                return RedirectToAction("Login","Account");
            }
            var clasificado = _readOnlyRepository.GetById<Classified>(id);
            clasificado.DesactivadoPorAdmin = false;
            _writeOnlyRepository.Update(clasificado);
            return RedirectToAction("Administrar");
        }

        public ActionResult ArchivarPregunta(long id)
        {
            if ((string)Session["User"] == "Anonymous")
            {
                this.AddNotification("Pagina no Existe!", NotificationType.Error);
                return RedirectToAction("Login", "Account");
            }
            var clasificado = _readOnlyRepository.GetById<QuestionAnswer>(id);
            clasificado.Archive();
            _writeOnlyRepository.Update(clasificado);
            return RedirectToAction("Administrar");
        }
    }
}