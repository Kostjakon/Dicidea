using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Converters;
using Dicidea.Core.Models;
using Dicidea.Core.Helper;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace DicePage.ViewModels
{
    /// <summary>
    /// ViewModel für den <see cref="DiceDetail" />. Verwendet das <see cref="DiceListViewModel" /> als
    /// Datenquelle, verwendet aber einen <see cref="ListCollectionView" /> für das Binden an die ListView.
    /// </summary>
    public class DiceDetailViewModel : NotifyPropertyChanges, INavigationAware
    {
        private DiceListViewModel _diceListViewModel;
        private ListCollectionView _groupedDiceView;
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        private bool _showSaved;
        private bool _isSaving;
        /// <summary>
        /// Erzeugt die verschiedenen Commands und erhält und setzt den RegionManager und den DialogService
        /// </summary>
        /// <param name="regionManager">Benötigt zum navigieren</param>
        /// <param name="dialogService">Benötigt um Dialoge zu erzeugen</param>
        public DiceDetailViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            GoToDiceOverviewCommand = new DelegateCommand<object>(GoToDiceOverview, CanGoToDiceOverview);
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
        public ICommand GoToDiceOverviewCommand { get; }

        private bool CanGoToDiceOverview(object obj)
        {
            return true;
        }
        /// <summary>
        /// Zwischengespeicherte Navigation Parameter
        /// </summary>
        public NavigationParameters Parameters
        {
            get;
            set;
        }
        /// <summary>
        /// Zum Navigieren zur Würfel Übersicht. Es wird die Parameterliste mit übergeben.
        /// </summary>
        private void GoToDiceOverview(object obj)
        {
            Parameters.Add("diceListViewModel", _diceListViewModel);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceOverview), Parameters);
            _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
        }

        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        /// <summary>
        /// Zum Hinzufügen einer Kategorie zum ausgewählten Würfel
        /// </summary>
        private async void AddExecute()
        {
            await SelectedDice.AddCategoryAsync();
        }
        /// <summary>
        /// Zum Löschen eines Würfels.
        /// Wird auf den Löschen Button geklickt wird ein Dialog aufgerufen um zu fragen ob der Würfel gelöscht werden soll
        /// </summary>
        private async void DeleteExecute()
        {
            var selectedCategory = SelectedDice.SelectedCategory;
            bool delete = false;
            if (selectedCategory == null) return;
            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete category?" },
                    { "message", $"Do you really want to delete the category '{selectedCategory.Category.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if (!delete) return;
            await SelectedDice.DeleteCategoryAsync();
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

        public IRegionManager RegionManager { get; private set; }
        /// <summary>
        /// Die gruppierte Liste der Würfel.
        /// </summary>
        public ListCollectionView GroupedDiceView
        {
            get => _groupedDiceView;
            set => SetProperty(ref _groupedDiceView, value);
        }
        /// <summary>
        /// Ausgewählter Würfel
        /// </summary>
        public DiceViewModel SelectedDice
        {
            get
            {
                if (GroupedDiceView != null) return GroupedDiceView.CurrentItem as DiceViewModel;
                return null;
            }
            set => GroupedDiceView.MoveCurrentTo(value);
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
                SortDescriptions = {new SortDescription(propertyName, ListSortDirection.Ascending)}
            };
            GroupedDiceView.GroupDescriptions?.Add(new PropertyGroupDescription
            {
                PropertyName = propertyName,
                Converter = new NameToInitialConverter()
            });
            GroupedDiceView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedDice));
        }
        /// <summary>
        /// Wird aufgerufen wenn zu dieser Seite navigiert wird. Die übergebenen Parameter werden zwischengespeichert
        /// und eine gruppierte Liste der Würfel erzeugt. Außerdem wird der übergebene Würfel in der Liste der Würfel ausgewählt.
        /// </summary>
        /// <param name="navigationContext">NavigationContext der die NavigationParameter beinhaltet.</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext != null)
            {
                Parameters = navigationContext.Parameters;
                if(navigationContext.Parameters["diceListViewModel"] != null)
                {
                    _diceListViewModel = navigationContext.Parameters["diceListViewModel"] as DiceListViewModel;
                    CreateGroupedView();
                    if (navigationContext.Parameters["selectedDice"] != null)
                    {
                        DiceViewModel selectedDice = navigationContext.Parameters["selectedDice"] as DiceViewModel;
                        GroupedDiceView.MoveCurrentTo(selectedDice);
                        GroupedDiceView.Refresh();
                    }
                    else
                    {
                        GroupedDiceView.MoveCurrentToFirst();
                        GroupedDiceView.Refresh();
                        if (GroupedDiceView.Count > 0) Parameters.Add("selectedDice", GroupedDiceView.GetItemAt(0));
                    }
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
        {}
    }
}
