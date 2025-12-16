using System;
using System.Globalization;
using System.Windows.Data;

namespace DataBinding.Converters
{
    public class DaysSinceLastVisitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "Первый прием в клинике";

            if (value is int days)
            {
                string dayWord = GetDayWord(days);
                return $"{days} {dayWord} с предыдущего приема";
            }

            return "Первый прием в клинике";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetDayWord(int days)
        {
            int lastDigit = days % 10;
            int lastTwoDigits = days % 100;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 19)
                return "дней";

            if (lastDigit == 1)
                return "день";

            if (lastDigit >= 2 && lastDigit <= 4)
                return "дня";

            return "дней";
        }
    }
}
