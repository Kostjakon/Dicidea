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
    public class ValueListViewModel : NotifyPropertyChanges
    {
        private ObservableCollection<ValueViewModel> _values;
        private Dice _selectedDice;
        private Category _selectedCategory;
        private ElementViewModel _selectedElement;
        private readonly IDiceDataService _diceDataService;

        public ValueListViewModel(Dice dice, Category category, ElementViewModel element, IDiceDataService diceDataService)
        {
            _diceDataService = diceDataService;
            _selectedDice = dice;
            _selectedCategory = category;
            _selectedElement = element;
            LoadElements();
        }

        public ObservableCollection<ValueViewModel> Values
        {
            get => _values;
            private set => SetProperty(ref _values, value);
        }

        public async Task DeleteValueAsync(ValueViewModel value)
        {
            Values.Remove(value);
            await _diceDataService.DeleteValueAsync(_selectedDice, _selectedCategory, _selectedElement.Element, value.Value);
        }

        public async Task<ValueViewModel> AddValueAsync()
        {
            var valueModel = new Value();
            await _diceDataService.AddValueAsync(_selectedDice, _selectedCategory, _selectedElement.Element, valueModel);

            var newValue = new ValueViewModel(valueModel, _selectedElement);
            // DICEVIEWMODEL!
            Values.Add(newValue);
            return newValue;
        }

        private void LoadElements()
        {
            Values = new ObservableCollection<ValueViewModel>();
            List<Value> values = _selectedElement.Element.Values;
            //Debug.WriteLine(categories != null);
            if (values != null) values.ToList().ForEach(v => Values.Add(new ValueViewModel(v, _selectedElement)));
        }
    }
}
