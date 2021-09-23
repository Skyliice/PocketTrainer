using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PocketTrainer.Entities
{
    [Table("WorkDayExJunction")]
    public class WorkDayExJunction
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        public int Place { get; set; }
        public int WorkoutDayID { get; set; }
        public int ExerciseID { get; set; }
        
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Sets> SetsList { get; set; } 
    }
}