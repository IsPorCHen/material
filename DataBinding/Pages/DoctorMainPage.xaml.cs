using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace DataBinding.Pages
{
    public partial class DoctorMainPage : Page, INotifyPropertyChanged
    {
        public Doctor CurrentDoctor { get; set; }
        public ObservableCollection<Patient> Patients { get; set; }
        public Patient SelectedPatient { get; set; }

        public DoctorMainPage(Doctor doctor)
        {
            InitializeComponent();
            CurrentDoctor = doctor;
            Patients = new ObservableCollection<Patient>();
            LoadPatients();
            DataContext = this;
        }

        private void LoadPatients()
        {
            Patients.Clear();
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "P_*.json"))
            {
                try
                {
                    string jsonString = File.ReadAllText(file, Encoding.UTF8);
                    var patientData = JsonSerializer.Deserialize<Patient>(jsonString);

                    if (patientData != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file);
                        patientData.Id = int.Parse(fileName.Substring(2));
                        Patients.Add(patientData);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        private void CreatePatientButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreatePatientPage(Patients));
        }

        private void StartAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
            {
                MessageBox.Show("Выберите пациента для начала приема!");
                return;
            }
            NavigationService.Navigate(new AppointmentPage(SelectedPatient, CurrentDoctor, Patients));
        }

        private void EditInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
            {
                MessageBox.Show("Выберите пациента для изменения информации!");
                return;
            }
            NavigationService.Navigate(new EditPatientPage(SelectedPatient, Patients));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
            {
                MessageBox.Show("Пользователь не выбран");
                return;
            }

            bool confirm = MessageBox.Show(
                "Вы действительно хотите удалить запись?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                ) == MessageBoxResult.Yes;

            if (confirm)
            {
                string fileName = $"P_{SelectedPatient.Id}.json";

                if (File.Exists(fileName))
                {
                    try
                    {
                        File.Delete(fileName);
                        Patients.Remove(SelectedPatient);
                        MessageBox.Show("Пациент успешно удален!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении файла: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show($"Файл пациента не найден: {fileName}");
                    Patients.Remove(SelectedPatient);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
