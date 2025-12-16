using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Globalization;

namespace DataBinding.Pages
{
    public partial class AppointmentPage : Page, INotifyPropertyChanged
    {
        private Patient _currentPatient;
        private Doctor _currentDoctor;
        private ObservableCollection<Patient> _patients;
        private string _diagnosisText = "";
        private string _recommendationsText = "";
        private string _appointmentDate;

        public Patient CurrentPatient => _currentPatient;
        public Doctor CurrentDoctor => _currentDoctor;

        public string DiagnosisText
        {
            get => _diagnosisText;
            set { _diagnosisText = value; OnPropertyChanged(nameof(DiagnosisText)); }
        }

        public string RecommendationsText
        {
            get => _recommendationsText;
            set { _recommendationsText = value; OnPropertyChanged(nameof(RecommendationsText)); }
        }

        public string AppointmentDate
        {
            get => _appointmentDate;
            set
            {
                _appointmentDate = value;
                OnPropertyChanged(nameof(AppointmentDate));
            }
        }

        public DateTime? AppointmentDateTime
        {
            get
            {
                if (DateTime.TryParseExact(_appointmentDate, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
                return null;
            }
        }

        public AppointmentPage(Patient patient, Doctor doctor, ObservableCollection<Patient> patients)
        {
            InitializeComponent();
            _currentPatient = patient;
            _currentDoctor = doctor;
            _patients = patients;

            AppointmentDate = DateTime.Now.ToString("dd.MM.yyyy");

            DataContext = this;
        }

        private void SaveAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DiagnosisText))
            {
                MessageBox.Show("Заполните диагноз!");
                return;
            }

            if (!DateTime.TryParseExact(AppointmentDate, "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                MessageBox.Show("Некорректный формат даты! Используйте формат ДД.ММ.ГГГГ");
                return;
            }

            var newAppointment = new Appointment
            {
                Date = AppointmentDate,
                DoctorId = CurrentDoctor.Id,
                Diagnosis = DiagnosisText,
                Recommendations = RecommendationsText
            };

            _currentPatient.AppointmentStories.Add(newAppointment);

            _currentPatient.NotifyAppointmentChanged();

            SavePatientToJson(_currentPatient);

            MessageBox.Show("Прием сохранен!");

            DiagnosisText = "";
            RecommendationsText = "";
            AppointmentDate = DateTime.Now.ToString("dd.MM.yyyy");

            OnPropertyChanged(nameof(CurrentPatient));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void SavePatientToJson(Patient patient)
        {
            var patientData = new
            {
                patient.LastName,
                patient.Name,
                patient.MiddleName,
                patient.Birthday,
                patient.PhoneNumber,
                AppointmentStories = patient.AppointmentStories
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}