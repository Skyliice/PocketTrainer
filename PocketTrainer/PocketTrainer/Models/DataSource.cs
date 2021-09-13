using System.Collections.Generic;
using System.Threading.Tasks;
using PocketTrainer.Interfaces;

namespace PocketTrainer.Models
{
    public class DataSource
    {
        private static DataSource _instance;
        
        private IGatherable _gatherer;
        private List<MuscleGroup> _muscleGroups;
        private List<Exercise> _exercises;

        public DataSource()
        {
            _muscleGroups = new List<MuscleGroup>();
            _exercises = new List<Exercise>();
            _gatherer = new ApiGatherer();
        }

        public void SetValues()
        {
            _instance._muscleGroups = Task.Run(() => _gatherer.GetMuscleGroups()).Result;
            _instance._exercises = Task.Run(() => _gatherer.GetExercises()).Result;
        }

        public List<MuscleGroup> GetMuscleGroups()
        {
            if (_instance._muscleGroups != null)
                return _instance._muscleGroups;
            else 
                return new List<MuscleGroup>();
        }

        public List<Exercise> GetExercises()
        {
            if (_instance._exercises != null)
                return _instance._exercises;
            else 
                return new List<Exercise>();
        }

        public static DataSource GetInstance()
        {
            if (_instance == null)
                _instance = new DataSource();
            return _instance;
        }
    }
}