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
    public partial class EditPatientPage : Page, INotifyPropertyChanged
    {
        private Patient _currentPatient;
        private ObservableCollection<Patient> _patients;

        public Patient CurrentPatient => _currentPatient;

        public EditPatientPage(Patient patient, ObservableCollection<Patient> patients)
        {
            _currentPatient = patient ?? throw new ArgumentNullException(nameof(patient));
            _patients = patients ?? throw new ArgumentNullException(nameof(patients));

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

            try
            {
                SavePatientToJson(_currentPatient);

                var index = _patients.IndexOf(_currentPatient);
                if (index >= 0)
                {
                    _patients[index] = _currentPatient;
                }

                MessageBox.Show("Информация о пациенте обновлена!", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
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
                string fileName = $"P_{patient.Id}.json";
                File.WriteAllText(fileName, jsonString, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось сохранить пациента: {ex.Message}", ex);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}