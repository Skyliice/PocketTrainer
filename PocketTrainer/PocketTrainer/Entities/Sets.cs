using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace PocketTrainer.Entities
{
    [Table(name:"Sets")]
    public class Sets : INotifyPropertyChanged
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        public int ExercisePlace { get; set; }
        public int RepsNumber { get; set; }
        public int WorkoutDayID { get; set; }
        public float Weight { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}