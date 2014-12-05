using System.ComponentModel.DataAnnotations;

namespace Clasificados.Models
{
    public class UserRecoverPasswordModel
    {
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Correo es requerido")]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessage = "Correo no es un correo valido.")]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }
        public string Password { get; set; }
    }
}