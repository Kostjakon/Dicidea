using System.Diagnostics;
using DicePage.ViewModels;
using Dicidea.Core.Constants;
using Dicidea.Core.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace OverviewPage
{
    public class OverviewModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;

        public OverviewModule(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _regionManager = regionManager;

            DiceDataService = new DiceDataServiceJson(dialogService);

            DiceListViewModel = new DiceListViewModel(DiceDataService, dialogService);

            _parameters = new NavigationParameters();
            _parameters.Add("DiceListViewModel", DiceListViewModel);

            Debug.WriteLine("DiceListViewModel Parameter in ShellViewModel isn't null: " + (_parameters["DiceListViewModel"] != null));
            
        }
        public IDiceDataService DiceDataService { get; }

        public DiceListViewModel DiceListViewModel { get; }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(Views.Overview), _parameters);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.Overview>();
            containerRegistry.RegisterForNavigation<Views.MainNavigation>();
        }
    }
}