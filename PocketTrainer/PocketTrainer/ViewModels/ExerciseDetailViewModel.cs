using System.Linq;
using System.Windows.Input;
using PocketTrainer.Models;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace PocketTrainer.ViewModels
{
    public class ExerciseDetailViewModel
    {
        public Exercise SelectedExercise { get; set; }
        public INavigation Navigation;
        public ICommand BackCommand { get; protected set; }

        public ExerciseDetailViewModel(Exercise _pickedExercise)
        {
            SelectedExercise = _pickedExercise;
            var dsource = DataSource.GetInstance();
            BackCommand = new Command(GoBack);
            foreach (var selectedExerciseMuscleGroup in SelectedExercise.MuscleGroups)
            {
                selectedExerciseMuscleGroup.Name =
                    dsource.GetMuscleGroups().First(o => o.ID == selectedExerciseMuscleGroup.ID).Name;
            }    
        }
        
        private async void GoBack()
        {
            await Navigation.PopPopupAsync();
        }
    }
}