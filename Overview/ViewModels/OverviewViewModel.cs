using System;
using System.Diagnostics;
using System.Windows.Input;
using DicePage.ViewModels;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Services;
using MenuPage.Views;
using OverviewPage.Views;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RollEmSpacePage.Views;

namespace OverviewPage.ViewModels
{
    public class OverviewViewModel : BindableBase, INavigationAware
    {
        private bool _isActive;
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;
        private DiceListViewModel _diceListViewModel;
        //private IDiceDataService _diceDataService;
        private IRollEmSpaceDataService _rollEmSpaceDataService;
        private bool _firstInitialisation = true;

        public OverviewViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            IDiceDataService diceDataService = new DiceDataServiceJson();
            _rollEmSpaceDataService = new RollEmSpaceDataServiceJson(diceDataService);
            _diceListViewModel = new DiceListViewModel(diceDataService);
            GoToDiceCommand = new DelegateCommand<object>(GoToDice, CanGoToDice);
            _parameters = new NavigationParameters
            {
                { "diceListViewModel", _diceListViewModel }
            };
        }

        //public DiceListViewModel DiceListViewModel { get => _diceListViewModel; }

        public ICommand GoToDiceCommand { get; private set; }

        private bool CanGoToDice(object obj)
        {
            return true;
        }

        private void GoToDice(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceOverview), _parameters);
            _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
        }

        public DelegateCommand GoToRollEmSpace =>
            new DelegateCommand(() =>
                {
                    _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(RollEmSpaceOverview));
                    _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
                });
        public DelegateCommand GoToMenu =>
            new DelegateCommand(() =>
                {
                    _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(MenuOverview));
                    _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
                });


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Debug.WriteLine("Navigated to Overview");
            if (navigationContext != null && !_firstInitialisation)
            {
                _firstInitialisation = false;
                Debug.WriteLine("Navigation Context is not null");
                if (navigationContext.Parameters["diceListViewModel"] != null)
                {
                    Debug.WriteLine("diceListViewModel is not null");
                    _diceListViewModel = navigationContext.Parameters.GetValue<DiceListViewModel>("diceListViewModel");
                    //_diceListViewModel = navigationContext.Parameters["diceListViewModel"] as DiceListViewModel;
                }

                if (navigationContext.Parameters != null)
                {
                    Debug.WriteLine("Navigation Parameters are not null in OverviewViewModel: " + (navigationContext.Parameters != null));
                    _parameters = navigationContext.Parameters;
                }
            }
            else
            {
                //Message = $"Dice Active from your Prism Module, Dice Anzahl: {AllDice.Count}! Es wurde kein Würfel ausgewählt";
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }



        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Debug.WriteLine("Not implemented, navigated from DiceOverview to some other side");
        }

    }
}
