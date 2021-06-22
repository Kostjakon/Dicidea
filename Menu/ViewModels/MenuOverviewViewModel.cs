using Prism.Mvvm;

namespace MenuPage.ViewModels
{
    public class MenuOverviewViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public MenuOverviewViewModel()
        {
            Message = "Menu Overview from your Prism Module";
        }
    }
}
