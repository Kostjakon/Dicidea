using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace DicePage.ViewModels
{
    /// <summary>
    /// Kapselt den Würfel, fügt UI-spezifische Eigenschaften hinzu (Editieren von Titel und Beschreibung, und hinzufügen von Kategorien)
    /// und erstellt eine ListCollectionView der Kategorien des Würfels.
    /// </summary>
    public class DiceViewModel : NotifyPropertyChanges
    {
        private readonly CategoryListViewModel _categoryListViewModel;
        private ListCollectionView _groupedCategoriesView;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        private ElementViewModel _selectedElement;
        /// <summary>
        /// Erzeugt die gruppierte Liste der Kategorien des Würfels und setzt das EditCommand und das AddCommand.
        /// </summary>
        /// <param name="dice">Würfel für die das DiceViewModel erstellt werden soll</param>
        /// <param name="diceDataService">Wird zur Weitergabe an das CategoryListViewModel benötigt</param>
        /// <param name="dialogService">Wird zur Weitergabe an das CategoryListViewModel benötigt</param>
        public DiceViewModel(Dice dice, IDiceDataService diceDataService, IDialogService dialogService)
        {
            Dice = dice;
            DiceViewModel self = this;
            if (_categoryListViewModel == null)
            {
                _categoryListViewModel = new CategoryListViewModel(self, diceDataService, dialogService);
            }
            CreateGroupedView();
            AddCommand = new DelegateCommand(AddExecute);
            EditCommand = new DelegateCommand(EditExecute);
        }
        /// <summary>
        /// Ausgewähltes Element für das flippen der Karte
        /// </summary>
        public ElementViewModel SelectedElement
        {
            get => _selectedElement;
            set => SetProperty(ref _selectedElement, value);
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
        /// <summary>
        /// Der gruppierte <see cref="ListCollectionView" />, nach dem Anfangsbuchstaben des Würfelnamens gruppiert.
        /// </summary>
        public ListCollectionView GroupedCategoriesView
        {
            get => _groupedCategoriesView;
            set => SetProperty(ref _groupedCategoriesView, value);
        }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        /// <summary>
        /// Zum hinzufügen einer Kategorie
        /// </summary>
        private async void AddExecute()
        {
            await _categoryListViewModel.AddCategoryAsync();
        }
        /// <summary>
        /// Zum Aktivieren und deaktivieren der Editierfunktion
        /// </summary>
        public void EditExecute()
        {
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

        }
        /// <summary>
        /// Ausgewählte Kategorie
        /// </summary>
        public CategoryViewModel SelectedCategory
        {
            get
            {
                if (GroupedCategoriesView != null) return GroupedCategoriesView.CurrentItem as CategoryViewModel;
                else return null;
            }
            set => GroupedCategoriesView.MoveCurrentTo(value);
        }
        /// <summary>
        /// Funktion um das CategoryListViewModel in einen ListCollectionView umzuwandeln. Dieser wird zur gruppierten Darstellung der Kategorien benötigt.
        /// </summary>
        private void CreateGroupedView()
        {
            ObservableCollection<CategoryViewModel> categoryViewModels = _categoryListViewModel.Categories;

            var propertyName = "Category.Name";
            GroupedCategoriesView = new ListCollectionView(categoryViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            
            GroupedCategoriesView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedCategory));
        }
        /// <summary>
        /// Zum Hinzufügen einer Kategorie
        /// </summary>
        /// <returns></returns>
        public async Task AddCategoryAsync()
        {
            await _categoryListViewModel.AddCategoryAsync();
        }
        /// <summary>
        /// Zum löschen einer Kategorie
        /// </summary>
        /// <returns></returns>
        public async Task DeleteCategoryAsync()
        {
            await _categoryListViewModel.DeleteCategoryAsync(SelectedCategory);
            GroupedCategoriesView.Refresh();
        }
        /// <summary>
        /// Der zum DiceViewModel gehörende Würfel
        /// </summary>
        public Dice Dice { get; }

    }
}
