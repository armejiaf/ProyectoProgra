using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Clasificados.ValidationAttributes;
using Domain.Entities;

namespace Clasificados.Models
{
    public class QuestionModel
    {
        [Required(ErrorMessage = "Su correo es requerido!")]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessage = "Correo no es un correo valido.")]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Su nombre es requerido!")]
        [RegularExpression("[a-zA-Z _]**", ErrorMessage = "El nombre solo puede tener letras!")]
        [StringLength(50, ErrorMessage = "El nombre debe tener mas de 3 caracteres y menos de 50!", MinimumLength = 3)]
        [DataType(DataType.Text)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La pregunta es requerida!")]
        [DescriptionValidation(MinimumAmountOfWords = 3, MaximumAmountOfCharacters = 250, ErrorMessage = "La pregunta debe tener al menos 3 palabras y menos de 250 caracteres!")]
        [DataType(DataType.MultilineText)]
        public string Pregunta { get; set; }

        public List<QuestionAnswer> PreguntasFrecuentes { get; set; } 
    }
}