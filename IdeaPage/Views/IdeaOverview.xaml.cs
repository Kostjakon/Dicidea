using System.Windows.Controls;
using System.Windows.Input;
using Dicidea.Core.Constants;
using IdeaPage.ViewModels;
using Prism.Regions;

namespace IdeaPage.Views
{
    /// <summary>
    /// CodeBehind für den <see cref="IdeaOverview" />.
    /// </summary>
    public partial class IdeaOverview
    {
        private readonly IRegionManager _regionManager;
        private readonly IdeaOverviewViewModel _ideaOverviewViewModel;
        /// <summary>
        /// Speichert den RegionManager zwischen und holt sich das IdeaOverviewViewModel
        /// </summary>
        /// <param name="regionManager">Zum Navigieren benötigt</param>
        public IdeaOverview(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
            _ideaOverviewViewModel = this.DataContext as IdeaOverviewViewModel;
        }
        /// <summary>
        /// Funktion die aufgerufen wird wenn in der IdeaOverview.xaml auf eine Idee gedoppelklickt wird.
        /// Hier wird die aktuelle Liste der Würfel, die aktuelle Liste der Ideen, die zwei DataServices und der angeklickte Würfel als Parameter gespeichert.
        /// Außerdem wird die gedoppelklickte Idee als Parameter gespeichert und im Anschluss zur IdeaDetail Seite navigiert.
        /// </summary>
        /// <param name="sender">Doppelgeklickte Idee als IdeaViewModel</param>
        /// <param name="e"></param>
        private void IdeaOverview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var parameters = new NavigationParameters
            {
                { "diceListViewModel", _ideaOverviewViewModel.Parameters["diceListViewModel"] },
                { "ideaListViewModel", _ideaOverviewViewModel.Parameters["ideaListViewModel"] },
                { "ideaDataService", _ideaOverviewViewModel.Parameters["ideaDataService"] },
                { "diceDataService", _ideaOverviewViewModel.Parameters["diceDataService"] }
            };
            if (!((sender as ListView)?.SelectedItem is IdeaViewModel idea)) return;
            parameters.Add("selectedIdea", idea);
            parameters.Add("groupedIdeaView", _ideaOverviewViewModel.GroupedIdeaView);
            _regionManager.Regions[RegionNames.MainContentRegion].RemoveAll();
            _regionManager.RequestNavigate(RegionNames.LeftBottomContentRegion, nameof(IdeaDetail), parameters);
            e.Handled = true;
        }
    }
}
