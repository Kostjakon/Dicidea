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
using IdeaPage.ViewModels;
using IdeaPage.Views;
using MahApps.Metro.Controls.Dialogs;
using MenuPage.Views;
using OverviewPage.Views;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RollEmSpacePage.Views;

namespace OverviewPage.ViewModels
{
    public class OverviewViewModel : BindableBase, INavigationAware
    {
        private bool _isActive;
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;
        private DiceListViewModel _diceListViewModel;
        private IdeaListViewModel _ideaListViewModel;
        private IDiceDataService _diceDataService;
        private IIdeaDataService _ideaDataService;
        private readonly IDialogService _dialogService;
        private Dice _lastRolledDice;
        private Idea _lastRolledIdea;

        public OverviewViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            GoToDiceCommand = new DelegateCommand<object>(GoToDice, CanGoToDice);
            GoToRollEmSpaceCommand = new DelegateCommand<object>(GoToRollEmSpace);
            GoToLastRolledRollEmSpaceCommand = new DelegateCommand<object>(GoToLastRolledRollEmSpace);
            GoToIdeaCommand = new DelegateCommand<object>(GoToIdea);
            GoToLastRolledIdeaCommand = new DelegateCommand<object>(GoToLastRolledIdea);
        }

        public Dice LastRolledDice
        {
            get => _lastRolledDice;
            set => SetProperty(ref _lastRolledDice, value);
        }

        private async Task getLastRolledDice()
        {
            if(_diceDataService != null) LastRolledDice = await _diceDataService.GetLastRolledDiceAsync();
        }
        public Idea LastRolledIdea
        {
            get => _lastRolledIdea;
            set => SetProperty(ref _lastRolledIdea, value);
        }

        private async Task getLastRolledIdea()
        {
            if(_ideaDataService != null) LastRolledIdea = await _ideaDataService.GetLastRolledIdeaAsync();
        }
        //public DiceListViewModel DiceListViewModel { get => _diceListViewModel; }

        public ICommand GoToDiceCommand { get; private set; }
        public ICommand GoToRollEmSpaceCommand { get; private set; }
        public ICommand GoToLastRolledRollEmSpaceCommand { get; private set; }
        public ICommand GoToIdeaCommand { get; private set; }
        public ICommand GoToLastRolledIdeaCommand { get; private set; }

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
            if (_diceListViewModel.AllDice.Count > 0)
            {
                DiceViewModel selectedDice = _diceListViewModel.AllDice.First(d => d.Dice == LastRolledDice);
                _parameters.Add("selectedDice", selectedDice);
            }
            _regionManager.Regions[RegionNames.MainContentRegion].RemoveAll();
            _regionManager.RequestNavigate(RegionNames.LeftBottomContentRegion, nameof(RollEmSpaceDetail), _parameters);
            _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
        }

        public void GoToIdea(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(IdeaOverview), _parameters);
            _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
        }
        public void GoToLastRolledIdea(object obj)
        {
            if (_ideaListViewModel.AllIdeas.Count > 0 && LastRolledIdea != null)
            {
                IdeaViewModel selectedIdea = _ideaListViewModel.AllIdeas.First(i => i.Idea == LastRolledIdea);
                Debug.WriteLine(selectedIdea.Idea.Name);
                _parameters.Add("selectedIdea", selectedIdea);
            }
            _regionManager.Regions[RegionNames.MainContentRegion].RemoveAll();
            _regionManager.RequestNavigate(RegionNames.LeftBottomContentRegion, nameof(IdeaDetail), _parameters);
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
            if (navigationContext.Parameters["diceListViewModel"] != null)
            {
                _diceListViewModel = navigationContext.Parameters.GetValue<DiceListViewModel>("diceListViewModel");
                _diceDataService = navigationContext.Parameters.GetValue<IDiceDataService>("diceDataService");
                _ideaDataService = navigationContext.Parameters.GetValue<IIdeaDataService>("ideaDataService");
                _ideaListViewModel = navigationContext.Parameters.GetValue<IdeaListViewModel>("ideaListViewModel");
                _parameters = new NavigationParameters
                {
                    { "diceListViewModel", _diceListViewModel },
                    { "ideaListViewModel", _ideaListViewModel },
                    { "ideaDataService", _ideaDataService },
                    { "diceDataService", _diceDataService }
                };
                await getLastRolledDice();
                await getLastRolledIdea();
            }
            else
            {
                Debug.WriteLine("Create everything new");
                _diceDataService ??= new DiceDataServiceJson();
                _diceListViewModel ??= new DiceListViewModel(_diceDataService, _dialogService);
                _ideaDataService ??= new IdeaDataServiceJson();
                _ideaListViewModel ??= new IdeaListViewModel(_ideaDataService, _dialogService);
                _parameters ??= new NavigationParameters
                {
                    { "diceListViewModel", _diceListViewModel },
                    { "ideaListViewModel", _ideaListViewModel },
                    { "ideaDataService", _ideaDataService },
                    { "diceDataService", _diceDataService }
                };
                await getLastRolledDice();
                await getLastRolledIdea();
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
