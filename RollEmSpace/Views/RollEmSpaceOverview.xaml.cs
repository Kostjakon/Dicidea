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
            //var parameters = _rollEmSpaceOverviewViewModel.Parameters;
            var parameters = new NavigationParameters
            {
                { "diceListViewModel", _rollEmSpaceOverviewViewModel.Parameters["diceListViewModel"] },
                { "ideaListViewModel", _rollEmSpaceOverviewViewModel.Parameters["ideaListViewModel"] },
                { "ideaDataService", _rollEmSpaceOverviewViewModel.Parameters["ideaDataService"] },
                { "diceDataService", _rollEmSpaceOverviewViewModel.Parameters["diceDataService"] },
            };
            var dice = (sender as ListView).SelectedItem as DiceViewModel;
            if (dice != null) Debug.WriteLine(dice.Dice.Name);
            //DialogCoordinator.Instance),selectedContact
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
