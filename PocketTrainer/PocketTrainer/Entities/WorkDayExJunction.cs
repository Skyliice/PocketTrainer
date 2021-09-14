using SQLite;

namespace PocketTrainer.Entities
{
    [Table("WorkDayExJunction")]
    public class WorkDayExJunction
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        public int Place { get; set; }
        public int WorkoutDayID { get; set; }
        public int ExcerciseID { get; set; }
    }
}