using Prism.Commands;
using Prism.Mvvm;
using System.Diagnostics;
using System.Windows.Input;
using DicePage.Views;
using Dicidea.Core.Constants;
using MenuPage.Views;
using OverviewPage.Views;
using Prism.Regions;
using RollEmSpacePage.Views;
using IdeaPage.Views;

namespace OverviewPage.ViewModels
{
    /// <summary>
    /// ViewModel für den <see cref="MainNavigation" />. Bei jeder Navigation von der Overview Seite zu dieser View werden die NavigationParameter
    /// des OverviewViewModels zwischengespeichert. Die Navigation gibt diese Parameter zu jeder View zu der sie navigiert weiter.
    /// </summary>
    public class MainNavigationViewModel : BindableBase, INavigationAware
    {
        private IRegionManager _regionManager;
        private NavigationParameters _parameters;
        /// <summary>
        /// Der RegionManager wird hier erhalten und zwischengespeichert.
        /// </summary>
        /// <param name="regionManager">Für die Navigation</param>
        public MainNavigationViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            GoToDiceCommand = new DelegateCommand<object>(GoToDice, CanGoToDice);
            
        }
        

        public ICommand GoToDiceCommand { get; private set; }

        private bool CanGoToDice(object obj)
        {
            return true;
        }
        /// <summary>
        /// Navigation zur Würfel Überblick Seite.
        /// </summary>
        /// <param name="obj"></param>
        private void GoToDice(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceOverview), _parameters);
            _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
        }
        /// <summary>
        /// Navigation zur Überblick Seite.
        /// </summary>
        public DelegateCommand GoToOverview =>
            new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(Overview), _parameters);
                _regionManager.Regions[RegionNames.LeftContentRegion].RemoveAll();
                _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
            });
        /// <summary>
        /// Navigation zur RollEm Überblick Seite.
        /// </summary>
        public DelegateCommand GoToRollEmSpace =>
            new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(RollEmSpaceOverview), _parameters);
                _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
            });
        /// <summary>
        /// Navigation zur Menü Seite. Die Seite ist noch nicht implementiert.
        /// </summary>
        public DelegateCommand GoToMenu =>
            new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(MenuOverview), _parameters);
                _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
            });
        /// <summary>
        /// Navigation zur Ideen Überblick Seite.
        /// </summary>
        public DelegateCommand GoToIdeas =>
            new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(IdeaOverview), _parameters);
                _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
            });
        /// <summary>
        /// Die Navigation Parameter werden so wie sie hier erhalten werden zwischengespeichert.
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Debug.WriteLine("Main Navigation");
            _parameters = navigationContext.Parameters;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }



        public void OnNavigatedFrom(NavigationContext navigationContext)
        {}
    }
}
