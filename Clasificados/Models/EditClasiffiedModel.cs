using System.ComponentModel.DataAnnotations;
using Clasificados.ValidationAttributes;

namespace Clasificados.Models
{
    public class EditClassiffiedModel
    {
        public long IdClasificado { get; set; }

        [DataType(DataType.Text)]
        [TitleValidation(MinimumAmountOfWords = 1, MaximumAmountOfCharacters = 100, ErrorMessage = "Titulo debe tener al menos una palabra y menos de 100 caracteres!")]
        public string Titulo { get; set; }

        public string Categoria { get; set; }

        [DescriptionValidation(MinimumAmountOfWords = 3, MaximumAmountOfCharacters = 255, ErrorMessage = "La descripcion debe tener al menos 3 palabras y menos de 250 caracteres!")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Range(1, 1000000.00, ErrorMessage = "El precio debe ser entre 1 y 1,000,000.00")]
        [DataType(DataType.Currency)]
        public string Precio { get; set; }

        public string Negocio { get; set; }

    }
}