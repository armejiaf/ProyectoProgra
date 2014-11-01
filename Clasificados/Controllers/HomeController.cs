﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Windows.Forms;
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
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
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
                MessageBox.Show("Usuario ya existe!");
                return View(register);
            }
            MailService.SendGreetingMessage(register.Correo,register.Nombre,register.Password);
            EncryptRegister(register);
            usuario.Nombre = register.Nombre;
            usuario.Correo = register.Correo;
            usuario.Password = register.Password;
            usuario.Salt = register.Salt;
            _writeOnlyRepository.Create(usuario);
            MessageBox.Show("Se ha registrado exitosamente.");
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
                MessageBox.Show("Correo Electrónico incorrecto!");
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
                MessageBox.Show("Usuario o Contraseña incorrectos!");
                return View();
            }

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
                MessageBox.Show("Correo Electrónico incorrecto!");
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

        private static bool ValidateLogin(UserRegisterModel verification)
        {
            if (String.IsNullOrEmpty(verification.Correo) || String.IsNullOrEmpty(verification.Password))
            {
                MessageBox.Show("Todos los campos son requeridos!");
                return false;
            }
            switch (ValidateEmail(verification.Correo))
            {
                case true:
                    break;
                case false:
                    MessageBox.Show("Correo Electrónico no valido!");
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

        private static bool ValidateNewUser(UserRegisterModel register)
        {
            if (string.IsNullOrEmpty(register.Nombre) || string.IsNullOrEmpty(register.Correo)
                || string.IsNullOrEmpty(register.Password) || string.IsNullOrEmpty(register.ConfirmPassword))
            {
                MessageBox.Show("Todo los campos son requeridos!");
                return false;
            }
            switch (ValidateEmail(register.Correo))
            {
                case true:
                    break;
                case false:
                    MessageBox.Show("Correo Electrónico no valido!");
                    return false;
            }
            if (register.Nombre.Length < 3 || register.Nombre.Length > 50)
            {
                MessageBox.Show("El nombre de tener mas de 3 caracteres y menos de 50!");
                return false;
            }
            if (register.Password != register.ConfirmPassword)
            {
                MessageBox.Show("Las contraseñas no coinciden!");
                return false;
            }
            if (register.Password.Length < 8 || register.Password.Length > 20)
            {
                MessageBox.Show("La contraseña debe tener mas de 8 caracteres y menos de 20!");
                return false;
            }
            if (register.Password.Any(t => !Char.IsLetterOrDigit(t)))
            {
                MessageBox.Show("La contraseña solo puede tener numeros y letras!");
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