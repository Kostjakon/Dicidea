using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    public class IdeaListViewModel : NotifyPropertyChanges
    {
        private readonly IIdeaDataService _ideaDataService;
        private ObservableCollection<IdeaViewModel> _allIdeas;
        private List<Idea> _ideas;
        private readonly IDialogService _dialogService;

        public IdeaListViewModel(IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ideaDataService = ideaDataService;
            LoadIdeasAsync().Wait();
        }
        public IdeaListViewModel(List<Idea> ideas, IDialogService dialogService)//, IIdeaDataService ideaDataService
        {
            _dialogService = dialogService;
            //_ideaDataService = ideaDataService;
            AllIdeas = new ObservableCollection<IdeaViewModel>();
            Ideas = ideas;
            Ideas.ToList().ForEach(d => AllIdeas.Add(new IdeaViewModel(d, _ideaDataService, _dialogService)));
        }

        public ObservableCollection<IdeaViewModel> AllIdeas
        {
            get => _allIdeas;
            private set => SetProperty(ref _allIdeas, value);
        }

        public List<Idea> Ideas
        {
            get => _ideas;
            set => SetProperty(ref _ideas, value);
        }

        public async Task DeleteIdeaAsync(IdeaViewModel idea)
        {
            AllIdeas.Remove(idea);
            Ideas.Remove(idea.Idea);
            await _ideaDataService.DeleteIdeaAsync(idea.Idea);
        }
        
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

        public async Task SaveIdeasAsync()
        {
            await _ideaDataService.SaveIdeasAsync();
        }

        private async Task LoadIdeasAsync()
        {
            AllIdeas = new ObservableCollection<IdeaViewModel>();
            Ideas = await _ideaDataService.GetAllIdeasAsync();
            AllIdeas.Clear();
            Ideas.ToList().ForEach(i => AllIdeas.Add(new IdeaViewModel(i, _ideaDataService, _dialogService)));
        }
    }
}
