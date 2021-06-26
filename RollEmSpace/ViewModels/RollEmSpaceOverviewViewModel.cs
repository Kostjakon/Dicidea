using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using DicePage.ViewModels;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using IdeaPage.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace RollEmSpacePage.ViewModels
{
    public class RollEmSpaceOverviewViewModel : NotifyPropertyChanges, INavigationAware
    {
        private string _filterText;
        private SortOrder _sortOrder = SortOrder.Unsorted;
        private ListCollectionView _groupedDiceView;
        private DiceListViewModel _diceListViewModel;
        private IIdeaDataService _ideaDataService;
        private readonly IRegionManager _regionManager;

        public RollEmSpaceOverviewViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            if (DiceView != null)
            {
                DiceView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedDice));
                DiceView.Refresh();
            }
            SortCommand = new DelegateCommand(SortExecute);
        }

        public bool IsEditEnabled
        {
            get;
            private set;
        }

        public NavigationParameters Parameters
        {
            get;
            set;
        }

        public DiceListViewModel DiceListViewModel
        {
            get => _diceListViewModel;
        }

        public SortOrder SortOrder
        {
            get => _sortOrder;
            set
            {
                SetProperty(ref _sortOrder, value);
                switch (value)
                {
                    case SortOrder.Ascending:
                        SortAscending();
                        break;
                    case SortOrder.Descending:
                        SortDescending();
                        break;
                    case SortOrder.Unsorted:
                        Unsort();
                        break;
                }
            }
        }

        public DelegateCommand SortCommand { get; set; }

        public ListCollectionView DiceView { get; private set; }

        public IRegionManager RegionManager { get => _regionManager; }

        public string FilterText
        {
            get => _filterText;
            set
            {
                SetProperty(ref _filterText, value);
                Filter();
            }
        }

        public IIdeaDataService IdeaDataService
        {
            get => _ideaDataService;
            set => SetProperty(ref _ideaDataService, value);
        }

        public DiceViewModel SelectedDice
        {
            get
            {
                if (GroupedDiceView != null) return GroupedDiceView.CurrentItem as DiceViewModel;
                else return null;
            }
            set => GroupedDiceView.MoveCurrentTo(value);
        }

        public ListCollectionView GroupedDiceView
        {
            get => _groupedDiceView;
            set => SetProperty(ref _groupedDiceView, value);
        }

        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Dice.Name))
            {
                GroupedDiceView.Refresh();
            }
        }

        private void Unsort()
        {
            DiceView.IsLiveSorting = false;
            DiceView.CustomSort = null;
        }

        private void SortAscending()
        {
            DiceView.IsLiveSorting = true;
            DiceView.CustomSort = Comparer<DiceViewModel>.Create((d1, d2) => string.Compare(d1.Dice.Name, d2.Dice.Name, StringComparison.OrdinalIgnoreCase));
        }

        private void SortDescending()
        {
            DiceView.IsLiveSorting = true;
            DiceView.CustomSort = Comparer<DiceViewModel>.Create((d1, d2) => string.Compare(d2.Dice.Name, d1.Dice.Name, StringComparison.OrdinalIgnoreCase));
        }

        private void SortExecute()
        {
            switch (SortOrder)
            {
                case SortOrder.Descending:
                    SortOrder = SortOrder.Unsorted;
                    break;
                case SortOrder.Unsorted:
                    SortOrder = SortOrder.Ascending;
                    break;
                case SortOrder.Ascending:
                    SortOrder = SortOrder.Descending;
                    break;
            }
        }


        private void CreateGroupedDiceView()
        {
            ObservableCollection<DiceViewModel> diceViewModels = _diceListViewModel.AllDice;
            foreach (var diceViewModel in diceViewModels)
            {
                diceViewModel.Dice.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "Dice.Name";
            GroupedDiceView = new ListCollectionView(diceViewModels)
            {
                IsLiveSorting = true,
                IsLiveGrouping = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            GroupedDiceView.GroupDescriptions.Add(new PropertyGroupDescription
            {
                PropertyName = propertyName,
                Converter = new NameToInitialConverter()
            });
            GroupedDiceView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedDice));
        }

        private void Filter()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                DiceView.Filter = o => true;
            }
            else
            {
                DiceView.IsLiveFiltering = true;
                DiceView.Filter = o =>
                {
                    if (o is DiceViewModel vm)
                        return vm.Dice.Name?.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0;
                    return true;
                };
            }
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Parameters = navigationContext.Parameters;
            Debug.WriteLine("Navigated to RollEmSpace");
            if (navigationContext != null)
            {
                Debug.WriteLine("Navigationcontext is not null");
                if (navigationContext.Parameters["diceListViewModel"] != null)
                {
                    Debug.WriteLine("DicelistView is not null");
                    _diceListViewModel = navigationContext.Parameters["diceListViewModel"] as DiceListViewModel;
                    CreateGroupedDiceView();
                    Debug.WriteLine(_diceListViewModel.AllDice.First().Dice.Name);
                }
                if (navigationContext.Parameters["ideaDataService"] != null)
                {
                    _ideaDataService = navigationContext.Parameters["ideaDataService"] as IIdeaDataService;
                }
                SortCommand = new DelegateCommand(SortExecute);
            }
            else
            {
                //Message = $"Dice Active from your Prism Module, Dice Anzahl: {AllDice.Count}! Es wurde kein Würfel ausgewählt";
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }



        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Debug.WriteLine("Not implemented, navigated from DiceOverview to some other side");
        }
    }
}
