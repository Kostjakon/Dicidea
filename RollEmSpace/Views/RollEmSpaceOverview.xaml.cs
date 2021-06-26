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
    public partial class RollEmSpaceOverview : UserControl
    {
        private readonly IRegionManager _regionManager;
        private readonly RollEmSpaceOverviewViewModel _rollEmSpaceOverviewViewModel;
        public RollEmSpaceOverview(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
            _rollEmSpaceOverviewViewModel = this.DataContext as RollEmSpaceOverviewViewModel;
        }

        private void SelectDice_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var parameters = _rollEmSpaceOverviewViewModel.Parameters;
            var dice = (sender as ListView).SelectedItem;
            //DialogCoordinator.Instance),selectedContact
            if (dice != null)
            {
                //DiceViewModel toAdd = (DiceViewModel) dice;
                parameters.Add("selectedDice", (DiceViewModel)dice);
                //parameters.Add("diceListViewModel", _diceOverviewViewModel.DiceListViewModel);
                parameters.Add("groupedDiceView", _rollEmSpaceOverviewViewModel.GroupedDiceView);
                parameters.Add("regionManager", _rollEmSpaceOverviewViewModel.RegionManager);
                parameters.Add("ideaDataService", _rollEmSpaceOverviewViewModel.IdeaDataService);

                //DiceListViewModel selectedDice = parameters["diceListViewModel"] as DiceListViewModel;
                //_regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceDetail), parameters);
                _regionManager.Regions[RegionNames.MainContentRegion].RemoveAll();
                _regionManager.RequestNavigate(RegionNames.LeftBottomContentRegion, nameof(RollEmSpaceDetail), parameters);
                e.Handled = true;
            }
        }
    }
}
