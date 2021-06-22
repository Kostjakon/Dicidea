using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Prism.Regions;

namespace DicePage.ViewModels
{
    public class DiceDetailListViewModel : NotifyPropertyChanges, INavigationAware
    {
        private DiceListViewModel _diceListViewModel;
        //private readonly IDialogCoordinator _dialogCoordinator;
        private ListCollectionView _groupedDiceView;
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;
        public DiceDetailListViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            GoToDiceOverviewCommand = new DelegateCommand<object>(GoToDiceOverview, CanGoToDiceOverview);
            SelectDiceCommand = new DelegateCommand<object>(SelectDice, CanSelectDice);
        }

        public ICommand GoToDiceOverviewCommand { get; private set; }

        private bool CanGoToDiceOverview(object obj)
        {
            return true;
        }

        private void GoToDiceOverview(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceOverview), _parameters);
            _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
        }

        public ICommand SelectDiceCommand { get; private set; }

        private bool CanSelectDice(object obj)
        {
            return true;
        }

        private void SelectDice(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceDetail), _parameters);
        }
        public IRegionManager RegionManager { get; private set; }
        public ListCollectionView GroupedDiceView
        {
            get => _groupedDiceView;
            set => SetProperty(ref _groupedDiceView, value);
        }

        public bool IsEditEnabled => GroupedDiceView.CurrentItem != null;

        public DiceViewModel SelectedDice
        {
            get
            {
                if (GroupedDiceView != null) return GroupedDiceView.CurrentItem as DiceViewModel;
                else return null;
            }
            set => GroupedDiceView.MoveCurrentTo(value);
        }

        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Dice.Name))
            {
                GroupedDiceView.Refresh();
            }
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
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            GroupedDiceView.GroupDescriptions.Add(new PropertyGroupDescription
            {
                PropertyName = propertyName,
                Converter = new NameToInitialConverter()
            });
            GroupedDiceView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedDice));
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext != null)
            {
                _parameters = navigationContext.Parameters;
                if (navigationContext.Parameters["diceListViewModel"] != null)
                {
                    _diceListViewModel = navigationContext.Parameters["diceListViewModel"] as DiceListViewModel;
                    CreateGroupedView();
                }

                if (navigationContext.Parameters["selectedDice"] != null)
                {
                    DiceViewModel selectedDice = navigationContext.Parameters["selectedDice"] as DiceViewModel;
                    Debug.WriteLine("Is selected Dice the same Dice as the Dice in dice list? " + (_diceListViewModel.AllDice[0].Dice == selectedDice.Dice));
                    GroupedDiceView.MoveCurrentTo(selectedDice);
                }

                if (navigationContext.Parameters["regionManager"] != null)
                {
                    RegionManager = navigationContext.Parameters["regionManager"] as IRegionManager;
                }
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }



        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Debug.WriteLine("Not implemented, navigated from DiceDetail to some other side");
        }
    }
}
