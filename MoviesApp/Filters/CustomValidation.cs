using System.ComponentModel.DataAnnotations;

namespace MoviesApp.Filters
{
    public class CustomValidation : ValidationAttribute
    {
        private int _length;
        public CustomValidation(int length)
        {
            _length = length;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value.ToString()?.Length < _length)
            {
                return new ValidationResult($"The length name need to be more than 3");
            }

            return ValidationResult.Success;
        }
    }
}