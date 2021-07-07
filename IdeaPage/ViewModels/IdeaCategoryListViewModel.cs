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
    /// <see cref="IdeaViewModel" /> gelesen und in eine ObservableCollection umgewandelt.
    /// </summary>
    public class IdeaCategoryListViewModel : NotifyPropertyChanges
    {
        private ObservableCollection<IdeaCategoryViewModel> _ideaCategories;
        private readonly IdeaViewModel _selectedIdea;
        private readonly IIdeaDataService _ideaDataService;
        private readonly IDialogService _dialogService;
        /// <summary>
        /// Die zugehörige Idee, der Ideen DataService und der DialogService werden hier gesetzt
        /// und die Kategorien der Idee geladen und in eine ObservableCollection umgewandelt.
        /// </summary>
        /// <param name="idea">Benötigt für die Kategorien der Idee und um Kategorien daraus zu löschen.</param>
        /// <param name="ideaDataService">Benötigt um Kategorien auch aus der Datenquelle zu löschen</param>
        /// <param name="dialogService">Benötigt zur Weitergabe an das IdeaCategoryViewModel</param>
        public IdeaCategoryListViewModel(IdeaViewModel idea, IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ideaDataService = ideaDataService;
            _selectedIdea = idea;
            LoadIdeaCategories();
        }
        /// <summary>
        /// Liste der IdeaCategoryViewModels für alle Kategorien der im Konstruktor übergebenen Idee
        /// </summary>
        public ObservableCollection<IdeaCategoryViewModel> IdeaCategories
        {
            get => _ideaCategories;
            private set => SetProperty(ref _ideaCategories, value);
        }
        /// <summary>
        /// Zum Löschen einer Kategorie aus der übergebenen Idee
        /// </summary>
        /// <param name="ideaCategory">Die Kategorie die gelöscht werden soll</param>
        /// <returns></returns>
        public async Task DeleteIdeaCategoryAsync(IdeaCategoryViewModel ideaCategory)
        {
            IdeaCategories.Remove(ideaCategory);
            _selectedIdea.Idea.IdeaCategories.Remove(ideaCategory.IdeaCategory);
            if (_ideaDataService != null)
            {
                await _ideaDataService.DeleteIdeaCategoryAsync(_selectedIdea.Idea, ideaCategory.IdeaCategory);
            }
        }
        /// <summary>
        /// Zum Laden der Kategorien aus der übergebenen Idee und umwandeln der Kategorien in eine ObservableCollection von IdeaCategoryViewModels
        /// </summary>
        private void LoadIdeaCategories()
        {
            IdeaCategories = new ObservableCollection<IdeaCategoryViewModel>();
            List<IdeaCategory> ideaCategories = _selectedIdea.Idea.IdeaCategories;
            if (ideaCategories != null) ideaCategories.ToList().ForEach(c => IdeaCategories.Add(new IdeaCategoryViewModel(_selectedIdea, c, _ideaDataService, _dialogService)));
        }
    }
}
