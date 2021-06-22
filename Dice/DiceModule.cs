using DicePage.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DicePage
{
    public class DiceModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public DiceModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate("MainContentRegion", nameof(DiceOverview));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DiceOverview>();
            containerRegistry.RegisterForNavigation<DiceDetail>();
            containerRegistry.RegisterForNavigation<DiceDetailList>();
        }
    }
}