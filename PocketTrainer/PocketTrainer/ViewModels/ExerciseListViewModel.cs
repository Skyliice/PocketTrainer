using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using PocketTrainer.Models;
using PocketTrainer.Views;
using Rg.Plugins.Popup.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace PocketTrainer.ViewModels
{
    public class ExerciseListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Exercise> Exercises
        {
            get => _exercises;
            set
            {
                _exercises = value;
                OnPropertyChanged("Exercises");
            } 
        }
        public INavigation Navigation;
        public IAsyncCommand<Exercise> TappedItem { get; protected set; }
        public ICommand BackCommand { get; protected set; }

        private ObservableCollection<Exercise> _exercises;
        public ExerciseListViewModel(int muscleGroupID)
        {
            TappedItem = new AsyncCommand<Exercise>(GoToNextPage);
            var dsource = DataSource.GetInstance();
            var tmplst = dsource.GetExercises().Where(o=>o.MuscleGroups.Any(z=>z.ID==muscleGroupID));
            Exercises = new ObservableCollection<Exercise>(tmplst);
        }

        public ExerciseListViewModel(WorkoutDay _currentWorkoutDay)
        {
            var dsource = DataSource.GetInstance();
            var tmplst = dsource.GetExercises().Where(o => _currentWorkoutDay.Exercises.Any(x => x.ID == o.ID));
            Exercises = new ObservableCollection<Exercise>(tmplst);
            TappedItem = new AsyncCommand<Exercise>(GoToNextPopupPage);
            BackCommand = new Command(GoBack);
        }

        private async void GoBack()
        {
            await Navigation.PopPopupAsync();
        }

        private async Task GoToNextPopupPage(Exercise pickedExercise)
        {
            await Navigation.PushPopupAsync(new ExerciseDetailPopupView(pickedExercise));
        }
        private async Task GoToNextPage(Exercise pickedExercise)
        {
            await Navigation.PushAsync(new ExerciseDetailView(pickedExercise));
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}