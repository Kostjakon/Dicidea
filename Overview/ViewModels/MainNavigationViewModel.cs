using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using DicePage.ViewModels;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Services;
using MenuPage.Views;
using OverviewPage.Views;
using Prism.Regions;
using RollEmSpacePage.Views;
using IdeaPage.Views;

namespace OverviewPage.ViewModels
{
    public class MainNavigationViewModel : BindableBase, INavigationAware
    {
        private IRegionManager _regionManager;
        private NavigationParameters _parameters;
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

        private void GoToDice(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceOverview), _parameters);
            _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
        }

        public DelegateCommand GoToOverview =>
            new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(Overview), _parameters);
                _regionManager.Regions[RegionNames.LeftContentRegion].RemoveAll();
                _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
            });
        public DelegateCommand GoToRollEmSpace =>
            new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(RollEmSpaceOverview), _parameters);
                _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
            });
        public DelegateCommand GoToMenu =>
            new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(MenuOverview), _parameters);
                _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
            });
        public DelegateCommand GoToIdeas =>
            new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(IdeaOverview), _parameters);
                _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
            });

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
        {
            Debug.WriteLine("Not implemented, navigated from DiceOverview to some other side");
        }
    }
}
