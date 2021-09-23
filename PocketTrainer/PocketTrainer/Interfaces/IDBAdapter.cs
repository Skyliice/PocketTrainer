using System.Collections.Generic;
using System.Threading.Tasks;
using PocketTrainer.Entities;
using WorkoutDay = PocketTrainer.Models.WorkoutDay;

namespace PocketTrainer.Interfaces
{
    public interface IDBAdapter
    {
        public Task InitializeConnection();
        public Task AddNewExerciseToDay(string date,int selectedExerciseID);

        public Task<List<Log>> GetLogs();
        public Task<List<Sets>> GetSets();
        public Task<List<WorkDayExJunction>> GetJunctions();
        public Task<List<Entities.WorkoutDay>> GetWDays();

    }
}