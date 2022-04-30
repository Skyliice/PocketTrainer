using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using PocketTrainer.Entities;

namespace PocketTrainer.Models
{
    public class Exercise : INotifyPropertyChanged
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
        [JsonProperty]
        public int SetNumber { get; set; }
        [JsonProperty]
        public int RepsNumber { get; set; }
        public int Place { get; set; }
        public List<Sets> SetsList
        {
            get => _setsList;
            set
            {
                _setsList = value;
                OnPropertyChanged("SetsList");
            } 
        }
        private List<Sets> _setsList;

        public void FillSetsList()
        {
            SetsList = new List<Sets>();
            for (int i = 0; i < SetNumber; i++)
            {
                SetsList.Add(new Sets(){RepsNumber = this.RepsNumber, Weight = 0});
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}