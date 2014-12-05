using System.ComponentModel.DataAnnotations;

namespace Clasificados.Models
{
    public class UserRegisterModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Nombre es requerido!")]
        [StringLength(50, ErrorMessage = "El nombre debe tener mas de 3 caracteres y menos de 50!", MinimumLength = 3)]
        [DataType(DataType.Text)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Correo es requerido!")]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessage = "Correo no es un correo valido.")]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Contraseña es requerida!")]
        [StringLength(20, ErrorMessage = "La contraseña debe tener mas de 8 caracteres y menos de 20!", MinimumLength = 8)]
        [Compare("ConfirmPassword", ErrorMessage = "Contraseñas deben ser iguales.")]
        [RegularExpression("[a-zA-Z0-9]*", ErrorMessage = "La contraseña solo puede tener numeros y letras!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmar contraseña es requerido!")]
        [StringLength(20, ErrorMessage = "La contraseña debe tener mas de 8 caracteres y menos de 20!", MinimumLength = 8)]
        [Compare("Password", ErrorMessage = "Contraseñas deben ser iguales.")]
        [RegularExpression("[a-zA-Z0-9]*", ErrorMessage = "La contraseña solo puede tener numeros y letras!")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
       
        public string Salt { get; set; }
    }
}