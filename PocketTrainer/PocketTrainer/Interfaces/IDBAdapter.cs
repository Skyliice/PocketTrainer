using System.Collections.Generic;
using System.Threading.Tasks;
using PocketTrainer.Entities;
using PocketTrainer.Models;
using Sets = PocketTrainer.Entities.Sets;
using WorkoutDay = PocketTrainer.Models.WorkoutDay;

namespace PocketTrainer.Interfaces
{
    public interface IDBAdapter
    {
        public Task InitializeConnection();
        public Task AddNewExerciseToDay(string date,int selectedExerciseID);
        public Task AddNewWorkoutDayToDay(string date, List<Exercise> wDayExercises);
        public Task DeleteWorkoutDay(string date);
        public Task DeleteExerciseFromWorkoutDay(string date, int exercisePlace);
        public Task UpdateSetInfo(int setID, int repsNumber, float weight);

        public Task<List<Log>> GetLogs();
        public Task<List<Sets>> GetSets();
        public Task<List<WorkDayExJunction>> GetJunctions();
        public Task<List<Entities.WorkoutDay>> GetWDays();
        public Task AddSetToExercise(int selectedExerciseID,int exercisePlace,float weight,int repsNumber,string date);

        public Task<WorkDayExJunction> GetJunctionWithChildren(int juncID);

    }
}