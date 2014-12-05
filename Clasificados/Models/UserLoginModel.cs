using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Clasificados.Models
{
    public class UserLoginModel
    {
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Correo es requerido.")]
        [DisplayName("Correo")]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessage = "Correo no es un correo valido.")]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Contraseña es requerida.")]
        [DisplayName("Contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Salt { get; set; }
        
    }
}