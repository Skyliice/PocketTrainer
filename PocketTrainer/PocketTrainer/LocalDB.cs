using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PocketTrainer.Entities;
using PocketTrainer.Interfaces;
using PocketTrainer.Models;
using SQLite;
using Sets = PocketTrainer.Entities.Sets;
using WorkoutDay = PocketTrainer.Entities.WorkoutDay;

namespace PocketTrainer
{
    public class LocalDB
    {
        public static LocalDB Instance { get; set; }
        private IDBAdapter _dbAdapter { get; set; }
        
        public Exercise SelectedExercise { get; set; }
        public EventWaitHandle WaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        public LocalDB()
        {
            _dbAdapter = new SQLiteDatabaseAdapter();
            _dbAdapter.InitializeConnection();
        }
        
        public static LocalDB GetInstance()
        {
            if (Instance == null)
                Instance = new LocalDB();
            return Instance;

        }

        public async Task AddExerciseToWDay(string date)
        {
            await _dbAdapter.AddNewExerciseToDay(date,SelectedExercise.ID);
        }
        
        public async Task<List<Log>> GetLogs()
        {
            return await _dbAdapter.GetLogs();
        }

        public async Task<List<Sets>> GetSets()
        {
            return await _dbAdapter.GetSets();
        }

        public async Task<List<WorkDayExJunction>> GetJunctions()
        {
            return await _dbAdapter.GetJunctions();
        }

        public async Task<List<WorkoutDay>> GetWDays()
        {
            return await _dbAdapter.GetWDays();
        }
        
    }
}