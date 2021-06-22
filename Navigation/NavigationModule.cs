using Dicidea.Core.Constants;
using Navigation.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Navigation
{
    public class NavigationModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public NavigationModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.LeftContentRegion, nameof(Views.Navigation));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.Navigation>();
        }
    }
}