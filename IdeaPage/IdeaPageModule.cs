using IdeaPage.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace IdeaPage
{
    public class IdeaPageModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public IdeaPageModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate("MainContentRegion", nameof(IdeaOverview));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.RegisterForNavigation<IdeaOverview>();
            containerRegistry.RegisterForNavigation<IdeaDetail>();
        }
    }
}