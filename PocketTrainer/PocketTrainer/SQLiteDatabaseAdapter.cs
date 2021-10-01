using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PocketTrainer.Entities;
using PocketTrainer.Interfaces;
using PocketTrainer.Models;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
using Sets = PocketTrainer.Entities.Sets;
using WorkoutDay = PocketTrainer.Entities.WorkoutDay;

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
            else
            {
                var wDay = await Db.FindAsync<WorkoutDay>(o => o.ID==currentLog.WorkoutDayID);
                AddExerciseWDayJunction(wDay.ID,selectedExerciseID);
            }
        }

        public async Task AddNewWorkoutDayToDay(string date, List<Exercise> wDayExercises)
        {
            var curlog = await GetLogByDate(date);
            var wDay = new WorkoutDay();
            if (curlog == null)
            {
                wDay = await CreateWorkoutDay();
                await CreateLog(date, wDay.ID);
            }
            else
            {
                wDay=await Db.FindAsync<WorkoutDay>(o => o.ID==curlog.WorkoutDayID);
            }
            var count = 1;
            foreach (var wDayExercise in wDayExercises)
            {
                AddExerciseWDayJunction(wDay.ID, wDayExercise.ID);
                for (int i = 0; i < wDayExercise.SetsList.Count; i++)
                {
                    await AddSetToExercise(wDayExercise.ID, count, wDayExercise.SetsList[i].Weight,
                        wDayExercise.SetsList[i].RepsNumber, date);
                }
                count++;
            }
        }

        public async Task DeleteWorkoutDay(string date)
        {
            var curlog = await GetLogByDate(date);
            if (curlog != null)
            {
                var wDay = (await GetWDays()).Find(o=>o.ID==curlog.WorkoutDayID);
                var juncts = (await GetJunctions()).Where(o => o.WorkoutDayID == wDay.ID);
                foreach (var junction in juncts)
                {
                    var sets = (await GetSets()).Where(o => o.WorkDayExJunctionID == junction.ID);
                    foreach (var set in sets)
                    {
                        await Db.DeleteAsync(set);
                    }
                    await Db.DeleteAsync(junction);
                }
                await Db.DeleteAsync(wDay);
                await Db.DeleteAsync(curlog);
            }
        }

        public async Task DeleteExerciseFromWorkoutDay(string date, int exercisePlace)
        {
            var curlog = await GetLogByDate(date);
            if (curlog != null)
            {
                var wDay = (await GetWDays()).Find(o=>o.ID==curlog.WorkoutDayID);
                var juncts = (await GetJunctions()).Where(o => o.WorkoutDayID == wDay.ID).ToList();
                var exerciseJunction = juncts.First(o => o.Place == exercisePlace);
                var sets = (await GetSets()).Where(o => o.WorkDayExJunctionID == exerciseJunction.ID);
                foreach (var set in sets)
                {
                    await Db.DeleteAsync(set);
                }
                await Db.DeleteAsync(exerciseJunction);
                juncts = juncts.Where(o => o.ID != exerciseJunction.ID).ToList();
                if (juncts.Count == 0)
                {
                    await DeleteWorkoutDay(date);
                    return;
                }
                juncts.Where(o=>o.Place>exercisePlace).ToList().ForEach(z=>z.Place -= 1);
                await Db.UpdateAllAsync(juncts);
                
            }
        }

        public async Task UpdateSetInfo(int setID, int repsNumber,float weight)
        {
            var selectedSet = (await GetSets()).Find(o => o.ID == setID);
            selectedSet.Weight = weight;
            selectedSet.RepsNumber = repsNumber;
            await Db.UpdateAsync(selectedSet);
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

        public async Task AddSetToExercise(int selectedExerciseID,int exercisePlace,float weight,int repsNumber,string date)
        {
            var lg = await GetLogByDate(date);
            var juncts = (await GetJunctions()).Find(o =>
                o.WorkoutDayID == lg.WorkoutDayID && o.Place == exercisePlace && o.ExerciseID == selectedExerciseID);
            var set = new Sets() {RepsNumber = repsNumber, Weight = weight, WorkDayExJunctionID = juncts.ID};
            await Db.InsertAsync(set);
            set.WorkDayExJunction = juncts;
            await Db.UpdateWithChildrenAsync(set);
            var lbs = await Db.Table<Sets>().ToListAsync();
        }

        public async Task<WorkDayExJunction> GetJunctionWithChildren(int juncID)
        {
            return await Db.GetWithChildrenAsync<WorkDayExJunction>(juncID);
        }

        private async void AddExerciseWDayJunction(int workoutDayID, int selectedExerciseID)
        {
            var lst = await Db.Table<WorkDayExJunction>().Where(o => o.WorkoutDayID == workoutDayID).ToListAsync();
            var junction = new WorkDayExJunction()
                {Place = lst.Count + 1, ExerciseID = selectedExerciseID, WorkoutDayID = workoutDayID};
            await Db.InsertAsync(junction);
            junction.WorkoutDay = await Db.Table<WorkoutDay>().FirstAsync(o => o.ID == workoutDayID);
            junction.SetsList = new List<Sets>();
            await Db.UpdateWithChildrenAsync(junction);
        }

        private async Task<Log> CreateLog(string date,int wDayID)
        {
            var lg = new Log() {DateOfWorkout = date, WorkoutDayID = wDayID};
            await Db.InsertAsync(lg);
            lg.WorkoutDay = await Db.Table<WorkoutDay>().FirstAsync(o => o.ID == wDayID);
            await Db.UpdateWithChildrenAsync(lg);
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