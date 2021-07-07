using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Services.Dialogs;

namespace DicePage.ViewModels
{
    /// <summary>
    /// ViewModel für eine Liste von Elementen. Die Elemente werden aus dem im Konstruktor übergebenen
    /// <see cref="DiceViewModel" /> gelesen und in eine ObservableCollection umgewandelt.
    /// </summary>
    public class CategoryListViewModel : NotifyPropertyChanges
    {
        private ObservableCollection<CategoryViewModel> _categories;
        private DiceViewModel _selectedDice;
        private readonly IDiceDataService _diceDataService;
        private readonly IDialogService _dialogService;
        /// <summary>
        /// Der zugehörige Würfel, der Würfel DataService und der DialogService werden hier gesetzt
        /// und die Kategorien des Würfels geladen und in eine ObservableCollection umgewandelt.
        /// </summary>
        /// <param name="dice">Benötigt für die Kategorien des Würfels und um Kategorien daraus zu löschen.</param>
        /// <param name="diceDataService">Benötigt um Kategorien auch aus der Datenquelle zu löschen</param>
        /// <param name="dialogService">Benötigt zur Weitergabe an das IdeaCategoryViewModel</param>
        public CategoryListViewModel(DiceViewModel dice, IDiceDataService diceDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _diceDataService = diceDataService;
            _selectedDice = dice;
            LoadCategories();
        }
        /// <summary>
        /// Liste der CategoryViewModels für alle Kategorien des im Konstruktor übergebenen Würfels
        /// </summary>
        public ObservableCollection<CategoryViewModel> Categories
        {
            get => _categories;
            private set => SetProperty(ref _categories, value);
        }
        /// <summary>
        /// Zum Löschen einer Kategorie aus dem übergebenen Würfel
        /// </summary>
        /// <param name="category">Die Kategorie die gelöscht werden soll</param>
        /// <returns></returns>
        public async Task DeleteCategoryAsync(CategoryViewModel category)
        {
            Categories.Remove(category);
            await _diceDataService.DeleteCategoryAsync(_selectedDice.Dice, category.Category);
        }
        /// <summary>
        /// Zum hinzufügen einer neuen Kategorie
        /// </summary>
        /// <returns></returns>
        public async Task<CategoryViewModel> AddCategoryAsync()
        {
            var categoryModel = new Category(true);
            await _diceDataService.AddCategoryAsync(_selectedDice.Dice, categoryModel);
            var newCategory = new CategoryViewModel(_selectedDice, categoryModel, _diceDataService, _dialogService);
            Categories.Add(newCategory);
            return newCategory;
        }
        /// <summary>
        /// Zum Laden der Kategorien aus dem übergebenen Würfel und umwandeln der Kategorien in eine ObservableCollection von CategoryViewModels
        /// </summary>
        private void LoadCategories()
        {
            Categories = new ObservableCollection<CategoryViewModel>();
            List<Category> categories = _selectedDice.Dice.Categories;
            if(categories!= null) categories.ToList().ForEach(c => Categories.Add(new CategoryViewModel(_selectedDice, c, _diceDataService, _dialogService)));
        }
    }
}
