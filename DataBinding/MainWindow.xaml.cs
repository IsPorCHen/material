using DataBinding.Pages;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DataBinding
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            MainFrame.Navigate(new LoginPage());
            UpdateSystemInfo();
            MainFrame.Navigated += (s, e) => UpdateSystemInfo();
        }

        private int _doctorsCount;
        public int DoctorsCount
        {
            get => _doctorsCount;
            set
            {
                if (_doctorsCount != value)
                {
                    _doctorsCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _patientsCount;
        public int PatientsCount
        {
            get => _patientsCount;
            set
            {
                if (_patientsCount != value)
                {
                    _patientsCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateSystemInfo()
        {
            var doctorFiles = Directory.GetFiles(".", "D_*.json");
            var patientFiles = Directory.GetFiles(".", "P_*.json");
            DoctorsCount = doctorFiles.Length;
            PatientsCount = patientFiles.Length;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeHelper.Toggle();
        }
    }
}
