using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using DicePage.ViewModels;
using Dicidea.Core.Constants;
using Prism.Regions;
using RollEmSpacePage.ViewModels;

namespace RollEmSpacePage.Views
{
    /// <summary>
    /// Interaction logic for RollEmSpaceOverview.xaml
    /// </summary>
    public partial class RollEmSpaceOverview
    {
        private readonly IRegionManager _regionManager;
        private readonly RollEmSpaceOverviewViewModel _rollEmSpaceOverviewViewModel;
        public RollEmSpaceOverview(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
            _rollEmSpaceOverviewViewModel = this.DataContext as RollEmSpaceOverviewViewModel;
        }

        /// <summary>
        /// Funktion die aufgerufen wird wenn in der RollEmSpaceOverview.xaml auf einen Würfel gedoppelklickt wird. Hier wird die aktuelle Liste der Würfel, die aktuelle Liste der Ideen, die zwei DataServices und der angeklickte Würfel als Parameter gespeichert.
        /// </summary>
        /// <param name="sender">Der doppelgeklickte Würfel als DiceViewModel</param>
        /// <param name="e"></param>
        private void SelectDice_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var parameters = new NavigationParameters
            {
                { "diceListViewModel", _rollEmSpaceOverviewViewModel.Parameters["diceListViewModel"] },
                { "ideaListViewModel", _rollEmSpaceOverviewViewModel.Parameters["ideaListViewModel"] },
                { "ideaDataService", _rollEmSpaceOverviewViewModel.Parameters["ideaDataService"] },
                { "diceDataService", _rollEmSpaceOverviewViewModel.Parameters["diceDataService"] },
            };
            var dice = (sender as ListView)?.SelectedItem as DiceViewModel;
            if (dice != null) Debug.WriteLine(dice.Dice.Name);
            if (dice != null)
            {
                parameters.Add("selectedDice", dice);
                _regionManager.Regions[RegionNames.MainContentRegion].RemoveAll();
                _regionManager.RequestNavigate(RegionNames.LeftBottomContentRegion, nameof(RollEmSpaceDetail), parameters);
                e.Handled = true;
            }
        }
    }
}
