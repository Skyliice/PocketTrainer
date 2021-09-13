using System.Collections.Generic;
using System.Threading.Tasks;
using PocketTrainer.Models;
using PocketTrainer.Views;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace PocketTrainer.ViewModels
{
    public class MuscleGroupsViewModel
    {
        public List<MuscleGroup> MuscleGroups { get; set; }
        public INavigation Navigation;
        public IAsyncCommand<MuscleGroup> TappedItem { get; protected set; }

        public MuscleGroupsViewModel()
        {
            TappedItem = new AsyncCommand<MuscleGroup>(GoToNextPage);
            MuscleGroups = new List<MuscleGroup>();
            var dsource = DataSource.GetInstance();
            MuscleGroups = dsource.GetMuscleGroups();
        }

        private async Task GoToNextPage(MuscleGroup pickedMuscleGroup)
        {
            await Navigation.PushAsync(new ExerciseListView(pickedMuscleGroup.ID));
        }
    }
}