using System.ComponentModel.DataAnnotations;
using Clasificados.ValidationAttributes;

namespace Clasificados.Models
{
    public class ClassifiedModel
    {
        public long IdClasificado { get; set; }
        public long IdUsuario { get; set; }

        [Required(ErrorMessage = "Titulo es requerido.")]
        [DataType(DataType.Text)]
        [TitleValidation(MinimumAmountOfWords = 1, MaximumAmountOfCharacters = 100, ErrorMessage = "Titulo debe tener al menos una palabra y menos de 100 caracteres!")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Categoria es requerida.")]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "Descripcion es requerida.")]
        [DescriptionValidation(MinimumAmountOfWords = 3, MaximumAmountOfCharacters = 255, ErrorMessage = "La descripcion debe tener al menos 3 palabras y menos de 250 caracteres!")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Precio es requerido.")]
        [Range(1, 1000000.00, ErrorMessage = "El precio debe ser entre 1 y 1,000,000.00")]
        [DataType(DataType.Currency)]
        public string Precio { get; set; }

        [Required(ErrorMessage = "Negocio es requerido.")]
        public string Negocio { get; set; }

        public string UrlImg0 { get; set; }

        public string UrlImg1 { get; set; }

        public string UrlImg2 { get; set; }

        public string UrlImg3 { get; set; }

        public string UrlImg4 { get; set; }

        public string UrlImg5 { get; set; }

        public string UrlVideo { get; set; }
    }
}