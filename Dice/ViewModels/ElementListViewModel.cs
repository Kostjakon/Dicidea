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
    /// <see cref="CategoryViewModel" /> gelesen und in eine ObservableCollection umgewandelt.
    /// </summary>
    public class ElementListViewModel : NotifyPropertyChanges
    {
        private ObservableCollection<ElementViewModel> _elements;
        private DiceViewModel _selectedDice;
        private CategoryViewModel _selectedCategory;
        private readonly IDiceDataService _diceDataService;
        private readonly IDialogService _dialogService;
        /// <summary>
        /// Die zugehörige Kategorie, der Würfel DataService und der DialogService werden hier gesetzt
        /// und die Elemente der Kategorie geladen und in eine ObservableCollection umgewandelt.
        /// </summary>
        /// <param name="category">Benötigt für die Elemente der Kategorie und um Werte daraus zu löschen.</param>
        /// <param name="diceViewModel">Benötigt zur Weitergabe an das ElementViewModel</param>
        /// <param name="diceDataService">Benötigt um Elemente auch aus der Datenquelle zu löschen</param>
        /// <param name="dialogService">Benötigt zur Weitergabe an das ElementViewModel</param>
        public ElementListViewModel(DiceViewModel diceViewModel, CategoryViewModel category, IDiceDataService diceDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _diceDataService = diceDataService;
            _selectedDice = diceViewModel;
            _selectedCategory = category;
            LoadElements();
        }
        /// <summary>
        /// Liste der ElementViewModels für alle Elemente der im Konstruktor übergebenen Kategorie
        /// </summary>
        public ObservableCollection<ElementViewModel> Elements
        {
            get => _elements;
            private set => SetProperty(ref _elements, value);
        }
        /// <summary>
        /// Zum Löschen eines Elements aus der übergebenen Kategorie
        /// </summary>
        /// <param name="element">Element das gelöscht werden soll</param>
        /// <returns></returns>
        public async Task DeleteElementAsync(ElementViewModel element)
        {
            Elements.Remove(element);
            await _diceDataService.DeleteElementAsync(_selectedCategory.Category, element.Element);
        }
        /// <summary>
        /// Zum Hinzufügen eines Elements zur übergebenen Kategorie
        /// </summary>
        /// <returns></returns>
        public async Task<ElementViewModel> AddElementAsync()
        {
            var elementModel = new Element(true);
            await _diceDataService.AddElementAsync(_selectedCategory.Category, elementModel);

            var newElement = new ElementViewModel(elementModel, _selectedCategory, _selectedDice, _diceDataService, _dialogService);
            Elements.Add(newElement);
            return newElement;
        }
        /// <summary>
        /// Zum Laden der Elemente aus der übergebenen Kategorie und umwandeln der Elemente in eine ObservableCollection von ElementViewModels
        /// </summary>
        private void LoadElements()
        {
            Elements = new ObservableCollection<ElementViewModel>();
            List<Element> elements = _selectedCategory.Category.Elements;
            if (elements != null) elements.ToList().ForEach(e => Elements.Add(new ElementViewModel(e, _selectedCategory, _selectedDice, _diceDataService, _dialogService)));
        }
    }
}
