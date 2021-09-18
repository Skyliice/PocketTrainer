using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PocketTrainer.Models;
using PocketTrainer.Views;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace PocketTrainer.ViewModels
{
    public class WorkoutDayPopupViewModel
    {
        public List<WorkoutDay> CurrentWorkoutDays { get; set; }
        public INavigation Navigation { get; set; }
        public ICommand BackCommand { get; protected set; }
        public IAsyncCommand<WorkoutDay> TappedItem { get; protected set; }
        public WorkoutDayPopupViewModel(Workout _currentWorkout)
        {
            CurrentWorkoutDays = new List<WorkoutDay>();
            TappedItem = new AsyncCommand<WorkoutDay>(GoNext);
            BackCommand = new Command(GoBack);
            var dsource = DataSource.GetInstance();
            CurrentWorkoutDays = dsource.GetWorkoutDays().Where(o => _currentWorkout.WorkoutDays.Any(x => x.ID == o.ID))
                .ToList();
        }

        private async Task GoNext(WorkoutDay pickedWorkoutDay)
        {
            await Navigation.PushPopupAsync(new ExerciseListPopupView(pickedWorkoutDay));
        }
        private async void GoBack()
        {
            await Navigation.PopPopupAsync();
        }
    }
}