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
    public class IdeaElementListViewModel : NotifyPropertyChanges
    {
        private ObservableCollection<IdeaElementViewModel> _ideaElements;
        private Idea _selectedIdea;
        private IdeaCategoryViewModel _selectedIdeaCategory;
        private readonly IIdeaDataService _ideaDataService;

        public IdeaElementListViewModel(Idea idea, IdeaCategoryViewModel ideaCategory, IIdeaDataService ideaDataService)
        {
            _ideaDataService = ideaDataService;
            _selectedIdea = idea;
            _selectedIdeaCategory = ideaCategory;
            LoadIdeaElements();
        }

        public ObservableCollection<IdeaElementViewModel> IdeaElements
        {
            get => _ideaElements;
            private set => SetProperty(ref _ideaElements, value);
        }

        public async Task DeleteIdeaElementAsync(IdeaElementViewModel ideaElement)
        {
            IdeaElements.Remove(ideaElement);
            //await _ideaDataService.DeleteIdeaElementAsync(_selectedIdea, _selectedIdeaCategory.IdeaCategory, ideaElement.IdeaElement);
        }

        public async Task<IdeaElementViewModel> AddIdeaElementAsync()
        {
            // TODO: IdeaElement Konstruktor füllen
            var ideaElementModel = new IdeaElement();
            await _ideaDataService.AddIdeaElementAsync(_selectedIdea, _selectedIdeaCategory.IdeaCategory, ideaElementModel);

            var newIdeaElement = new IdeaElementViewModel(ideaElementModel, _selectedIdeaCategory, _selectedIdea, _ideaDataService);
            // DICEVIEWMODEL!
            IdeaElements.Add(newIdeaElement);
            return newIdeaElement;
        }

        private void LoadIdeaElements()
        {
            IdeaElements = new ObservableCollection<IdeaElementViewModel>();
            List<IdeaElement> ideaElements = _selectedIdeaCategory.IdeaCategory.IdeaElements;
            //Debug.WriteLine(categories != null);
            if (ideaElements != null) ideaElements.ToList().ForEach(e => IdeaElements.Add(new IdeaElementViewModel(e, _selectedIdeaCategory, _selectedIdea, _ideaDataService)));
        }

    }
}
