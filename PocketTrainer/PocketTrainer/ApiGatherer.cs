using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PocketTrainer.Interfaces;
using PocketTrainer.Models;

namespace PocketTrainer
{
    public class ApiGatherer : IGatherable
    {
        private readonly string url = "https://skylice.ru/pocket-trainer-api/";
        
        public async Task<List<MuscleGroup>> GetMuscleGroups()
        {
            var client = new HttpClient();
            string resultjson = await client.GetStringAsync(url+"getmusclegroups.php");
            var rslt = JsonConvert.DeserializeObject<List<MuscleGroup>>(resultjson);
            return rslt;
        }

        public async Task<List<Exercise>> GetExercises()
        {
            var client = new HttpClient();
            string resultjson = await client.GetStringAsync(url+"getexercises.php");
            var rslt = JsonConvert.DeserializeObject<List<Exercise>>(resultjson);
            return rslt;
        }
    }
}