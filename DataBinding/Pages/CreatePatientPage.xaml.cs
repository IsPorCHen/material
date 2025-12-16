using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace DataBinding.Pages
{
    public partial class CreatePatientPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<Patient> _patients;
        private Patient _currentPatient;

        public Patient CurrentPatient => _currentPatient;

        public CreatePatientPage(ObservableCollection<Patient> patients)
        {
            _patients = patients;
            _currentPatient = new Patient
            {
                LastName = "",
                Name = "",
                MiddleName = "",
                Birthday = "",
                PhoneNumber = "",
                AppointmentStories = new ObservableCollection<Appointment>()
            };

            InitializeComponent();
            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            LastNameTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            NameTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            MiddleNameTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            BirthDateTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            PhoneTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            if (Validation.GetHasError(LastNameTextBox) ||
                Validation.GetHasError(NameTextBox) ||
                Validation.GetHasError(MiddleNameTextBox) ||
                Validation.GetHasError(BirthDateTextBox) ||
                Validation.GetHasError(PhoneTextBox))
            {
                MessageBox.Show("Исправьте ошибки валидации!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string lastName = CurrentPatient.LastName;
            string name = CurrentPatient.Name;
            string middleName = CurrentPatient.MiddleName;
            string birthday = CurrentPatient.Birthday;
            string phoneNumber = CurrentPatient.PhoneNumber;

            if (string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(middleName) ||
                string.IsNullOrWhiteSpace(birthday))
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!DateTime.TryParseExact(birthday, "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                MessageBox.Show("Неверный формат даты рождения! Используйте ДД.ММ.ГГГГ",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                string digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

                if (digits.Length != 10 && digits.Length != 11)
                {
                    MessageBox.Show("Неверный формат телефона! Должно быть 10 или 11 цифр",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (digits.Length == 11 && digits[0] != '7' && digits[0] != '8')
                {
                    MessageBox.Show("Номер телефона должен начинаться с 7 или 8",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (IsPatientExists(lastName, name, middleName, birthday))
            {
                MessageBox.Show("Пациент с такими данными уже существует!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _currentPatient.Id = GeneratePatientId();

            SavePatientToJson(_currentPatient);
            _patients.Add(_currentPatient);

            MessageBox.Show($"Пациент создан с ID: {_currentPatient.Id}", "Успешно",
                MessageBoxButton.OK, MessageBoxImage.Information);

            NavigationService?.GoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private int GeneratePatientId()
        {
            var random = new Random();
            int newId;

            do
            {
                newId = random.Next(1000000, 9999999);
            }
            while (File.Exists($"P_{newId}.json"));

            return newId;
        }

        private bool IsPatientExists(string lastName, string name, string middleName, string birthday)
        {
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "P_*.json"))
            {
                try
                {
                    string jsonString = File.ReadAllText(file, Encoding.UTF8);
                    var patientData = JsonSerializer.Deserialize<Patient>(jsonString);

                    if (patientData != null &&
                        patientData.LastName == lastName &&
                        patientData.Name == name &&
                        patientData.MiddleName == middleName &&
                        patientData.Birthday == birthday)
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

        private void SavePatientToJson(Patient patient)
        {
            try
            {
                var patientData = new
                {
                    patient.LastName,
                    patient.Name,
                    patient.MiddleName,
                    patient.Birthday,
                    patient.PhoneNumber,
                    AppointmentStories = patient.AppointmentStories ?? new ObservableCollection<Appointment>()
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string jsonString = JsonSerializer.Serialize(patientData, options);
                File.WriteAllText($"P_{patient.Id}.json", jsonString, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}