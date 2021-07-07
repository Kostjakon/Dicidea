using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    /// <summary>
    /// ViewModel für eine Liste von Idee Elementen. Die Idee Elemente werden aus dem im Konstruktor übergebenen
    /// <see cref="IdeaCategoryViewModel" /> gelesen und in eine ObservableCollection umgewandelt.
    /// </summary>
    public class IdeaElementListViewModel : NotifyPropertyChanges
    {
        private ObservableCollection<IdeaElementViewModel> _ideaElements;
        private IdeaCategoryViewModel _selectedIdeaCategory;
        private readonly IIdeaDataService _ideaDataService;
        private readonly IDialogService _dialogService;
        /// <summary>
        /// Die zugehörige Ideen Kategorie, der Ideen DataService und der DialogService werden hier gesetzt
        /// und die Elemente der Ideen Kategorie geladen und in eine ObservableCollection umgewandelt.
        /// </summary>
        /// <param name="ideaCategory">Benötigt für die Elemente der Kategorie und um Werte daraus zu löschen.</param>
        /// <param name="ideaDataService">Benötigt um Elemente auch aus der Datenquelle zu löschen</param>
        /// <param name="dialogService">Benötigt zur Weitergabe an das IdeaElementViewModel</param>
        public IdeaElementListViewModel(IdeaCategoryViewModel ideaCategory, IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ideaDataService = ideaDataService;
            _selectedIdeaCategory = ideaCategory;
            LoadIdeaElements();
        }
        /// <summary>
        /// Liste der IdeaElementViewModels für alle Elemente der im Konstruktor übergebenen Kategorie
        /// </summary>
        public ObservableCollection<IdeaElementViewModel> IdeaElements
        {
            get => _ideaElements;
            private set => SetProperty(ref _ideaElements, value);
        }
        /// <summary>
        /// Zum Löschen eines Elements aus der übergebenen Ideen Kategorie
        /// </summary>
        /// <param name="ideaElement">Ideen Element das gelöscht werden soll</param>
        /// <returns></returns>
        public async Task DeleteIdeaElementAsync(IdeaElementViewModel ideaElement)
        {
            IdeaElements.Remove(ideaElement);
            _selectedIdeaCategory.IdeaCategory.IdeaElements.Remove(ideaElement.IdeaElement);
            if (_ideaDataService != null)
            {
                await _ideaDataService.DeleteIdeaElementAsync(_selectedIdeaCategory.IdeaCategory, ideaElement.IdeaElement);
            }
        }
        /// <summary>
        /// Zum Laden der Elemente aus der übergebenen Ideen Kategorie und umwandeln der Elemente in eine ObservableCollection von IdeaElementViewModels
        /// </summary>
        private void LoadIdeaElements()
        {
            IdeaElements = new ObservableCollection<IdeaElementViewModel>();
            List<IdeaElement> ideaElements = _selectedIdeaCategory.IdeaCategory.IdeaElements;
            ideaElements?.ToList().ForEach(e => IdeaElements.Add(new IdeaElementViewModel(e, _selectedIdeaCategory, _ideaDataService, _dialogService)));
        }

    }
}
