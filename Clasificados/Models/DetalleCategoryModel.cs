using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Clasificados.ValidationAttributes;
using Domain.Entities;

namespace Clasificados.Models
{
    public class DetalleCategoryModel
    {
        public long IdClasificado { get; set; }
        public  bool Archived { get; set; }
        public  string FechaCreacion { get; set; }
        public  long IdUsuario { get; set; }
        public  string Titulo { get; set; }
        public  string Categoria { get; set; }
        public  string Descripcion { get; set; }
        public  string Precio { get; set; }
        public  string Negocio { get; set; }
        public  string UrlImg0 { get; set; }
        public  string UrlImg1 { get; set; }
        public  string UrlImg2 { get; set; }
        public  string UrlImg3 { get; set; }
        public  string UrlImg4 { get; set; }
        public  string UrlImg5 { get; set; }
        public  string UrlVideo { get; set; }
        public  int Visitas { get; set; }
        public  int Recomendado { get; set; }
        public User Usuario { get; set; }

        [Required(ErrorMessage = "Su nombre es requerido!")]
        [RegularExpression("[a-zA-Z]*", ErrorMessage = "El nombre solo puede tener letras!")]
        [StringLength(50, ErrorMessage = "El nombre debe tener mas de 3 caracteres y menos de 50!", MinimumLength = 3)]
        [DataType(DataType.Text)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Su correo es requerido!")]
        [RegularExpression("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", ErrorMessage = "Correo no es un correo valido.")]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El mensaje es requerido!")]
        [DescriptionValidation(MinimumAmountOfWords = 3, MaximumAmountOfCharacters = 250, ErrorMessage = "El mensaje debe tener al menos 3 palabras y menos de 250 caracteres!")]
        [DataType(DataType.MultilineText)]
        public string Mensaje { get; set; }
    }
}