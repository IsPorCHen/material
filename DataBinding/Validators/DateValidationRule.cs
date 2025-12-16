using System;
using System.Globalization;
using System.Windows.Controls;

namespace DataBinding.Validators
{
    public class DateValidationRule : ValidationRule
    {
        public string ErrorMessage { get; set; } = "Некорректный формат даты. Используйте ДД.ММ.ГГГГ";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? strValue = value as string;

            if (string.IsNullOrWhiteSpace(strValue))
            {
                return ValidationResult.ValidResult;
            }

            if (!DateTime.TryParseExact(strValue, "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return new ValidationResult(false, ErrorMessage);
            }

            return ValidationResult.ValidResult;
        }
    }
}
