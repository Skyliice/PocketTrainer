using System.Collections.Generic;
using System.Threading.Tasks;
using PocketTrainer.Models;

namespace PocketTrainer.Interfaces
{
    public interface IGatherable
    {
        public Task<List<MuscleGroup>> GetMuscleGroups();
        public Task<List<Exercise>> GetExercises();
        public Task<List<Workout>> GetWorkouts();
        public Task<List<WorkoutDay>> GetWorkoutDays();

    }
}