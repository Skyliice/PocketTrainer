using PocketTrainer.Models;
using Xamarin.Forms;

namespace PocketTrainer.ViewModels
{
    public class MainViewModel
    {
        public INavigation Navigation;
        public MainViewModel()
        {
            var dsource = DataSource.GetInstance();
            dsource.SetValues();
        }
    }
}