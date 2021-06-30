using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dicidea.Core.Constants;
using IdeaPage.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using Prism.Regions;

namespace IdeaPage.Views
{
    public partial class IdeaOverview : UserControl
    {
        private readonly IRegionManager _regionManager;
        private readonly IdeaOverviewViewModel _ideaOverviewViewModel;
        public IdeaOverview(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
            _ideaOverviewViewModel = this.DataContext as IdeaOverviewViewModel;
        }

        private void IdeaOverview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var parameters = new NavigationParameters
            {
                { "diceListViewModel", _ideaOverviewViewModel.Parameters["diceListViewModel"] },
                { "ideaListViewModel", _ideaOverviewViewModel.Parameters["ideaListViewModel"] },
                { "ideaDataService", _ideaOverviewViewModel.Parameters["ideaDataService"] },
                { "diceDataService", _ideaOverviewViewModel.Parameters["diceDataService"] }
            }; ;
            //DialogCoordinator.Instance),selectedContact
            if ((sender as ListView).SelectedItem is IdeaViewModel idea)
            {
                //DiceViewModel toAdd = (DiceViewModel) dice;
                parameters.Add("selectedIdea", idea);
                //parameters.Add("diceListViewModel", _diceOverviewViewModel.DiceListViewModel);
                parameters.Add("groupedIdeaView", _ideaOverviewViewModel.GroupedIdeaView);
                parameters.Add("regionManager", _ideaOverviewViewModel.RegionManager);

                //DiceListViewModel selectedDice = parameters["diceListViewModel"] as DiceListViewModel;
                //_regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceDetail), parameters);
                _regionManager.Regions[RegionNames.MainContentRegion].RemoveAll();
                _regionManager.RequestNavigate(RegionNames.LeftBottomContentRegion, nameof(IdeaDetail), parameters);
                e.Handled = true;
            }
        }
    }
}
