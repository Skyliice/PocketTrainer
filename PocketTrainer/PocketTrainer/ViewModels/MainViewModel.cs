using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using PocketTrainer.Entities;
using PocketTrainer.Models;
using PocketTrainer.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Sets = PocketTrainer.Models.Sets;

namespace PocketTrainer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Exercise> CurrentExercises
        {
            get => _currentExercises;
            set
            {
                _currentExercises = value;
                OnPropertyChanged("CurrentExercises");
            } 
        }

        public string CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged("CurrentDate");
            }
        }

        public bool IsCalendarVisible
        {
            get => _isCalendarVisible;
            set
            {
                _isCalendarVisible = value;
                OnPropertyChanged("IsCalendarVisible");
            }
        }
        
        public ICommand CalendarVisibilityCommand { get; protected set; }
        public ICommand AddNewRoutineCommand { get; protected set; }
        public ICommand DayTappedCommand { get; protected set; }

        private string _currentDate;
        public CultureInfo CurrentCulture { get; protected set; }
        
        public INavigation Navigation;
        private ObservableCollection<Exercise> _currentExercises;
        private bool _isCalendarVisible;

        public MainViewModel()
        {
            var dsource = DataSource.GetInstance();
            LocalDB.GetInstance();
            IsCalendarVisible = true;
            CurrentDate = DateTime.Now.ToLongDateString();
            CalendarVisibilityCommand = new Command(ChangeCalendarVisibility);
            DayTappedCommand = new Command(DayTapped);
            AddNewRoutineCommand = new Command(AddNewRoutine);
            CurrentCulture= CultureInfo.CurrentCulture;
            dsource.SetValues();
            CurrentExercises = new ObservableCollection<Exercise>();
        }

        private void ChangeCalendarVisibility()
        {
            IsCalendarVisible = !IsCalendarVisible;
        }

        private void DayTapped()
        {
            CurrentDate = DateTime.ParseExact(CurrentDate,"MM/dd/yyyy HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture).ToLongDateString();
            RefreshCalendar();
        }

        private async void AddNewRoutine()
        {
            var answer = await Application.Current.MainPage.DisplayActionSheet("Что вы хотите добавить?", "Отмена", null,
                "Упражнение", "Прошлая тренировка", "Программа");
            if (answer == "Упражнение")
            {
                await Navigation.PushAsync(new MuscleGroupsView(true));
                var ldb = LocalDB.GetInstance();
                await Task.Run((() => ldb.WaitHandle.WaitOne()));
                await AddExercise();
            }
            else if (answer == "Прошлая тренировка")
                Console.Write("TODO");
            else if (answer == "Программа")
                await Navigation.PushAsync(new WorkoutListView());
        }

        private async Task AddExercise()
        {
            var ldb = LocalDB.GetInstance();
            if (ldb.SelectedExercise == null)
                return;
            var stringDate = FormatDate();
            await ldb.AddExerciseToWDay(stringDate);
            RefreshCalendar();
        }

        private string FormatDate()
        {
            var date = DateTime.Parse(CurrentDate);
            return $"{date.Day}-{date.Month}-{date.Year}";
        }

        private async void RefreshCalendar()
        {
            var ldb = LocalDB.GetInstance();
            var logs = (await ldb.GetLogs());
            var curLog = logs.Where(o => o.DateOfWorkout == FormatDate()).ToList();
            if (curLog.Count !=0)
            {
                var junctions = (await ldb.GetJunctions()).Where(o => o.WorkoutDayID == curLog.First().WorkoutDayID).ToList();
                var dsource = DataSource.GetInstance();
                var ex = dsource.GetExercises().Where(o => junctions.Any(x => x.ExerciseID == o.ID));
                ex.ForEach(o=>o.Place=junctions.First(x=>x.ExerciseID==o.ID).Place);
                CurrentExercises = new ObservableCollection<Exercise>(ex.OrderBy(o=>o.Place));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}