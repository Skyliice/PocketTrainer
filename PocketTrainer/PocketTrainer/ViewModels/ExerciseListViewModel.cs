using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PocketTrainer.Models;
using PocketTrainer.Views;
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

        private ObservableCollection<Exercise> _exercises;
        public ExerciseListViewModel(int muscleGroupID)
        {
            TappedItem = new AsyncCommand<Exercise>(GoToNextPage);
            var dsource = DataSource.GetInstance();
            var tmplst = dsource.GetExercises().Where(o=>o.MuscleGroups.Any(z=>z.ID==muscleGroupID));
            Exercises = new ObservableCollection<Exercise>(tmplst);
            
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