using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PocketTrainer.Models
{
    public class Sets : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public int RepsNumber
        {
            get => _repsNumber;
            set
            {
                _repsNumber = value;
                OnPropertyChanged("RepsNumber");
            } 
        }
        public float Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                OnPropertyChanged("Weight");
            } 
        }

        private int _repsNumber;
        private float _weight;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}