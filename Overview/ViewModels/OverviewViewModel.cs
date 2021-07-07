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
using MenuPage.Views;
using OverviewPage.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RollEmSpacePage.Views;

namespace OverviewPage.ViewModels
{
    /// <summary>
    /// ViewModel für den <see cref="Overview" />. Hier werden beim Start des Programms die Ideen und Würfel geladen
    /// und als DiceListViewModel und IdeaListViewModel über die Navigation Parameter an die Haupt-Navigation und die
    /// Seite übergeben zu der navigiert wird
    /// </summary>
    public class OverviewViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;
        private DiceListViewModel _diceListViewModel;
        private IdeaListViewModel _ideaListViewModel;
        private IDiceDataService _diceDataService;
        private IIdeaDataService _ideaDataService;
        private readonly IDialogService _dialogService;
        private Dice _lastRolledDice;
        private Idea _lastRolledIdea;
        /// <summary>
        /// Die Commands werden erzeugt und der regionManager und der dialogService gesetzt.
        /// </summary>
        /// <param name="regionManager">Zum navigieren benötigt</param>
        /// <param name="dialogService">Zum Erzeugen von Dialogen benötigt</param>
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
        /// <summary>
        /// Zuletzt gerollter/erzeugter Würfel
        /// </summary>
        public Dice LastRolledDice
        {
            get => _lastRolledDice;
            set => SetProperty(ref _lastRolledDice, value);
        }
        /// <summary>
        /// Zum laden des zuletzt gerollten/erzeugten Würfels
        /// </summary>
        /// <returns></returns>
        private async Task GetLastRolledDice()
        {
            if(_diceDataService != null) LastRolledDice = await _diceDataService.GetLastRolledDiceAsync();
        }
        /// <summary>
        /// Zuletzt gerollte Idee
        /// </summary>
        public Idea LastRolledIdea
        {
            get => _lastRolledIdea;
            set => SetProperty(ref _lastRolledIdea, value);
        }
        /// <summary>
        /// Zum Laden der zuletzt gerollten Idee
        /// </summary>
        /// <returns></returns>
        private async Task GetLastRolledIdea()
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
        /// <summary>
        /// Zum Navigieren zur Würfel Überblick Seite
        /// </summary>
        /// <param name="obj"></param>
        private void GoToDice(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceOverview), _parameters);
            _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
        }
        /// <summary>
        /// Zum Navigieren zur RollEm Übersicht Seite
        /// </summary>
        /// <param name="obj"></param>
        public void GoToRollEmSpace(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(RollEmSpaceOverview), _parameters);
            _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
        }
        /// <summary>
        /// Zum Navigieren zur RollEm Detail Seite. Gibt den zuletzt gerollten/erstellten Würfel mit.
        /// </summary>
        /// <param name="obj"></param>
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
        /// <summary>
        /// Zum Navigieren zur Ideen Übersicht Seite
        /// </summary>
        /// <param name="obj"></param>
        public void GoToIdea(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(IdeaOverview), _parameters);
            _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
        }
        /// <summary>
        /// Zum Navigieren zur Ideen Detail Seite. Gibt die zuletzt gerollte Idee mit.
        /// </summary>
        /// <param name="obj"></param>
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
        /// <summary>
        /// Zum Navigieren zur Menü Seite. Die Seite ist noch nicht implementiert.
        /// </summary>
        public DelegateCommand GoToMenu =>
            new DelegateCommand(() =>
                {
                    _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(MenuOverview), _parameters);
                    _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(MainNavigation), _parameters);
                });

        /// <summary>
        /// Wenn von einer anderen Seite zu dieser navigiert wird, werden die Navigations Parameter zwischengespeichert,
        /// wenn das erste mal auf die Seite navigiert wird und die Navigation Parameter leer sind, wird alles neu erzeugt.
        /// Dazu gehören: DiceListViewModel, DiceDataService, IdeaListViewModel, IdeaDataService.
        /// Die zwischengespeicherte Navigation Parameter Variable wird in beiden Fällen neu erstellt und mit den
        /// jeweiligen Parametern gefüllt.
        /// </summary>
        /// <param name="navigationContext"></param>
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
                await GetLastRolledDice();
                await GetLastRolledIdea();
            }
            else
            {
                Debug.WriteLine("Create everything new");
                _diceDataService ??= new DiceDataServiceJson(_dialogService);
                _diceListViewModel ??= new DiceListViewModel(_diceDataService, _dialogService);
                _ideaDataService ??= new IdeaDataServiceJson(_dialogService);
                _ideaListViewModel ??= new IdeaListViewModel(_ideaDataService, _dialogService);
                _parameters ??= new NavigationParameters
                {
                    { "diceListViewModel", _diceListViewModel },
                    { "ideaListViewModel", _ideaListViewModel },
                    { "ideaDataService", _ideaDataService },
                    { "diceDataService", _diceDataService }
                };
                await GetLastRolledDice();
                await GetLastRolledIdea();
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }



        public void OnNavigatedFrom(NavigationContext navigationContext) {}

    }
}
