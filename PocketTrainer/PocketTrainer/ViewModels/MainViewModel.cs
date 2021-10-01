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
using Rg.Plugins.Popup.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Plugin.Calendar.Controls;
using Xamarin.Plugin.Calendar.Models;
using Log = PocketTrainer.Entities.Log;
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

        public bool IsFreeDay
        {
            get => _isFreeDay;
            set
            {
                _isFreeDay = value;
                OnPropertyChanged("IsFreeDay");
            }
        }

        public bool HasWday
        {
            get => _hasWday;
            set
            {
                _hasWday = value;
                OnPropertyChanged("HasWday");
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged("IsRefreshing");
            }
        }

        public EventCollection EventsCollection
        {
            get => _eventsCollection;
            set
            {
                _eventsCollection = value;
                OnPropertyChanged("EventsCollection");
            }
        }

        public ICommand CalendarVisibilityCommand { get; protected set; }
        public ICommand AddNewRoutineCommand { get; protected set; }
        public ICommand DayTappedCommand { get; protected set; }
        public ICommand DeleteWDayCommand { get; protected set; }
        public Command<Exercise> ExerciseCommand { get; protected set; }
        public Command<Exercise> AddSetCommand { get; protected set; }
        public Command<Exercise> ImageClickedCommand { get; protected set; }
        public Command<Sets> SetTapCommand { get; protected set; }

        private string _currentDate;
        public CultureInfo CurrentCulture { get; protected set; }
        
        public INavigation Navigation;
        private ObservableCollection<Exercise> _currentExercises;
        private bool _isCalendarVisible;
        private bool _isFreeDay;
        private bool _isRefreshing;
        private bool _hasWday;
        private EventCollection _eventsCollection;

        public MainViewModel()
        {
            var dsource = DataSource.GetInstance();
            LocalDB.GetInstance();
            IsCalendarVisible = true;
            AddSetCommand = new Command<Exercise>(AddNewSet);
            CurrentDate = DateTime.Now.ToLongDateString();
            CalendarVisibilityCommand = new Command(ChangeCalendarVisibility);
            DayTappedCommand = new Command(DayTapped);
            AddNewRoutineCommand = new Command(AddNewRoutine);
            DeleteWDayCommand = new Command(DeleteWDay);
            SetTapCommand = new Command<Sets>(SetTapped);
            ExerciseCommand = new Command<Exercise>(OpenExerciseMenu);
            ImageClickedCommand = new Command<Exercise>(ImageClicked);
            CurrentCulture= CultureInfo.CurrentCulture;
            dsource.SetValues();
            CurrentExercises = new ObservableCollection<Exercise>();
            RefreshCalendar();
            IsRefreshing = false;
            RefreshEvents();
        }

        private async void ImageClicked(Exercise pickedExercise)
        {
            await Navigation.PushPopupAsync(new ExerciseDetailPopupView(pickedExercise));
        }

        private async void SetTapped(Sets pickedSet)
        {
           await Navigation.PushPopupAsync(new EditSetPopupView(pickedSet));
        }

        private async void OpenExerciseMenu(Exercise pickedExercise)
        {
            var response = await Application.Current.MainPage.DisplayActionSheet("Что вы хотите сделать?","Отмена",null,"Удалить упражнение");
            if (response == "Удалить упражнение")
            {
                var ldb = LocalDB.GetInstance();
                await ldb.DeleteExerciseFromWDay(FormatDate(), pickedExercise.Place);
                RefreshCalendar();
                RefreshEvents();
            }
        }

        private async void DeleteWDay()
        {
            var response = await Application.Current.MainPage.DisplayAlert("Удаление",
                "Вы действительно хотите удалить выбранную тренировку?", "Да", "Нет");
            if (response)
            {
                var ldb = LocalDB.GetInstance();
                await ldb.DeleteWDay(FormatDate());
                RefreshCalendar();
                RefreshEvents();
            }
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

        private async void AddNewSet(Exercise selectedExercise)
        {
            var ldb = LocalDB.GetInstance();
            var index = CurrentExercises.IndexOf(selectedExercise);
            var setslist = CurrentExercises.ElementAt(index).SetsList;
            setslist.Add(new Sets(){RepsNumber = 0,Weight = 0});
            CurrentExercises.ElementAt(index).SetsList = null;
            CurrentExercises.ElementAt(index).SetsList=setslist;
            var date = FormatDate();
            await ldb.AddSetToExercise(selectedExercise.ID,selectedExercise.Place,date);
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
            {
                var ldb = LocalDB.GetInstance();
                await Navigation.PushAsync(new WorkoutListView(true));
                await Task.Run((() => ldb.WaitHandle.WaitOne()));
                await AddWorkoutDay();
            }
                
        }

        private async Task AddWorkoutDay()
        {
            var ldb = LocalDB.GetInstance();
            await ldb.AddNewWorkoutDayToDay(FormatDate());
            RefreshEvents();
            RefreshCalendar();
        }

        private async void RefreshEvents()
        {
            var ev = new EventCollection(); 
            var ldb = LocalDB.GetInstance();
            var logs = await ldb.GetLogs();
            foreach (var log in logs)
            {
                ev.Add(DateTime.Parse(log.DateOfWorkout), new EventCollection());
            }
            EventsCollection = ev;
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
            IsRefreshing = true;
            var ldb = LocalDB.GetInstance();
            var logs = (await ldb.GetLogs());
            var curLog = logs.Where(o => o.DateOfWorkout == FormatDate()).ToList();
            if (curLog.Count !=0)
            {
                var junctions = (await ldb.GetJunctions()).Where(o => o.WorkoutDayID == curLog.First().WorkoutDayID).ToList();
                var lst = new List<WorkDayExJunction>();
                foreach (var junction in junctions)
                {
                    lst.Add(await ldb.GetJunctionWithChildren(junction.ID));
                }
                junctions = lst;
                var dsource = DataSource.GetInstance();
                var ex = dsource.GetExercises().Where(o => junctions.Any(x => x.ExerciseID == o.ID));
                ex.ForEach(o=>o.Place=junctions.First(x=>x.ExerciseID==o.ID).Place);
                ex.ForEach(o=>o.SetsList=ConvertSets(junctions.First(x=>x.ExerciseID==o.ID).SetsList));
                CurrentExercises = new ObservableCollection<Exercise>(ex.OrderBy(o=>o.Place));
                IsFreeDay = false;
            }
            else
            {
                CurrentExercises = new ObservableCollection<Exercise>();
                IsFreeDay = true;
            }
            HasWday = !IsFreeDay;
            IsRefreshing = false;
        }

        private List<Models.Sets> ConvertSets(List<Entities.Sets> sets)
        {
            var newSets = new List<Models.Sets>();
            if (sets == null)
                return newSets;
            foreach (var set in sets)
            {
                newSets.Add(new Sets(){RepsNumber = set.RepsNumber,Weight = set.Weight, ID = set.ID});
            }
            return newSets;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}