using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DicePage.ViewModels;
using Dicidea.Core.Constants;
using Dicidea.Core.Models;
using Prism.Regions;

namespace DicePage.Views
{
    /// <summary>
    /// Interaction logic for DiceOverview.xaml
    /// </summary>
    public partial class DiceOverview : UserControl
    {
        private readonly IRegionManager _regionManager;
        private readonly DiceOverviewViewModel _diceOverviewViewModel;
        public DiceOverview(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
            _diceOverviewViewModel = this.DataContext as DiceOverviewViewModel;
        }

        private void DiceOverview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var parameters = _diceOverviewViewModel.Parameters;
            var dice = (sender as ListView).SelectedItem;
            //DialogCoordinator.Instance),selectedContact
            if (dice != null)
            {
                //DiceViewModel toAdd = (DiceViewModel) dice;
                parameters.Add("selectedDice", (DiceViewModel)dice);
                //parameters.Add("diceListViewModel", _diceOverviewViewModel.DiceListViewModel);
                parameters.Add("groupedDiceView", _diceOverviewViewModel.GroupedDiceView);
                parameters.Add("regionManager", _diceOverviewViewModel.RegionManager);

                //DiceListViewModel selectedDice = parameters["diceListViewModel"] as DiceListViewModel;
                //_regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceDetail), parameters);
                _regionManager.Regions[RegionNames.MainContentRegion].RemoveAll();
                _regionManager.RequestNavigate(RegionNames.LeftBottomContentRegion, nameof(DiceDetail), parameters);
                e.Handled = true;
            }
            
        }
        
    }
}
