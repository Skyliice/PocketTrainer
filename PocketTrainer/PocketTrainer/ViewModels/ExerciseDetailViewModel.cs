using System.Linq;
using PocketTrainer.Models;
using Xamarin.Forms;

namespace PocketTrainer.ViewModels
{
    public class ExerciseDetailViewModel
    {
        public Exercise SelectedExercise { get; set; }
        public INavigation Navigation;

        public ExerciseDetailViewModel(Exercise _pickedExercise)
        {
            SelectedExercise = _pickedExercise;
            var dsource = DataSource.GetInstance();
            foreach (var selectedExerciseMuscleGroup in SelectedExercise.MuscleGroups)
            {
                selectedExerciseMuscleGroup.Name =
                    dsource.GetMuscleGroups().First(o => o.ID == selectedExerciseMuscleGroup.ID).Name;
            }    
        }
    }
}