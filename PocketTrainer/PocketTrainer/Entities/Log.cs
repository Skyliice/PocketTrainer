using SQLite;

namespace PocketTrainer.Entities
{
    [Table(name:"Log")]
    public class Log
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        public int WorkoutDayID { get; set; }
        public string DateOfWorkout { get; set; }
    }
}