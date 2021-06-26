using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Mvvm;

namespace IdeaPage.ViewModels
{
    public class IdeaListViewModel : NotifyPropertyChanges
    {
        private readonly IIdeaDataService _ideaDataService;
        private ObservableCollection<IdeaViewModel> _allIdeas;

        public IdeaListViewModel(IIdeaDataService ideaDataService)
        {
            _ideaDataService = ideaDataService;
            LoadIdeasAsync();
        }

        public ObservableCollection<IdeaViewModel> AllIdeas
        {
            get => _allIdeas;
            private set => SetProperty(ref _allIdeas, value);
        }

        public async Task DeleteIdeaAsync(IdeaViewModel idea)
        {
            AllIdeas.Remove(idea);
            await _ideaDataService.DeleteIdeaAsync(idea.Idea);
        }

        public async Task<IdeaViewModel> AddIdeaAsync()
        {
            // TODO: Idea Konstruktor füllen
            var ideaModel = new Idea();
            await _ideaDataService.AddIdeaAsync(ideaModel);

            var newIdea = new IdeaViewModel(ideaModel, _ideaDataService);
            AllIdeas.Add(newIdea);
            return newIdea;
        }

        public async Task SaveIdeasAsync()
        {
            await _ideaDataService.SaveIdeasAsync();
        }

        private async Task LoadIdeasAsync()
        {
            AllIdeas = new ObservableCollection<IdeaViewModel>();
            List<Idea> idea = await _ideaDataService.GetAllIdeasAsync();
            idea.ToList().ForEach(d => AllIdeas.Add(new IdeaViewModel(d, _ideaDataService)));
        }
    }
}
