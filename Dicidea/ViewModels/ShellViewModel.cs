using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Navigation;
using DicePage.ViewModels;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Helper;
using Dicidea.Core.Services;
using MenuPage.Views;
using OverviewPage.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RollEmSpacePage.Views;
using OverviewPage.Views;
//using RollEmSpaceOverview = RollEmSpacePage.Views.RollEmSpaceOverview;

namespace Dicidea.ViewModels
{
    public class ShellViewModel : NotifyPropertyChanges
    {
        private string _title = "Dicidea";
        private readonly IRegionManager _regionManager;

        public ShellViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public bool HideTitleBackground
        {
            get => true;
        }
        
        public string Title
        {
            get { return _title + HideTitleBackground; }
            set { SetProperty(ref _title, value); }
        }

    }
}
