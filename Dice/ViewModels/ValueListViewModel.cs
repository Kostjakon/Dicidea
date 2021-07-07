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
    /// ViewModel für eine Liste von Idee Werten. Die Idee Werte werden aus dem im Konstruktor übergebenen
    /// <see cref="ElementViewModel" /> gelesen und in eine ObservableCollection umgewandelt.
    /// </summary>
    public class ValueListViewModel : NotifyPropertyChanges
    {
        private ObservableCollection<ValueViewModel> _values;
        private ElementViewModel _selectedElement;
        private readonly IDiceDataService _diceDataService;
        private readonly IDialogService _dialogService;
        /// <summary>
        /// Das zugehörige Element, der Würfel DataService und der DialogService werden hier gesetzt
        /// und die Werte des Elements geladen und in eine ObservableCollection umgewandelt.
        /// </summary>
        /// <param name="element">Benötigt für die Werte des Elements und um Werte daraus zu löschen.</param>
        /// <param name="diceDataService">Benötigt um Werte auch aus der Datenquelle zu löschen</param>
        /// <param name="dialogService">Benötigt zur Weitergabe an das ValueViewModel</param>
        public ValueListViewModel(ElementViewModel element, IDiceDataService diceDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _diceDataService = diceDataService;
            _selectedElement = element;
            LoadElements();
        }
        /// <summary>
        /// Liste der ValueViewModels für alle Werte des im Konstruktor übergebenen Elements
        /// </summary>
        public ObservableCollection<ValueViewModel> Values
        {
            get => _values;
            private set => SetProperty(ref _values, value);
        }
        /// <summary>
        /// Zum Löschen eines Wertes aus dem übergebenen Element
        /// </summary>
        /// <param name="value">Wert der gelöscht werden soll</param>
        /// <returns></returns>
        public async Task DeleteValueAsync(ValueViewModel value)
        {
            Values.Remove(value);
            await _diceDataService.DeleteValueAsync(_selectedElement.Element, value.Value);
        }
        /// <summary>
        /// Zum Hinzufügen eines Wertes zu dem übergebenen Element
        /// </summary>
        /// <returns></returns>
        public async Task<ValueViewModel> AddValueAsync()
        {
            var valueModel = new Value(true);
            await _diceDataService.AddValueAsync(_selectedElement.Element, valueModel);
            var newValue = new ValueViewModel(valueModel, _selectedElement, _dialogService);
            Values.Add(newValue);
            return newValue;
        }
        /// <summary>
        /// Zum Laden der Werte aus dem übergebenen Element und umwandeln der Werte in eine ObservableCollection von ValueViewModels
        /// </summary>
        private void LoadElements()
        {
            Values = new ObservableCollection<ValueViewModel>();
            List<Value> values = _selectedElement.Element.Values;
            if (values != null) values.ToList().ForEach(v => Values.Add(new ValueViewModel(v, _selectedElement, _dialogService)));
        }
    }
}
