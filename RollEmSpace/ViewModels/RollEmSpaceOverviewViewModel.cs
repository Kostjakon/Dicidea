using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using DicePage.ViewModels;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Prism.Commands;
using Prism.Regions;

namespace RollEmSpacePage.ViewModels
{
    /// <summary>
    /// ViewModel für den RollEmSpaceOverview. Verwendet das <see cref="DiceListViewModel" /> als
    /// Datenquelle, verwendet aber einen <see cref="ListCollectionView" /> für das Binden an die ListView um später einfach
    /// Sortieren und Filtern zu können.
    /// </summary>
    public class RollEmSpaceOverviewViewModel : NotifyPropertyChanges, INavigationAware
    {
        private string _filterText;
        private SortOrder _sortOrder = SortOrder.Unsorted;
        private ListCollectionView _groupedDiceView;
        private DiceListViewModel _diceListViewModel;

        public RollEmSpaceOverviewViewModel()
        {
            SortCommand = new DelegateCommand(SortExecute);
        }
        
        /// <summary>
        /// Zwischengespeicherte NavigationParameter
        /// </summary>
        public NavigationParameters Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// Die verschiedenen Sortiermöglichkeiten
        /// </summary>
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
        
        /// <summary>
        /// Der Text nach dem gefiltert werden soll
        /// </summary>
        public string FilterText
        {
            get => _filterText;
            set
            {
                SetProperty(ref _filterText, value);
                Filter();
            }
        }
        /// <summary>
        /// Der ausgewählte Würfel
        /// </summary>
        public DiceViewModel SelectedDice
        {
            get
            {
                if (GroupedDiceView != null) return GroupedDiceView.CurrentItem as DiceViewModel;
                else return null;
            }
            set => GroupedDiceView.MoveCurrentTo(value);
        }
        /// <summary>
        ///     Der gruppierte <see cref="ListCollectionView" />, nach dem Anfangsbuchstaben des Würfelnamens gruppiert.
        /// </summary>
        public ListCollectionView GroupedDiceView
        {
            get => _groupedDiceView;
            set => SetProperty(ref _groupedDiceView, value);
        }

        /// <summary>
        ///     Aktualisiert die Liste wenn der Name geändert wurde.
        /// </summary>
        /// <param name="propertyName">Name der geänderten Property</param>
        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Dice.Name))
            {
                GroupedDiceView.Refresh();
            }
        }

        /// <summary>
        /// Sortierungen
        /// </summary>
        private void Unsort()
        {
            GroupedDiceView.IsLiveSorting = false;
            GroupedDiceView.CustomSort = null;
        }
        private void SortAscending()
        {
            GroupedDiceView.IsLiveSorting = true;
            GroupedDiceView.CustomSort = Comparer<DiceViewModel>.Create((d1, d2) => string.Compare(d1.Dice.Name, d2.Dice.Name, StringComparison.OrdinalIgnoreCase));
        }
        private void SortDescending()
        {
            GroupedDiceView.IsLiveSorting = true;
            GroupedDiceView.CustomSort = Comparer<DiceViewModel>.Create((d1, d2) => string.Compare(d2.Dice.Name, d1.Dice.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Funktion für das Sort Command. Noch nicht in der View implementiert.
        /// </summary>
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

        /// <summary>
        /// Funktion um das diceListViewModel in einen ListCollectionView umzuwandeln. Diese wird zur gruppierten Darstellung der Würfel benötigt.
        /// </summary>
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
            if (GroupedDiceView.GroupDescriptions != null)
                GroupedDiceView.GroupDescriptions.Add(new PropertyGroupDescription
                {
                    PropertyName = propertyName,
                    Converter = new NameToInitialConverter()
                });
            GroupedDiceView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedDice));
        }

        /// <summary>
        /// Funktion zum Filtern der Würfel. Noch nicht implementiert!
        /// </summary>
        private void Filter()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                GroupedDiceView.Filter = o => true;
            }
            else
            {
                GroupedDiceView.IsLiveFiltering = true;
                GroupedDiceView.Filter = o =>
                {
                    if (o is DiceViewModel vm)
                        return vm.Dice.Name?.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0;
                    return true;
                };
            }
        }

        /// <summary>
        /// Wird aufgerufen wenn zu dieser Seite navigiert wird. Die übergebenen Parameter werden zwischengespeichert, das übergebene DiceListViewModel gesetzt und die gruppierte Liste erzeugt
        /// </summary>
        /// <param name="navigationContext">NavigationContext der die NavigationParameter beinhaltet.</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Parameters = navigationContext.Parameters;
            if (navigationContext.Parameters["diceListViewModel"] != null)
            {
                // Datenquelle
                _diceListViewModel = navigationContext.Parameters["diceListViewModel"] as DiceListViewModel;
                CreateGroupedDiceView();
            }
            SortCommand = new DelegateCommand(SortExecute);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }



        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
