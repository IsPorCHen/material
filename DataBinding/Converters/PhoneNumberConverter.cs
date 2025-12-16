using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DataBinding.Converters
{
    public class PhoneNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return "";

            string phone = value.ToString();

            string digits = new string(phone.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(digits))
                return "";

            if (digits.Length == 10)
                digits = "8" + digits;

            if (digits.StartsWith("7") && digits.Length == 11)
                digits = "8" + digits.Substring(1);

            if (digits.Length == 11)
            {
                try
                {
                    return string.Format("{0} ({1}) {2} {3} {4}",
                        digits.Substring(0, 1),
                        digits.Substring(1, 3),
                        digits.Substring(4, 3),
                        digits.Substring(7, 2),
                        digits.Substring(9, 2));
                }
                catch
                {
                    return phone;
                }
            }

            return phone;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            string phone = value.ToString();

            string digits = new string(phone.Where(char.IsDigit).ToArray());

            if (digits.Length == 11 && digits.StartsWith("8"))
            {
                digits = "7" + digits.Substring(1);
            }

            return digits;
        }
    }
}