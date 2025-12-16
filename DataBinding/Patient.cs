using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace DataBinding
{
    public class Patient : INotifyPropertyChanged
    {
        private string _lastName = "";
        private string _name = "";
        private string _middleName = "";
        private string _birthday = "";
        private string _phoneNumber = "";
        private ObservableCollection<Appointment> _appointmentStories = new ObservableCollection<Appointment>();

        public int Id { get; set; }

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

        public string Birthday
        {
            get => _birthday;
            set 
            { 
                _birthday = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(BirthDate));
                OnPropertyChanged(nameof(Age));
                OnPropertyChanged(nameof(AdultStatus));
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Appointment> AppointmentStories
        {
            get => _appointmentStories;
            set 
            { 
                _appointmentStories = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(DaysSinceLastAppointment));
            }
        }
        public DateTime? BirthDate
        {
            get
            {
                if (DateTime.TryParseExact(_birthday, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
                return null;
            }
        }

        public int Age
        {
            get
            {
                if (!BirthDate.HasValue) return 0;
                
                var today = DateTime.Today;
                var birthDate = BirthDate.Value;
                int age = today.Year - birthDate.Year;
                
                if (birthDate.Date > today.AddYears(-age))
                    age--;
                
                return age;
            }
        }

        public string AdultStatus
        {
            get => Age >= 18 ? "совершеннолетний" : "несовершеннолетний";
        }

        public DateTime? LastVisitDate
        {
            get
            {
                if (_appointmentStories == null || _appointmentStories.Count == 0)
                    return null;

                DateTime? lastDate = null;
                foreach (var appointment in _appointmentStories)
                {
                    if (DateTime.TryParseExact(appointment.Date, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime appointmentDate))
                    {
                        if (!lastDate.HasValue || appointmentDate > lastDate.Value)
                            lastDate = appointmentDate;
                    }
                }
                return lastDate;
            }
        }

        public string DaysSinceLastAppointment
        {
            get
            {
                if (!LastVisitDate.HasValue)
                    return "Первый прием в клинике";

                int days = (DateTime.Today - LastVisitDate.Value).Days;
                string dayWord = GetDayWord(days);
                return $"{days} {dayWord} с предыдущего приема";
            }
        }

        public void NotifyAppointmentChanged()
        {
            OnPropertyChanged(nameof(LastVisitDate));
            OnPropertyChanged(nameof(DaysSinceLastAppointment));
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
