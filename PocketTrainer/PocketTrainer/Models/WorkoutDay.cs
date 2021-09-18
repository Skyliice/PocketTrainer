using System.Collections.Generic;
using Newtonsoft.Json;

namespace PocketTrainer.Models
{
    public class WorkoutDay
    {
        [JsonProperty(PropertyName = "WorkoutDayID")]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "Exercises")]
        public List<Exercise> Exercises { get; set; }
    }
}