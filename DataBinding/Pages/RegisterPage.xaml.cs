using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace DataBinding.Pages
{
    public partial class RegisterPage : Page, INotifyPropertyChanged
    {
        private string _lastName = "";
        private string _name = "";
        private string _middleName = "";
        private string _specialisation = "";

        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string MiddleName
        {
            get => _middleName;
            set { _middleName = value; OnPropertyChanged(); }
        }

        public string Specialisation
        {
            get => _specialisation;
            set { _specialisation = value; OnPropertyChanged(); }
        }

        public RegisterPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(LastNameTextBox) ||
                Validation.GetHasError(NameTextBox) ||
                Validation.GetHasError(MiddleNameTextBox) ||
                Validation.GetHasError(SpecialisationTextBox))
            {
                MessageBox.Show("Исправьте ошибки валидации!");
                return;
            }

            string lastName = LastNameTextBox.Text;
            string name = NameTextBox.Text;
            string middleName = MiddleNameTextBox.Text;
            string specialisation = SpecialisationTextBox.Text;

            if (string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(middleName) ||
                string.IsNullOrWhiteSpace(specialisation))
            {
                MessageBox.Show("Все поля обязательны для заполнения!");
                return;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            if (PasswordBox.Password.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать минимум 4 символа!");
                return;
            }

            if (IsDoctorExists(lastName, name, middleName))
            {
                MessageBox.Show("Пользователь с такими ФИО уже существует!");
                return;
            }

            var random = new Random();
            int newId;
            bool idExists;
            do
            {
                newId = random.Next(10000, 99999);
                idExists = File.Exists($"D_{newId}.json");
            } while (idExists);

            var newDoctor = new Doctor
            {
                Id = newId,
                LastName = lastName,
                Name = name,
                MiddleName = middleName,
                Specialisation = specialisation,
                Password = PasswordBox.Password
            };

            SaveDoctorToJson(newDoctor);

            MessageBox.Show($"Доктор зарегистрирован с ID: {newDoctor.Id}");
            NavigationService.GoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private bool IsDoctorExists(string lastName, string name, string middleName)
        {
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "D_*.json"))
            {
                try
                {
                    string jsonString = File.ReadAllText(file, Encoding.UTF8);
                    var doctorData = JsonSerializer.Deserialize<Doctor>(jsonString);

                    if (doctorData != null &&
                        doctorData.LastName == lastName &&
                        doctorData.Name == name &&
                        doctorData.MiddleName == middleName)
                    {
                        return true;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return false;
        }

        private void SaveDoctorToJson(Doctor doctor)
        {
            var doctorData = new
            {
                doctor.Id,
                doctor.LastName,
                doctor.Name,
                doctor.MiddleName,
                doctor.Specialisation,
                doctor.Password
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string jsonString = JsonSerializer.Serialize(doctorData, options);
            string fileName = $"D_{doctor.Id}.json";
            File.WriteAllText(fileName, jsonString, Encoding.UTF8);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
