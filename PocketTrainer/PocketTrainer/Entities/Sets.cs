using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace PocketTrainer.Entities
{
    [Table(name:"Sets")]
    public class Sets
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        public int RepsNumber { get; set; }
        [ForeignKey(typeof(WorkDayExJunction))]
        public int WorkDayExJunctionID { get; set; }
        public float Weight { get; set; }
        [ManyToOne]
        public WorkDayExJunction WorkDayExJunction { get; set; }
        
    }
}