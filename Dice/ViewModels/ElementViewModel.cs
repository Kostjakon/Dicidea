using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace DicePage.ViewModels
{
    /// <summary>
    /// Kapselt das Element einer Kategorie, fügt UI-spezifische Eigenschaften hinzu (Löschen des Elements, hinzufügen eines Werts)
    /// und erstellt eine ListCollectionView der Werte des Würfels.
    /// </summary>
    public class ElementViewModel : NotifyPropertyChanges
    {
        private readonly CategoryViewModel _categoryViewModel;
        private readonly IDialogService _dialogService;
        private readonly ValueListViewModel _valueListViewModel;
        private ListCollectionView _groupedValuesView;
        private readonly DiceViewModel _diceViewModel;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        /// <summary>
        /// Erzeugt die gruppierte Liste der Werte des Elements und setzt das EditCommand, das AddCommand, das ActivateCommand und das DeleteCommand.
        /// </summary>
        /// <param name="element">Element für den das ElementViewModel erstellt werden soll</param>
        /// <param name="categoryViewModel">Kategorie zu der das Element gehört, wird benötigt zum löschen eines Elements und zur Auswahl des Inhalts auf der Rückseite der Karten</param>
        /// <param name="diceViewModel">Würfel zur Auswahl des Inhalts auf der Rückseite der Karten benötigt</param>
        /// <param name="diceDataService">Wird zur Weitergabe an das ValueListViewModel benötigt</param>
        /// <param name="dialogService">Wird zum Erzeugen eines Dialogs benötigt</param>
        public ElementViewModel(Element element, CategoryViewModel categoryViewModel, DiceViewModel diceViewModel, IDiceDataService diceDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _categoryViewModel = categoryViewModel;
            _diceViewModel = diceViewModel;
            FlipCommand = new DelegateCommand<object>(Flip, CanFlip);
            Element = element;
            EditCommand = new DelegateCommand(EditExecute);
            AddCommand = new DelegateCommand(AddExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            ActivateCommand = new DelegateCommand(ActivateExecute);
            ElementViewModel self = this;
            if (_valueListViewModel == null)
            {
                _valueListViewModel = new ValueListViewModel(self, diceDataService, _dialogService);
            }
            CreateGroupedView();
        }
        /// <summary>
        /// Gruppierte Liste der Werte
        /// </summary>
        public ListCollectionView GroupedValuesView
        {
            get => _groupedValuesView;
            set => SetProperty(ref _groupedValuesView, value);
        }
        /// <summary>
        /// Ausgewählter Wert
        /// </summary>
        public ValueViewModel SelectedValue
        {
            get
            {
                if (GroupedValuesView != null) return GroupedValuesView.CurrentItem as ValueViewModel;
                return null;
            }
            set => GroupedValuesView.MoveCurrentTo(value);
        }
        /// <summary>
        /// Aktualisiert die Liste wenn der Name geändert wurde.
        /// </summary>
        /// <param name="propertyName">Name der geänderten Property</param>
        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Element.Name))
            {
                //GroupedElementsView.Refresh();
            }
        }
        /// <summary>
        /// Funktion um das valueListViewModel in einen ListCollectionView umzuwandeln. Diese wird zur gruppierten Darstellung der Werte benötigt.
        /// </summary>
        private void CreateGroupedView()
        {
            ObservableCollection<ValueViewModel> valueViewModels = _valueListViewModel.Values;
            foreach (var valueViewModel in valueViewModels)
            {
                valueViewModel.Value.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "Element.Name";
            GroupedValuesView = new ListCollectionView(valueViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            if (GroupedValuesView.GroupDescriptions != null)
                GroupedValuesView.GroupDescriptions.Add(new PropertyGroupDescription
                {
                    PropertyName = propertyName,
                    Converter = new NameToInitialConverter()
                });
            GroupedValuesView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedValue));
        }
        /// <summary>
        /// Bool zum anzeigen und ausblenden der Textboxen
        /// </summary>
        public bool IsEditEnabled
        {
            get => _isEditEnabled;
            set => SetProperty(ref _isEditEnabled, value);
        }
        /// <summary>
        /// Bool zum anzeigen und ausblenden der Textblöcke
        /// </summary>
        public bool IsEditDisabled
        {
            get => _isEditDisabled;
            set => SetProperty(ref _isEditDisabled, value);
        }

        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand ActivateCommand { get; set; }
        /// <summary>
        /// Zum Aktivsetzen von Elementen
        /// </summary>
        public void ActivateExecute()
        {
            Element.Active = !Element.Active;
        }
        /// <summary>
        /// Zum Editieren von Elementen
        /// </summary>
        public void EditExecute()
        {
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;
        }
        /// <summary>
        /// Zum Hinzufügen von Elementen
        /// </summary>
        public async void AddExecute()
        {
            await AddValueAsync();
        }
        /// <summary>
        /// Zum Löschen von Elementen. Wird auf den Button geklickt wird zuerst ein Dialog angezeigt
        /// der mit Ja bestätigt werden muss, erst dann wird das Element gelöscht.
        /// </summary>
        public async void DeleteExecute()
        {
            var selectedElement = this;
            bool delete = false;
            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete element?" },
                    { "message", $"Do you really want to delete the element '{selectedElement.Element.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if (!delete) return;
            _categoryViewModel.SelectedElement = selectedElement;
            await _categoryViewModel.DeleteElementAsync();
        }
        /// <summary>
        /// Gibt an ob dieses Element in der Kategorie ausgewählt ist
        /// </summary>
        public bool IsSelected
        {
            get => _categoryViewModel.SelectedElement == this;
        }
        /// <summary>
        /// Die Category zu der das Element gehört
        /// </summary>
        public Category Category
        {
            get => _categoryViewModel.Category;
        }
        /// <summary>
        /// Der Würfel zu dem das Element gehört
        /// </summary>
        public Dice Dice
        {
            get => _diceViewModel.Dice;
        }
        /// <summary>
        /// Das Element das zum ElementViewModel gehört
        /// </summary>
        public Element Element { get; }
        public ICommand FlipCommand { get; set; }

        private bool CanFlip(object obj)
        {
            return true;
        }
        /// <summary>
        /// Zum Hinzufügen eines Werts
        /// </summary>
        /// <returns></returns>
        public async Task AddValueAsync()
        {
            await _valueListViewModel.AddValueAsync();
            //GroupedCategoriesView.Refresh();
        }
        /// <summary>
        /// Zum Löschen eines Werts
        /// </summary>
        /// <returns></returns>
        public async Task DeleteValueAsync()
        {
            await _valueListViewModel.DeleteValueAsync(SelectedValue);
            GroupedValuesView.Refresh();
        }
        /// <summary>
        /// Zum flippen der Karte
        /// </summary>
        /// <param name="obj"></param>
        private void Flip(object obj)
        {
            _diceViewModel.SelectedElement = this;
            _categoryViewModel.SelectedElement = this;
            _diceViewModel.SelectedCategory = _categoryViewModel;
        }
    }
}
