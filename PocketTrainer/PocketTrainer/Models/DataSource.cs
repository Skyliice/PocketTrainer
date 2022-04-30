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
        private List<Workout> _workouts;
        private List<WorkoutDay> _workoutDays;

        public DataSource()
        {
            _muscleGroups = new List<MuscleGroup>();
            _exercises = new List<Exercise>();
            _workouts = new List<Workout>();
            _workoutDays = new List<WorkoutDay>();
            _gatherer = new ApiGatherer();
        }
        
        public static DataSource GetInstance()
        {
            if (_instance == null)
                _instance = new DataSource();
            return _instance;
        }

        public void SetValues()
        {
            _instance._muscleGroups = Task.Run(() => _gatherer.GetMuscleGroups()).Result;
            _instance._exercises = Task.Run(() => _gatherer.GetExercises()).Result;
            _instance._workouts = Task.Run(() => _gatherer.GetWorkouts()).Result;
            _instance._workoutDays = Task.Run(() => _gatherer.GetWorkoutDays()).Result;
            foreach (var wDay in _instance._workoutDays)
            {
                foreach (var wDayExercise in wDay.Exercises)
                {
                    wDayExercise.FillSetsList();
                }
            }
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

        public List<Workout> GetWorkouts()
        {
            if (_instance._workouts != null)
                return _instance._workouts;
            else 
                return new List<Workout>();
        }
        
        public List<WorkoutDay> GetWorkoutDays()
        {
            if (_instance._exercises != null)
                return _instance._workoutDays;
            else 
                return new List<WorkoutDay>();
        }
    }
}