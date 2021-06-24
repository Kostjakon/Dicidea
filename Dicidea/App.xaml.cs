using Dicidea.Views;
using Prism.Ioc;
using System.Windows;
using DicePage;
using MenuPage;
using OverviewPage;
using Prism.Modularity;
using RollEmSpacePage;
using Dicidea.Core.Services;
using IdeaPage;

namespace Dicidea
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDiceDataService, DiceDataServiceJson>();
            containerRegistry.Register<IRollEmSpaceDataService, RollEmSpaceDataServiceJson>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<MenuModule>();
            moduleCatalog.AddModule<DiceModule>();
            moduleCatalog.AddModule<RollEmSpaceModule>();
            moduleCatalog.AddModule<IdeaPageModule>();
            moduleCatalog.AddModule<OverviewModule>();
        }
    }
}
