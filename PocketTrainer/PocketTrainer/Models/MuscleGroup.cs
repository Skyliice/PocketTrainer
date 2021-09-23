using Newtonsoft.Json;

namespace PocketTrainer.Models
{
    public class MuscleGroup
    {
        [JsonProperty(PropertyName = "MuscleGroupID")]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "ImagePath")]
        public string ImagePath { get; set; }
    }
}