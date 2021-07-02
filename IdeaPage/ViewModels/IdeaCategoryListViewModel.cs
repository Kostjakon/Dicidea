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
    public class IdeaCategoryListViewModel : NotifyPropertyChanges
    {
        private ObservableCollection<IdeaCategoryViewModel> _ideaCategories;
        private IdeaViewModel _selectedIdea;
        private readonly IIdeaDataService _ideaDataService;
        private readonly IDialogService _dialogService;

        public IdeaCategoryListViewModel(IdeaViewModel idea, IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ideaDataService = ideaDataService;
            _selectedIdea = idea;
            LoadIdeaCategories();
        }

        public ObservableCollection<IdeaCategoryViewModel> IdeaCategories
        {
            get => _ideaCategories;
            private set => SetProperty(ref _ideaCategories, value);
        }

        public async Task DeleteIdeaCategoryAsync(IdeaCategoryViewModel ideaCategory)
        {
            IdeaCategories.Remove(ideaCategory);
            _selectedIdea.Idea.IdeaCategories.Remove(ideaCategory.IdeaCategory);
            //await _ideaDataService.DeleteIdeaCategoryAsync(_selectedIdea.Idea, ideaCategory.IdeaCategory);
        }

        private void LoadIdeaCategories()
        {
            IdeaCategories = new ObservableCollection<IdeaCategoryViewModel>();
            List<IdeaCategory> ideaCategories = _selectedIdea.Idea.IdeaCategories;
            //Debug.WriteLine(categories != null);
            if (ideaCategories != null) ideaCategories.ToList().ForEach(c => IdeaCategories.Add(new IdeaCategoryViewModel(_selectedIdea, c, _ideaDataService, _dialogService)));
        }
    }
}
