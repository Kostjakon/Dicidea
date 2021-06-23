using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Converters;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Dicidea.Core.Helper;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace DicePage.ViewModels
{
    public class DiceOverviewViewModel : NotifyPropertyChanges, INavigationAware
    {
        private string _filterText;
        private SortOrder _sortOrder = SortOrder.Unsorted;
        private ListCollectionView _groupedDiceView;
        private DiceListViewModel _diceListViewModel;
        private readonly IRegionManager _regionManager;

        public DiceOverviewViewModel(IRegionManager regionManager)
        {
            
            _regionManager = regionManager;
            if (DiceView != null)
            {
                DiceView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedDice));
                DiceView.Refresh();
            }
            SortCommand = new DelegateCommand(SortExecute);
            AddCommand = new DelegateCommand(AddExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            SaveCommand = new DelegateCommand(SaveExecute);
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
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

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

        private async void AddExecute()
        {
            DiceViewModel result = await _diceListViewModel.AddDiceAsync();
            GroupedDiceView.MoveCurrentTo(result);
            GroupedDiceView.Refresh();

        }
        private async void SaveExecute()
        {
            await _diceListViewModel.SaveDiceAsync();
        }

        private async void DeleteExecute()
        {
            Debug.WriteLine("Delete Dice");
            await _diceListViewModel.DeleteDiceAsync(SelectedDice);
            GroupedDiceView.Refresh();
        }


        private void CreateGroupedView()
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
            Debug.WriteLine("Navigated to Dice Overview");
            if (navigationContext != null)
            {
                Debug.WriteLine("Navigation Context is not null");
                Debug.WriteLine(navigationContext.Parameters);
                if (navigationContext.Parameters["diceListViewModel"] != null)
                {
                    Debug.WriteLine("diceListViewModel is not null");
                    _diceListViewModel = navigationContext.Parameters["diceListViewModel"] as DiceListViewModel;
                    //DiceView = new ListCollectionView(DiceListViewModel.AllDice);
                    //DiceView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedDice));
                    CreateGroupedView();
                    //if (DiceView.Count >0) DiceView.Refresh();
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
