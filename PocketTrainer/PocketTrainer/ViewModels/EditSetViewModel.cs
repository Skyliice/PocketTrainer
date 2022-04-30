using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PocketTrainer.Models;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace PocketTrainer.ViewModels
{
    public class EditSetViewModel : INotifyPropertyChanged
    {
        public Sets SelectedSet
        {
            get => _selectedSet;
            set
            {
                _selectedSet = value;
                OnPropertyChanged("SelectedSet");
            } 
        }

        public bool IsOkEnabled
        {
            get => _isOkEnabled;
            set
            {
                _isOkEnabled = value;
                OnPropertyChanged("IsOkEnabled");
            }
        }
        public string EditableRepsNumber { get; set; }
        public string EditableWeight { get; set; }
        
        public ICommand WeightChangedCommand { get; protected set; }
        public ICommand RepsNumberChangedCommand { get; protected set; }
        public ICommand OkCommand { get; protected set; }
        public ICommand CancelCommand { get; protected set; }
        public ICommand DeleteCommand { get; protected set; }

        private bool _isOkEnabled;
        private Sets _selectedSet;

        private bool _weightIsCorrect;
        private bool _repsNumberIsCorrect;
        
        public INavigation Navigation { get; set; }
        public EditSetViewModel(Sets pickedSet)
        {
            SelectedSet = pickedSet;
            IsOkEnabled = true;
            _weightIsCorrect = true;
            _repsNumberIsCorrect = true;
            EditableRepsNumber = pickedSet.RepsNumber.ToString();
            EditableWeight = pickedSet.Weight.ToString();
            OkCommand = new Command(OkPressed);
            CancelCommand = new Command(CancelPressed);
            DeleteCommand = new Command(DeletePressed);
            WeightChangedCommand = new Command(WeightChanged);
            RepsNumberChangedCommand = new Command(RepsNumberChanged);
        }

        private void RepsNumberChanged()
        {
            int tmp;
            if (Int32.TryParse(EditableRepsNumber, out tmp))
            {
                _repsNumberIsCorrect = true;
            }
            else _repsNumberIsCorrect = false;
            CheckOkStatus();
        }

        private void WeightChanged()
        {
            float tmp;
            if (float.TryParse(EditableWeight, out tmp))
            {
                _weightIsCorrect = true;
            }
            else _weightIsCorrect = false;
            CheckOkStatus();
        }

        private void CheckOkStatus()
        {
            if (_repsNumberIsCorrect && _weightIsCorrect)
            {
                IsOkEnabled = true;
            }
            else
                IsOkEnabled = false;
        }

        private async void DeletePressed()
        {
            var response = await Application.Current.MainPage.DisplayAlert("Внимание",
                "Вы действительно хотите удалить этот сет?", "Да", "Отмена");
            if (response)
            {
                
            }
        }

        private async void CancelPressed()
        {
            await Navigation.PopPopupAsync();
        }

        private async void OkPressed()
        {
            var ldb = LocalDB.GetInstance();
            SelectedSet.Weight = Single.Parse(EditableWeight);
            SelectedSet.RepsNumber = Int32.Parse(EditableRepsNumber);
            await ldb.UpdateSetInfo(SelectedSet.ID, SelectedSet.RepsNumber, SelectedSet.Weight);
            await Navigation.PopPopupAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}