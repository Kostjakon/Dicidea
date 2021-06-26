using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using RollEmSpacePage.Views;

namespace RollEmSpacePage
{
    public class RollEmSpaceModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public RollEmSpaceModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate("MainContentRegion", nameof(RollEmSpaceOverview));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<RollEmSpaceIdeasList>();
            containerRegistry.RegisterForNavigation<RollEmSpaceDetail>();
            containerRegistry.RegisterForNavigation<RollEmSpaceOverview>();
            //containerRegistry.RegisterForNavigation<RollEmSpaceActive>();
        }
    }
}