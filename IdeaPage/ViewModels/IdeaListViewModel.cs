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
    /// ViewModel für eine Liste von Ideen. Die Ideen werden aus dem im Konstruktor übergebenen
    /// <see cref="IdeaViewModel" /> gelesen und in eine ObservableCollection umgewandelt.
    /// </summary>
    public class IdeaListViewModel : NotifyPropertyChanges
    {
        private readonly IIdeaDataService _ideaDataService;
        private ObservableCollection<IdeaViewModel> _allIdeas;
        private List<Idea> _ideas;
        private readonly IDialogService _dialogService;
        /// <summary>
        /// Der Ideen DataService und der DialogService werden hier gesetzt, die Ideen aus dem
        /// Ideen DataService geladen und in eine ObservableCollection umgewandelt.
        /// Dieser Konstruktor wird bei dem Laden aller Ideen über den DataService verwendet.
        /// </summary>
        /// <param name="ideaDataService">Benötigt um die Ideen aus der Datenquelle zu laden</param>
        /// <param name="dialogService">Benötigt zur Weitergabe an das IdeaValueViewModel</param>
        public IdeaListViewModel(IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ideaDataService = ideaDataService;
            LoadIdeasAsync().Wait();
        }
        /// <summary>
        /// Die Ideen Liste und der DialogService werden hier gesetzt.
        /// Dieser Konstruktor wird bei dem rollen neuer Ideen verwendet.
        /// </summary>
        /// <param name="ideas">Liste der gerollten Ideen</param>
        /// <param name="dialogService">Benötigt zur Weitergabe an das IdeaValueViewModel</param>
        public IdeaListViewModel(List<Idea> ideas, IDialogService dialogService)//, IIdeaDataService ideaDataService
        {
            _dialogService = dialogService;
            //_ideaDataService = ideaDataService;
            AllIdeas = new ObservableCollection<IdeaViewModel>();
            Ideas = ideas;
            Ideas.ToList().ForEach(d => AllIdeas.Add(new IdeaViewModel(d, _ideaDataService, _dialogService)));
        }

        /// <summary>
        /// Observable Liste der IdeaViewModels für alle Ideen die über den DataService geladen wurden
        /// </summary>
        public ObservableCollection<IdeaViewModel> AllIdeas
        {
            get => _allIdeas;
            private set => SetProperty(ref _allIdeas, value);
        }
        /// <summary>
        /// Liste aller Ideen des temporären IdeaListViewModels beim rollen neuer Ideen
        /// </summary>
        public List<Idea> Ideas
        {
            get => _ideas;
            set => SetProperty(ref _ideas, value);
        }
        /// <summary>
        /// Zum Löschen einer Idee
        /// </summary>
        /// <param name="idea">Idee die gelöscht werden soll</param>
        /// <returns></returns>
        public async Task DeleteIdeaAsync(IdeaViewModel idea)
        {
            AllIdeas.Remove(idea);
            Ideas.Remove(idea.Idea);
            await _ideaDataService.DeleteIdeaAsync(idea.Idea);
        }
        /// <summary>
        /// Zum hinzufügen einer Liste von neu gewürfelten Ideen. Es werden nur die Ideen hinzugefügt,
        /// bei denen der Bool "Save" auf True gesetzt ist.
        /// </summary>
        /// <param name="ideas">Liste der gerollten Ideen die hinzugefügt werden sollen</param>
        /// <returns></returns>
        public async Task AddIdeasAsync(List<Idea> ideas)
        {
            foreach (Idea idea in ideas)
            {
                if (idea.Save)
                {
                    _allIdeas.Add(new IdeaViewModel(idea, _ideaDataService, _dialogService));
                }
            }
            await _ideaDataService.AddIdeasAsync(ideas);
        }
        /// <summary>
        /// Zum speichern der Ideen
        /// </summary>
        /// <returns></returns>
        public async Task SaveIdeasAsync()
        {
            await _ideaDataService.SaveIdeasAsync();
        }
        /// <summary>
        /// Zum Laden aller Ideen
        /// </summary>
        /// <returns></returns>
        private async Task LoadIdeasAsync()
        {
            AllIdeas = new ObservableCollection<IdeaViewModel>();
            Ideas = await _ideaDataService.GetAllIdeasAsync();
            AllIdeas.Clear();
            Ideas.ToList().ForEach(i => AllIdeas.Add(new IdeaViewModel(i, _ideaDataService, _dialogService)));
        }
    }
}
