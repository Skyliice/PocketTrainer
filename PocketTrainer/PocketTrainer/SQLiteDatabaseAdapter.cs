using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PocketTrainer.Entities;
using PocketTrainer.Interfaces;
using SQLite;

namespace PocketTrainer
{
    public class SQLiteDatabaseAdapter : IDBAdapter
    {
        private SQLiteAsyncConnection Db { get; set; }

        public async Task InitializeConnection()
        {
            Db = new SQLiteAsyncConnection(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                , "localdb.db"));
            await Db.CreateTableAsync<Log>();
            await Db.CreateTableAsync<Sets>();
            await Db.CreateTableAsync<WorkoutDay>();
            await Db.CreateTableAsync<WorkDayExJunction>();
        }

        public async Task AddNewExerciseToDay(string date, int selectedExerciseID)
        {
            var currentLog = await GetLogByDate(date);
            if (currentLog==null)
            {
                var wDay = await CreateWorkoutDay();
                await CreateLog(date, wDay.ID);
                AddExerciseWDayJunction(wDay.ID,selectedExerciseID);
            }
        }

        public async Task<List<Log>> GetLogs()
        {
            var logs= Db.Table<Log>();
            if (logs != null)
            {
                var lgs = await logs.ToListAsync();
                return lgs;
            }
            return new List<Log>();
        }

        public async Task<List<Sets>> GetSets()
        {
            return await Db.Table<Sets>().ToListAsync();
        }

        public async Task<List<WorkDayExJunction>> GetJunctions()
        {
            return await Db.Table<WorkDayExJunction>().ToListAsync();
        }

        public async Task<List<WorkoutDay>> GetWDays()
        {
            return await Db.Table<WorkoutDay>().ToListAsync();
        }

        private async void AddExerciseWDayJunction(int workoutDayID, int selectedExerciseID)
        {
            var lst = await Db.Table<WorkDayExJunction>().Where(o => o.WorkoutDayID == workoutDayID).ToListAsync();
            await Db.InsertAsync(new WorkDayExJunction()
                {Place = lst.Count + 1, ExerciseID = selectedExerciseID, WorkoutDayID = workoutDayID});
        }

        private async Task<Log> CreateLog(string date,int wDayID)
        {
            var lg = new Log() {DateOfWorkout = date, WorkoutDayID = wDayID};
            await Db.InsertAsync(lg);
            return lg;
        }

        private async Task<WorkoutDay> CreateWorkoutDay()
        {
            var rnd = new Random();
            var wDay = new WorkoutDay() {Name = "localWDay", ID = rnd.Next(0, 1000000)};
            await Db.InsertOrReplaceAsync(wDay);
            return wDay;
        }

        private async Task<Log> GetLogByDate(string date)
        {
            var lst=await Db.Table<Log>().Where(o => o.DateOfWorkout == date).ToListAsync();
            if (lst.Count == 0)
                return null;
            else
                return lst.First();
        }
    }
}