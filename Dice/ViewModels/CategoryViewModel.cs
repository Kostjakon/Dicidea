using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Kapselt die Kategorie eines Würfels, fügt UI-spezifische Eigenschaften hinzu (Löschen der Kategorie, Editieren der Kategorie, Aktivieren der Kategorie, Hinzufügen eines neuen Elements)
    /// und erstellt eine ListCollectionView der Kategorien des Würfels.
    /// </summary>
    public class CategoryViewModel : NotifyPropertyChanges
    {
        private readonly ElementListViewModel _elementListViewModel;
        private readonly DiceViewModel _diceViewModel;
        private ListCollectionView _groupedElementsView;
        private readonly IDialogService _dialogService;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        /// <summary>
        /// Erzeugt die gruppierte Liste der Elemente der Ideen Kategorie und setzt das EditCommand und DeleteCommand.
        /// </summary>
        /// <param name="dice">Würfel zu dem die Kategorie gehört, wird benötigt zum löschen der Kategorie</param>
        /// <param name="category">Kategorie für die das CategoryViewModel erstellt werden soll</param>
        /// <param name="diceDataService">Wird zur Weitergabe an das ValueListViewModel benötigt</param>
        /// <param name="dialogService">Wird zum Erzeugen eines Dialogs benötigt</param>
        public CategoryViewModel(DiceViewModel dice, Category category, IDiceDataService diceDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            DeleteCommand = new DelegateCommand<object>(DeleteExecute);
            _diceViewModel = dice;
            Category = category;
            var self = this;
            _elementListViewModel ??= new ElementListViewModel(dice, self, diceDataService, _dialogService);
            EditCommand = new DelegateCommand(EditExecute);
            ActivateCommand = new DelegateCommand(ActivateExecute);
            AddCommand = new DelegateCommand(AddExecute);
            CreateGroupedView();
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
        public DelegateCommand ActivateCommand { get; set; }
        /// <summary>
        /// Zum Aktivieren der Kategorie für das Flippen der Karte
        /// </summary>
        public void ActivateExecute()
        {
            Category.Active = !Category.Active;
        }
        /// <summary>
        /// Zum editieren von Kategorien
        /// </summary>
        public void EditExecute()
        {
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;
        }
        /// <summary>
        /// Zum Hinzufügen eines neuen Elements
        /// </summary>
        public async void AddExecute()
        {
            await AddElementAsync();
        }
        /// <summary>
        /// Gruppierte Liste der Elemente
        /// </summary>
        public ListCollectionView GroupedElementsView
        {
            get => _groupedElementsView;
            set => SetProperty(ref _groupedElementsView, value);
        }
        /// <summary>
        /// Das ausgewählte Element
        /// </summary>
        public ElementViewModel SelectedElement
        {
            get
            {
                if (GroupedElementsView != null) return GroupedElementsView.CurrentItem as ElementViewModel;
                else return null;
            }
            set => GroupedElementsView.MoveCurrentTo(value);
        }
        /// <summary>
        /// Aktualisiert die Liste wenn der Name geändert wurde.
        /// </summary>
        /// <param name="propertyName">Name der geänderten Property</param>
        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Category.Name))
            {
                //GroupedElementsView.Refresh();
            }
        }
        /// <summary>
        /// Funktion um das elementListViewModel in einen ListCollectionView umzuwandeln. Dieser wird zur gruppierten Darstellung der Elemente benötigt.
        /// </summary>
        private void CreateGroupedView()
        {
            ObservableCollection<ElementViewModel> elementViewModels = _elementListViewModel.Elements;
            foreach (var elementViewModel in elementViewModels)
            {
                elementViewModel.Element.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "Category.Name";
            GroupedElementsView = new ListCollectionView(elementViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            if (GroupedElementsView.GroupDescriptions != null)
                GroupedElementsView.GroupDescriptions.Add(new PropertyGroupDescription
                {
                    PropertyName = propertyName,
                    Converter = new NameToInitialConverter()
                });
            GroupedElementsView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedElement));
        }
        /// <summary>
        /// Die zum CategoryViewModel gehörige Kategorie
        /// </summary>
        public Category Category { get; }

        public ICommand DeleteCommand { get; set; }

        /// <summary>
        /// Zum Löschen von Kategorien. Wird auf den Button geklickt wird zuerst ein Dialog angezeigt
        /// der mit Ja bestätigt werden muss, erst dann wird die Kategorie gelöscht.
        /// </summary>
        private async void DeleteExecute(object obj)
        {
            var selectedCategory = this;
            bool delete = false;
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
            _diceViewModel.SelectedCategory = this;
            await _diceViewModel.DeleteCategoryAsync();
        }
        /// <summary>
        /// Zum Löschen von Elementen aus der Kategorie. Wird über das ElementViewModel verwendet.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteElementAsync()
        {
            await _elementListViewModel.DeleteElementAsync(SelectedElement);
            GroupedElementsView.Refresh();
        }
        /// <summary>
        /// Zum Hinzufügen neuer Elemente
        /// </summary>
        /// <returns></returns>
        public async Task AddElementAsync()
        {
            Debug.WriteLine("Add Category");
            await _elementListViewModel.AddElementAsync();
            GroupedElementsView.Refresh();
        }
    }
}
