using System;
using System.Linq;
using System.Windows;

namespace DataBinding
{
    static class ThemeHelper
    {
        private static readonly string[] _themePath = {
            "Styles/Colors/LightTheme.xaml",
            "Styles/Colors/DarkTheme.xaml"
        };

        public static string Current
        {
            get => Properties.Settings.Default.ThemePath == ""
                ? _themePath[0]
                : Properties.Settings.Default.ThemePath;
            set
            {
                Properties.Settings.Default.ThemePath = value;
                Properties.Settings.Default.Save();
            }
        }

        public static void Apply(string themePath)
        {
            var newTheme = new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            };

            var oldTheme = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => _themePath.Any(path =>
                    d.Source != null && d.Source.OriginalString.Contains("Colors")));

            if (oldTheme != null)
            {
                int index = Application.Current.Resources.MergedDictionaries.IndexOf(oldTheme);
                Application.Current.Resources.MergedDictionaries[index] = newTheme;
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Insert(0, newTheme);
            }

            Current = themePath;
        }

        public static void ApplySaved()
        {
            var theme = Current;
            Apply(theme);
        }

        public static void Toggle()
        {
            var newTheme = Current == _themePath[0]
                ? _themePath[1]
                : _themePath[0];
            Apply(newTheme);
        }
    }
}
