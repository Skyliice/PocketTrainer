using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace PocketTrainer.Entities
{
    [Table(name:"Sets")]
    public class Sets
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        public int ExercisePlace { get; set; }
        public int RepsNumber { get; set; }
        public int WorkoutDayID { get; set; }
        public float Weight { get; set; }
        
    }
}