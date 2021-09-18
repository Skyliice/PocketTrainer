using System.Collections.Generic;
using Newtonsoft.Json;

namespace PocketTrainer.Models
{
    public class Workout
    {
        [JsonProperty(PropertyName = "ID")]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "ImagePath")]
        public string ImagePath { get; set; }
        [JsonProperty(PropertyName = "WorkoutDays")]
        public List<Models.WorkoutDay> WorkoutDays { get; set; }
    }
}