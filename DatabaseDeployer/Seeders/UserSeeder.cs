using System;
using System.Security.Cryptography;
using System.Text;
using Domain.Entities;
using DomainDrivenDatabaseDeployer;
using FizzWare.NBuilder;
using NHibernate;

namespace DatabaseDeployer.Seeders
{
    class UserSeeder:IDataSeeder
    {
        readonly ISession _session;
        public UserSeeder(ISession session)
        {
            _session = session;
        }
        public void Seed()
        {
            var usuario = Builder<User>.CreateNew().Build();
            usuario.Nombre = "Allan Mejia";
            usuario.Password = "armf1993";
            usuario.Correo = "almesiete@gmail.com";
            usuario.Role = "admin";
            usuario.Miembro = true;
            EncryptUser(usuario);
            _session.Save(usuario);
        }
        private static void EncryptUser(User register)
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
