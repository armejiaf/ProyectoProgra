using System.ComponentModel.DataAnnotations;

namespace Clasificados.ValidationAttributes
{
    public class TitleValidationAttribute:ValidationAttribute

    {
        public int MinimumAmountOfWords { get; set; }
        public int MaximumAmountOfCharacters { get; set; }

        public TitleValidationAttribute()
        {
            MinimumAmountOfWords = 1;
            MaximumAmountOfCharacters = 50;
        }
        public override bool IsValid(object value)
        {
            var strValue = (string) value;
            if (strValue == null)
            {
                return false;
            }
            var wordsInStrValue = strValue.Split(' '); 
            return wordsInStrValue.Length >= MinimumAmountOfWords && strValue.Length <= MaximumAmountOfCharacters;
        }
    }
}
