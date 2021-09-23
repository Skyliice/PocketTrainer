using System;
using System.IO;
using System.Threading.Tasks;
using PocketTrainer.Entities;
using PocketTrainer.Interfaces;
using SQLite;

namespace PocketTrainer
{
    public class LocalDB : ILocalData
    {
        public static LocalDB Instance { get; set; }
        public SQLiteAsyncConnection Db;

        public LocalDB()
        {
            Task.Run(InitializeConnection);
        }
        
        public LocalDB GetInstance()
        {
            if (Instance == null)
                return new LocalDB();
            else
                return Instance;

        }
        
        public async Task InitializeConnection()
        {
            Db = new SQLiteAsyncConnection(Path.Combine(
                Environment.GetFolderPath(Environment.
                    SpecialFolder.LocalApplicationData) 
                , "localdb.db"));
            await Db.CreateTableAsync<Log>();
            await Db.CreateTableAsync<Sets>();
            await Db.CreateTableAsync<WorkoutDay>();
            await Db.CreateTableAsync<WorkDayExJunction>();
        }
    }
}