using SQLite;

namespace PocketTrainer.Entities
{
    [Table(name:"WorkoutDay")]
    public class WorkoutDay
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Name { get; set; }
        public int WebID { get; set; }
    }
}