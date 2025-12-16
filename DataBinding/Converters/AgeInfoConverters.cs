using System;
using System.Globalization;
using System.Windows.Data;

namespace DataBinding.Converters
{
    public class AgeInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? birthDate = null;

            if (value is DateTime dt)
            {
                birthDate = dt;
            }
            else if (value is string dateString && !string.IsNullOrEmpty(dateString))
            {
                if (DateTime.TryParseExact(dateString, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
                {
                    birthDate = parsed;
                }
            }

            if (!birthDate.HasValue)
                return "Возраст неизвестен";

            var today = DateTime.Today;
            int age = today.Year - birthDate.Value.Year;

            if (birthDate.Value.Date > today.AddYears(-age))
                age--;

            bool isAdult = age >= 18;
            string adultStatus = isAdult ? "совершеннолетний" : "несовершеннолетний";

            string ageWord = GetAgeWord(age);

            return $"{age} {ageWord} ({adultStatus})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetAgeWord(int age)
        {
            int lastDigit = age % 10;
            int lastTwoDigits = age % 100;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 19)
                return "лет";

            if (lastDigit == 1)
                return "год";

            if (lastDigit >= 2 && lastDigit <= 4)
                return "года";

            return "лет";
        }
    }
}
