using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using PocketTrainer.Entities;
using PocketTrainer.Models;
using Xamarin.Forms;

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

        private string _currentDate;
        public CultureInfo CurrentCulture { get; protected set; }
        
        public INavigation Navigation;
        private ObservableCollection<Exercise> _currentExercises;
        public MainViewModel()
        {
            CurrentDate = DateTime.Now.ToLongDateString();
            var dsource = DataSource.GetInstance();
            CurrentCulture= CultureInfo.CurrentCulture;
            dsource.SetValues();
            CurrentExercises = new ObservableCollection<Exercise>()
                {new Exercise() {Name = "TestExercise", ImagePath = "" , SetsList = new List<Sets>(){ new Sets(){RepsNumber = 0, Weight = 0}, new Sets(){RepsNumber = 1, Weight = 2} }}};
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}