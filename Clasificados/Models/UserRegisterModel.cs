namespace Clasificados.Models
{
    public class UserRegisterModel
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Salt { get; set; }
    }
}