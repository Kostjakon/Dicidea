using MenuPage.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace MenuPage
{
    public class MenuModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public MenuModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {

            _regionManager.RequestNavigate("MainContentRegion", nameof(MenuOverview));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MenuOverview>();
            //containerRegistry.RegisterForNavigation<MenuActive>();
        }
    }
}