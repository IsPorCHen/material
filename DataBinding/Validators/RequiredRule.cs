using System.Globalization;
using System.Windows.Controls;

namespace DataBinding.Validators
{
    public class RequiredRule : ValidationRule
    {
        public string ErrorMessage { get; set; } = "Поле обязательно для заполнения";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? strValue = value as string;

            if (string.IsNullOrWhiteSpace(strValue))
            {
                return new ValidationResult(false, ErrorMessage);
            }

            return ValidationResult.ValidResult;
        }
    }
}
