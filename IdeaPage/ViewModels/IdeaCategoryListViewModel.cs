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

namespace IdeaPage.ViewModels
{
    public class IdeaCategoryListViewModel : NotifyPropertyChanges
    {
        private ObservableCollection<IdeaCategoryViewModel> _ideaCategories;
        private IdeaViewModel _selectedIdea;
        private readonly IIdeaDataService _ideaDataService;

        public IdeaCategoryListViewModel(IdeaViewModel idea, IIdeaDataService ideaDataService)
        {
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
            await _ideaDataService.DeleteIdeaCategoryAsync(_selectedIdea.Idea, ideaCategory.Idea);
        }

        public async Task<IdeaCategoryViewModel> AddIdeaCategoryAsync()
        {
            var ideaCategoryModel = new IdeaCategory(true);
            await _ideaDataService.AddIdeaCategoryAsync(_selectedIdea.Idea, ideaCategoryModel);
            
            var newIdeaCategory = new IdeaCategoryViewModel(_selectedIdea, ideaCategoryModel, _ideaDataService);
            // DICEVIEWMODEL!
            IdeaCategories.Add(newIdeaCategory);

            return newIdeaCategory;
        }

        private void LoadIdeaCategories()
        {
            IdeaCategories = new ObservableCollection<IdeaCategoryViewModel>();
            List<IdeaCategory> ideaCategories = _selectedIdea.Idea.IdeaCategories;
            //Debug.WriteLine(categories != null);
            if (ideaCategories != null) ideaCategories.ToList().ForEach(c => IdeaCategories.Add(new IdeaCategoryViewModel(_selectedIdea, c, _ideaDataService)));
        }
    }
}
