using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DicePage.ViewModels;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Models;
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
        private IDiceDataService _diceDataService;
        private IRollEmSpaceDataService _rollEmSpaceDataService;
        private bool _firstInitialisation = true;
        private Dice _lastRolledDice;

        public OverviewViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _diceDataService = new DiceDataServiceJson();
            _rollEmSpaceDataService = new RollEmSpaceDataServiceJson(_diceDataService);
            _diceListViewModel = new DiceListViewModel(_diceDataService);
            GoToDiceCommand = new DelegateCommand<object>(GoToDice, CanGoToDice);
            GoToRollEmSpaceCommand = new DelegateCommand<object>(GoToRollEmSpace);
            GoToLastRolledRollEmSpaceCommand = new DelegateCommand<object>(GoToLastRolledRollEmSpace);
            _parameters = new NavigationParameters
            {
                { "diceListViewModel", _diceListViewModel }
            };
            getLastRolledDice();
        }

        public Dice LastRolledDice
        {
            get => _lastRolledDice;
            set => SetProperty(ref _lastRolledDice, value);
        }

        private async Task getLastRolledDice()
        {
            LastRolledDice = await _diceDataService.GetLastRolledDiceAsync();
        }
        //public DiceListViewModel DiceListViewModel { get => _diceListViewModel; }

        public ICommand GoToDiceCommand { get; private set; }
        public ICommand GoToRollEmSpaceCommand { get; private set; }
        public ICommand GoToLastRolledRollEmSpaceCommand { get; private set; }

        private bool CanGoToDice(object obj)
        {
            return true;
        }

        private void GoToDice(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceOverview), _parameters);
            _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
        }

        public void GoToRollEmSpace(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(RollEmSpaceOverview), _parameters);
            _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
        }
        public void GoToLastRolledRollEmSpace(object obj)
        {
            DiceViewModel selectedDice = _diceListViewModel.AllDice.First(d => d.Dice == LastRolledDice);
            _parameters.Add("selectedDice", selectedDice);
            _regionManager.Regions[RegionNames.MainContentRegion].RemoveAll();
            _regionManager.RequestNavigate(RegionNames.LeftBottomContentRegion, nameof(RollEmSpaceDetail), _parameters);
            _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
        }
        public DelegateCommand GoToMenu =>
            new DelegateCommand(() =>
                {
                    _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(MenuOverview), _parameters);
                    _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
                });


        public async void OnNavigatedTo(NavigationContext navigationContext)
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
            await getLastRolledDice();
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
