using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace DataBinding.Validators
{
    public class PhoneValidationRule : ValidationRule
    {
        public string ErrorMessage { get; set; } = "Введите корректный формат телефона: 8 (900) 000 00 00";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? strValue = value as string;

            if (string.IsNullOrWhiteSpace(strValue))
            {
                return ValidationResult.ValidResult;
            }

            string digits = new string(strValue.Where(char.IsDigit).ToArray());

            if (digits.Length == 0)
            {
                return ValidationResult.ValidResult;
            }

            if (digits.Length >= 10 && digits.Length <= 11)
            {
                if (digits.Length == 11)
                {
                    if (digits[0] != '7' && digits[0] != '8')
                    {
                        return new ValidationResult(false, "Номер должен начинаться с 7 или 8");
                    }
                }
                return ValidationResult.ValidResult;
            }

            if (digits.Length < 10)
            {
                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, ErrorMessage);
        }
    }
}