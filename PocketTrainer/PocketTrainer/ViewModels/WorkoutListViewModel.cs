using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PocketTrainer.Models;
using PocketTrainer.Views;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace PocketTrainer.ViewModels
{
    public class WorkoutListViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation;
        
        public ObservableCollection<Workout> Workouts { get; set; }
        public ICommand SelectionChanged { get; protected set; }
        
        public Workout SelectedWorkout
        {
            get => _selectedWorkout;
            set
            {
                _selectedWorkout = value;
                OnPropertyChanged("SelectedWorkout");
            }
        }

        private Workout _selectedWorkout;
        private bool _isPickingWorkout;

        public WorkoutListViewModel()
        {
            var dsource = DataSource.GetInstance();
            SelectionChanged = new Command(SelectedItemChanged);
            Workouts = new ObservableCollection<Workout>(dsource.GetWorkouts());
        }
        
        public WorkoutListViewModel(bool isPickingWorkout)
        {
            var dsource = DataSource.GetInstance();
            _isPickingWorkout = isPickingWorkout;
            SelectionChanged = new Command(SelectedItemChanged);
            Workouts = new ObservableCollection<Workout>(dsource.GetWorkouts());
        }

        private async void SelectedItemChanged()
        {
            if(SelectedWorkout==null)
                return;
            var wrk = SelectedWorkout;
            SelectedWorkout = null;
            await Navigation.PushPopupAsync(new WorkoutDayPopupView(wrk,_isPickingWorkout));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}