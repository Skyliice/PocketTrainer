using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PocketTrainer.Entities
{
    [Table(name:"WorkoutDay")]
    public class WorkoutDay
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Name { get; set; }
        public int WebID { get; set; }
        
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Log Log { get; set; }
        
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<WorkDayExJunction> WorkDayExJunctions { get; set; }
    }
}