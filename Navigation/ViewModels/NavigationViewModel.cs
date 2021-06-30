using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DicePage.ViewModels;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Services;
using MenuPage.Views;
using OverviewPage.Views;
using Prism.Regions;
using Prism.Services.Dialogs;
using RollEmSpacePage.Views;

namespace Navigation.ViewModels
{
    public class NavigationViewModel : BindableBase
    {
        
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;
        public NavigationViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;

            DiceDataService = new DiceDataServiceJson();

            DiceListViewModel = new DiceListViewModel(DiceDataService, dialogService);
            GoToDiceCommand = new DelegateCommand<object>(GoToDice, CanGoToDice);

            _parameters = new NavigationParameters();
            _parameters.Add("diceListViewModel", DiceListViewModel);
            _parameters.Add("regionManager", _regionManager);
        }



        public IDiceDataService DiceDataService { get; }

        public DiceListViewModel DiceListViewModel { get; }

        public ICommand GoToDiceCommand { get; private set; }

        private bool CanGoToDice(object obj)
        {
            return DiceListViewModel != null;
        }

        private void GoToDice(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceOverview), _parameters);
        }

        public DelegateCommand GoToOverview =>
            new DelegateCommand(() => _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(Overview)));
        public DelegateCommand GoToRollEmSpace =>
            new DelegateCommand(() => _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(RollEmSpaceOverview)));
        public DelegateCommand GoToMenu =>
            new DelegateCommand(() => _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(MenuOverview)));
    }
}
