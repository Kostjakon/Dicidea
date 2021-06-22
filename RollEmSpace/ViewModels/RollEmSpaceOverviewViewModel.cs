using Prism.Mvvm;

namespace RollEmSpacePage.ViewModels
{
    public class RollEmSpaceOverviewViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public RollEmSpaceOverviewViewModel()
        {
            Message = "Roll 'em Space Overview from your Prism Module";
        }
    }
}
