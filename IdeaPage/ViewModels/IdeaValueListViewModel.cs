using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    /// <summary>
    /// ViewModel für eine Liste von Idee Werten. Die Idee Werte werden aus dem im Konstruktor übergebenen
    /// <see cref="IdeaElementViewModel" /> gelesen und in eine ObservableCollection umgewandelt.
    /// </summary>
    public class IdeaValueListViewModel : BindableBase
    {
        private ObservableCollection<IdeaValueViewModel> _ideaValues;
        private readonly IdeaElementViewModel _selectedIdeaElement;
        private readonly IIdeaDataService _ideaDataService;
        private readonly IDialogService _dialogService;
        /// <summary>
        /// Das zugehörige Ideen Element, der Ideen DataService und der DialogService werden hier gesetzt
        /// und die Werte des Ideen Elements geladen und in eine ObservableCollection umgewandelt.
        /// </summary>
        /// <param name="ideaElement">Benötigt für die Werte des Elements und um Werte daraus zu löschen.</param>
        /// <param name="ideaDataService">Benötigt um Werte auch aus der Datenquelle zu löschen</param>
        /// <param name="dialogService">Benötigt zur Weitergabe an das IdeaValueViewModel</param>
        public IdeaValueListViewModel(IdeaElementViewModel ideaElement, IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ideaDataService = ideaDataService;
            _selectedIdeaElement = ideaElement;
            LoadValues();
        }
        /// <summary>
        /// Liste der IdeaValueViewModels für alle Werte des im Konstruktor übergebenen Elements
        /// </summary>
        public ObservableCollection<IdeaValueViewModel> IdeaValues
        {
            get => _ideaValues;
            private set => SetProperty(ref _ideaValues, value);
        }
        /// <summary>
        /// Zum Löschen eines Wertes aus dem übergebenen Ideen Element
        /// </summary>
        /// <param name="ideaValue">Wert der gelöscht werden soll</param>
        /// <returns></returns>
        public async Task DeleteIdeaValueAsync(IdeaValueViewModel ideaValue)
        {
            IdeaValues.Remove(ideaValue);
            _selectedIdeaElement.IdeaElement.IdeaValues.Remove(ideaValue.IdeaValue);
            if (_ideaDataService != null)
            {
                await _ideaDataService.DeleteIdeaValueAsync(_selectedIdeaElement.IdeaElement, ideaValue.IdeaValue);
            }
        }
        /// <summary>
        /// Zum Laden der Werte aus dem übergebenen Ideen Element und umwandeln der Werte in eine ObservableCollection von IdeaValueViewModels
        /// </summary>
        private void LoadValues()
        {
            IdeaValues = new ObservableCollection<IdeaValueViewModel>();
            List<IdeaValue> ideaValues = _selectedIdeaElement.IdeaElement.IdeaValues;
            if (ideaValues != null) ideaValues.ToList().ForEach(v => IdeaValues.Add(new IdeaValueViewModel(v, _selectedIdeaElement, _dialogService)));
        }
    }
}
