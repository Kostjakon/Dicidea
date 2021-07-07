using System;
using System.Windows.Controls;
using System.Windows.Input;
using DicePage.ViewModels;
using Dicidea.Core.Constants;
using Prism.Regions;

namespace DicePage.Views
{
    /// <summary>
    /// Interaction logic for DiceOverview.xaml
    /// </summary>
    public partial class DiceOverview
    {
        private readonly IRegionManager _regionManager;
        private readonly DiceOverviewViewModel _diceOverviewViewModel;
        /// <summary>
        /// Speichert den RegionManager zwischen und holt sich das DiceOverviewViewModel
        /// </summary>
        /// <param name="regionManager">Zum Navigieren benötigt</param>
        public DiceOverview(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
            _diceOverviewViewModel = this.DataContext as DiceOverviewViewModel;
        }
        /// <summary>
        /// Funktion die aufgerufen wird wenn in der DiceOverview.xaml auf einen Würfel gedoppelklickt wird.
        /// Hier wird die aktuelle Liste der Würfel, die aktuelle Liste der Ideen, die zwei DataServices und der angeklickte Würfel als Parameter gespeichert.
        /// Außerdem wird der gedoppelklickte Würfel als Parameter gespeichert und im Anschluss zur DiceDetail Seite navigiert.
        /// </summary>
        /// <param name="sender">Doppelgeklickter Würfel als DiceViewModel</param>
        /// <param name="e"></param>
        private void DiceOverview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var parameters = new NavigationParameters
            {
                { "diceListViewModel", _diceOverviewViewModel.Parameters["diceListViewModel"] },
                { "ideaListViewModel", _diceOverviewViewModel.Parameters["ideaListViewModel"] },
                { "ideaDataService", _diceOverviewViewModel.Parameters["ideaDataService"] },
                { "diceDataService", _diceOverviewViewModel.Parameters["diceDataService"] }
            };
            var dice = (sender as ListView)?.SelectedItem;
            if (dice != null)
            {
                parameters.Add("selectedDice", (DiceViewModel)dice);
                parameters.Add("groupedDiceView", _diceOverviewViewModel.GroupedDiceView);
                parameters.Add("regionManager", _diceOverviewViewModel.RegionManager);
                _regionManager.Regions[RegionNames.MainContentRegion].RemoveAll();
                _regionManager.RequestNavigate(RegionNames.LeftBottomContentRegion, nameof(DiceDetail), parameters);
                e.Handled = true;
            }
            
        }
        /// <summary>
        /// Event das automatisch aufgerufen wird wenn etwas zur ScrollView hinzugefügt oder entfernt wird.
        /// Die View wird daraufhin nach oben gescrollt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiceScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer scrollViewer && Math.Abs(e.ExtentHeightChange)>0.0)
            {
                scrollViewer.ScrollToTop();
            }
        }
    }
}
