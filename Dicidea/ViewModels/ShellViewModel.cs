using Dicidea.Core.Helper;

namespace Dicidea.ViewModels
{
    /// <summary>
    ///     Interaction logic for Shell.xaml
    /// </summary>
    public class ShellViewModel : NotifyPropertyChanges
    {
        private string _title;

        public ShellViewModel()
        {
            _title = "Dicidea";
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

    }
}
