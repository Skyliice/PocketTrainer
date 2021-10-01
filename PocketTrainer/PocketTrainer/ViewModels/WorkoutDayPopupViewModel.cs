using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class WorkoutDayPopupViewModel : INotifyPropertyChanged
    {
        public bool IsChosen
        {
            get => _isChosen;
            set
            {
                _isChosen = value;
                OnPropertyChanged("IsChosen");
            }
        }
        
        public WorkoutDay SelectedWorkoutDay
        {
            get => _selectedWorkoutDay;
            set
            {
                _selectedWorkoutDay = value;
                OnPropertyChanged("SelectedWorkoutDay");
            } 
        }

        public List<WorkoutDay> CurrentWorkoutDays { get; set; }
        public INavigation Navigation { get; set; }
        public ICommand BackCommand { get; protected set; }
        public ICommand TappedItem { get; protected set; }
        public ICommand ChooseWorkoutDayCommand { get; protected set; }

        private bool _isPickingWorkout;
        private bool _isChosen;
        private WorkoutDay _selectedWorkoutDay;
        public WorkoutDayPopupViewModel(Workout _currentWorkout,bool isPickingWorkout)
        {
            SelectedWorkoutDay = new WorkoutDay();
            IsChosen = false;
            _isPickingWorkout = isPickingWorkout;
            CurrentWorkoutDays = new List<WorkoutDay>();
            TappedItem = new Command(SelectionChanged);
            ChooseWorkoutDayCommand = new Command(ChooseWorkoutDay);
            BackCommand = new Command(GoBack);
            var dsource = DataSource.GetInstance();
            CurrentWorkoutDays = dsource.GetWorkoutDays().Where(o => _currentWorkout.WorkoutDays.Any(x => x.ID == o.ID))
                .ToList();
        }

        private async void ChooseWorkoutDay()
        {
            var ldb = LocalDB.GetInstance();
            ldb.SelectedWorkoutDay = SelectedWorkoutDay;
            await Navigation.PopToRootAsync();
            await Navigation.PopPopupAsync();
            ldb.WaitHandle.Set();
        }

        private async void SelectionChanged()
        {
            if (_isPickingWorkout)
            {
                IsChosen = SelectedWorkoutDay != null;
            }
            else
            {
                if(SelectedWorkoutDay==null)
                    return;
                var wrk = SelectedWorkoutDay;
                SelectedWorkoutDay = null;
                await Navigation.PushPopupAsync(new ExerciseListPopupView(wrk));
            }
        }
        private async void GoBack()
        {
            await Navigation.PopPopupAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}