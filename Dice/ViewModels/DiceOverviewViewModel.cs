using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Dicidea.Core.Converters;
using Dicidea.Core.Models;
using Dicidea.Core.Helper;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace DicePage.ViewModels
{
    /// <summary>
    /// ViewModel für den DiceOverview. Verwendet das DiceListViewModel als
    /// Datenquelle, verwendet aber einen <see cref="ListCollectionView" /> für das Binden an die ListView um später einfach
    /// Sortieren und Filtern zu können.
    /// </summary>
    public class DiceOverviewViewModel : NotifyPropertyChanges, INavigationAware
    {
        private string _filterText;
        private SortOrder _sortOrder = SortOrder.Unsorted;
        private ListCollectionView _groupedDiceView;
        private DiceListViewModel _diceListViewModel;
        private readonly IDialogService _dialogService;
        private bool _showSaved;
        private bool _isSaving;
        /// <summary>
        /// Erhält den RegionManager und den DialogManager und setzt das Sort-, Save- und DeleteCommand
        /// </summary>
        /// <param name="dialogService"></param>
        /// <param name="regionManager"></param>
        public DiceOverviewViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _dialogService = dialogService;
            RegionManager = regionManager;
            SortCommand = new DelegateCommand(SortExecute);
            AddCommand = new DelegateCommand(AddExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            SaveCommand = new DelegateCommand(SaveExecute);
        }
        /// <summary>
        /// Bool zum anzeigen und ausblenden der Saved Anzeige
        /// </summary>
        public bool ShowSaved
        {
            get => _showSaved;
            set => SetProperty(ref _showSaved, value);
        }
        /// <summary>
        /// Bool zum anzeigen der Saving Anzeige
        /// </summary>
        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
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
        /// Zwischengespeichertes DiceistViewModel
        /// </summary>
        public DiceListViewModel DiceListViewModel
        {
            get => _diceListViewModel;
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
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        public IRegionManager RegionManager { get; }
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
        /// Ausgewählter Würfel
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
        /// Der gruppierte <see cref="ListCollectionView" />, nach dem Anfangsbuchstaben des Würfelnamens gruppiert.
        /// </summary>
        public ListCollectionView GroupedDiceView
        {
            get => _groupedDiceView;
            set => SetProperty(ref _groupedDiceView, value);
        }
        /// <summary>
        /// Aktualisiert die Liste wenn der Name geändert wurde.
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
        /// Unsortierte Liste
        /// </summary>
        private void Unsort()
        {
            GroupedDiceView.IsLiveSorting = false;
            GroupedDiceView.CustomSort = null;
        }
        /// <summary>
        /// Aufsteigende Sortierung
        /// </summary>
        private void SortAscending()
        {
            GroupedDiceView.IsLiveSorting = true;
            GroupedDiceView.CustomSort = Comparer<DiceViewModel>.Create((d1, d2) => string.Compare(d1.Dice.Name, d2.Dice.Name, StringComparison.OrdinalIgnoreCase));
        }
        /// <summary>
        /// Absteigende Sortierung
        /// </summary>
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
        /// Zum Hinzufügen eines neuen Würfels
        /// </summary>
        private async void AddExecute()
        {
            DiceViewModel result = await _diceListViewModel.AddDiceAsync();
            GroupedDiceView.MoveCurrentTo(result);
            GroupedDiceView.Refresh();

        }
        /// <summary>
        /// Zum Speichern der Würfel
        /// </summary>
        private async void SaveExecute()
        {
            IsSaving = true;
            //await Task.Delay(3000);
            await _diceListViewModel.SaveDiceAsync();
            IsSaving = false;
            ShowSaved = true;
            await Task.Delay(3000);
            ShowSaved = false;
        }
        /// <summary>
        /// Zum Löschen eines Würfels.
        /// Wird auf den Löschen Button geklickt wird ein Dialog aufgerufen um zu fragen ob der Würfel gelöscht werden soll
        /// </summary>
        private async void DeleteExecute()
        {
            var selectedDice = SelectedDice;
            bool delete = false;
            if (selectedDice == null) return;
            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete dice?" },
                    { "message", $"Do you really want to delete the dice '{selectedDice.Dice.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if (!delete) return;
            await _diceListViewModel.DeleteDiceAsync(SelectedDice);
            GroupedDiceView.Refresh();
        }

        /// <summary>
        /// Funktion um das diceListViewModel in einen ListCollectionView umzuwandeln. Dieser wird zur gruppierten Darstellung der Würfel benötigt.
        /// </summary>
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
        /// Wird aufgerufen wenn zu dieser Seite navigiert wird. Die übergebenen Parameter werden zwischengespeichert
        /// und eine gruppierte Liste der Würfel erzeugt.
        /// </summary>
        /// <param name="navigationContext">NavigationContext der die NavigationParameter beinhaltet.</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Parameters = navigationContext.Parameters;
            if (navigationContext.Parameters["diceListViewModel"] != null)
            {
                _diceListViewModel = navigationContext.Parameters["diceListViewModel"] as DiceListViewModel;
                CreateGroupedView();
            }
            SortCommand = new DelegateCommand(SortExecute);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {}
    }
}
