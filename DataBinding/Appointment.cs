using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DataBinding
{
    public class Appointment : INotifyPropertyChanged
    {
        private string _date = "";
        private int _doctorId;
        private string _diagnosis = "";
        private string _recommendations = "";

        public string Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        public int DoctorId
        {
            get => _doctorId;
            set { _doctorId = value; OnPropertyChanged(); }
        }

        public string Diagnosis
        {
            get => _diagnosis;
            set { _diagnosis = value; OnPropertyChanged(); }
        }

        public string Recommendations
        {
            get => _recommendations;
            set { _recommendations = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
