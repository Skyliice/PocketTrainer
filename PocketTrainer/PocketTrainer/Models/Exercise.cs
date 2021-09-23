using System.Collections.Generic;
using Newtonsoft.Json;
using PocketTrainer.Entities;

namespace PocketTrainer.Models
{
    public class Exercise
    {
        [JsonProperty(PropertyName = "ExerciseId")]
        public int ID { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Description { get; set; }
        [JsonProperty]
        public string ImagePath { get; set; }
        [JsonProperty]
        public List<MuscleGroup> MuscleGroups { get; set; } 
        public int Place { get; set; }
        public List<Sets> SetsList { get; set; }
    }
}