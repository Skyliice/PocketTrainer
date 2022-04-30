using System;
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
        
        public Exercise SelectedExercise
        {
            get => _selectedExercise;
            set
            {
                _selectedExercise = value;
                OnPropertyChanged("SelectedExercise");
            } 
        }

        public bool CanChooseExercise
        {
            get => _canChooseExercise;
            set
            {
                _canChooseExercise = value;
                OnPropertyChanged("CanChooseExercise");
            }
        }

        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                OnPropertyChanged("SearchString");
            }
        }

        public INavigation Navigation;
        public IAsyncCommand<Exercise> TappedItem { get; protected set; }
        public ICommand BackCommand { get; protected set; }
        public ICommand SearchCommand { get; protected set; }
        public ICommand PickExerciseCommand { get; protected set; }

        private ObservableCollection<Exercise> _exercises;
        private ObservableCollection<Exercise> _allExercises;
        private Exercise _selectedExercise;
        private bool _canChooseExercise;
        private string _searchString;

        public ExerciseListViewModel(int muscleGroupID, bool isPickingExercise)
        {
            if (!isPickingExercise)
            {
                TappedItem = new AsyncCommand<Exercise>(GoToNextPage);
            }
            else
            {
                TappedItem = new AsyncCommand<Exercise>(SelectionChanged);
                PickExerciseCommand = new Command(PickExercise);
            }
            BackCommand = new Command(GoBackToMuscleGroup);
            SearchCommand = new Command(SearchExercises);
            CanChooseExercise = false;
            var dsource = DataSource.GetInstance();
            var tmplst = dsource.GetExercises().Where(o=>o.MuscleGroups.Any(z=>z.ID==muscleGroupID));
            Exercises = new ObservableCollection<Exercise>(tmplst);
            _allExercises = new ObservableCollection<Exercise>(tmplst);
        }

        public ExerciseListViewModel(WorkoutDay _currentWorkoutDay)
        {
            var dsource = DataSource.GetInstance();
            var tmplst = dsource.GetExercises().Where(o => _currentWorkoutDay.Exercises.Any(x => x.ID == o.ID));
            Exercises = new ObservableCollection<Exercise>(tmplst);
            TappedItem = new AsyncCommand<Exercise>(GoToNextPopupPage);
            BackCommand = new Command(GoBack);
        }

        private void SearchExercises()
        {
            if (SearchString == String.Empty)
                Exercises = _allExercises;
            var temprecords = Exercises.Where(o => o.Name.ToLower().Contains(SearchString.ToLower()));
            var tempcollection = new ObservableCollection<Exercise>(temprecords);
            Exercises = tempcollection;
        }

        private async void PickExercise()
        {
            if (SelectedExercise is null)
                Console.Read();
            else
            {
                var ldb = LocalDB.GetInstance();
                ldb.SelectedExercise = SelectedExercise;
                await Navigation.PopToRootAsync();
                ldb.WaitHandle.Set();
            }
        }

        private async void GoBackToMuscleGroup()
        {
            await Navigation.PopAsync();
        }

        private async void GoBack()
        {
            await Navigation.PopPopupAsync();
        }

        private async Task SelectionChanged(Exercise pickedExercise)
        {
            CanChooseExercise = SelectedExercise != null;
        }

        private async Task GoToNextPopupPage(Exercise pickedExercise)
        {
            SelectedExercise = null;
            await Navigation.PushPopupAsync(new ExerciseDetailPopupView(pickedExercise));
        }
        private async Task GoToNextPage(Exercise pickedExercise)
        {
            SelectedExercise = null;
            await Navigation.PushAsync(new ExerciseDetailView(pickedExercise));
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}