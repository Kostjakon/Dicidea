using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;

namespace DicePage.ViewModels
{
    public class ElementListViewModel : NotifyPropertyChanges
    {
        private ObservableCollection<ElementViewModel> _elements;
        private Dice _selectedDice;
        private CategoryViewModel _selectedCategory;
        private readonly IDiceDataService _diceDataService;

        public ElementListViewModel(Dice dice, CategoryViewModel category, IDiceDataService diceDataService)
        {
            _diceDataService = diceDataService;
            _selectedDice = dice;
            _selectedCategory = category;
            Task.Run(LoadElements);
        }

        public ObservableCollection<ElementViewModel> Elements
        {
            get => _elements;
            private set => SetProperty(ref _elements, value);
        }

        public async Task DeleteElementAsync(ElementViewModel element)
        {
            Elements.Remove(element);
            await _diceDataService.DeleteElementAsync(_selectedDice, _selectedCategory.Category, element.Element);
        }

        public async Task<ElementViewModel> AddElementAsync()
        {
            var elementModel = new Element();
            await _diceDataService.AddElementAsync(_selectedDice, _selectedCategory.Category, elementModel);

            var newElement = new ElementViewModel(elementModel, _selectedCategory, _selectedDice, _diceDataService);
            // DICEVIEWMODEL!
            Elements.Add(newElement);
            return newElement;
        }

        private void LoadElements()
        {
            Elements = new ObservableCollection<ElementViewModel>();
            List<Element> elements = _selectedCategory.Category.Elements;
            //Debug.WriteLine(categories != null);
            if (elements != null) elements.ToList().ForEach(e => Elements.Add(new ElementViewModel(e, _selectedCategory, _selectedDice, _diceDataService)));
        }
    }
}
