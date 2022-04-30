using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PocketTrainer.Entities
{
    [Table(name:"Log")]
    public class Log
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        [ForeignKey(typeof(WorkoutDay))]
        public int WorkoutDayID { get; set; }
        public string DateOfWorkout { get; set; }
        
        [OneToOne]
        public WorkoutDay WorkoutDay { get; set; }
    }
}