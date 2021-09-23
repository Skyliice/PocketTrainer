using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketTrainer.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PocketTrainer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MuscleGroupsView : ContentPage
    {
        public MuscleGroupsView(bool isPickingExercise)
        {
            InitializeComponent();
            BindingContext = new MuscleGroupsViewModel(isPickingExercise) {Navigation = this.Navigation};
        }
        
        public MuscleGroupsView()
        {
            InitializeComponent();
            BindingContext = new MuscleGroupsViewModel(false) {Navigation = this.Navigation};
        }
    }
}