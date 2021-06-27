using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Mvvm;

namespace IdeaPage.ViewModels
{
    public class IdeaValueListViewModel : BindableBase
    {
        private ObservableCollection<IdeaValueViewModel> _ideaValues;
        private Idea _selectedIdea;
        private IdeaCategory _selectedIdeaCategory;
        private IdeaElementViewModel _selectedIdeaElement;
        private readonly IIdeaDataService _ideaDataService;

        public IdeaValueListViewModel(Idea idea, IdeaCategory ideaCategory, IdeaElementViewModel ideaElement, IIdeaDataService ideaDataService)
        {
            _ideaDataService = ideaDataService;
            _selectedIdea = idea;
            _selectedIdeaCategory = ideaCategory;
            _selectedIdeaElement = ideaElement;
            LoadElements();
        }

        public ObservableCollection<IdeaValueViewModel> IdeaValues
        {
            get => _ideaValues;
            private set => SetProperty(ref _ideaValues, value);
        }

        public async Task DeleteIdeaValueAsync(IdeaValueViewModel ideaValue)
        {
            IdeaValues.Remove(ideaValue);
            //await _ideaDataService.DeleteIdeaValueAsync(_selectedIdea, _selectedIdeaCategory, _selectedIdeaElement.IdeaElement, ideaValue.IdeaValue);
        }

        public async Task<IdeaValueViewModel> AddIdeaValueAsync()
        {
            // TODO: IdeaValue Konstruktor füllen
            var ideaValueModel = new IdeaValue();
            await _ideaDataService.AddIdeaValueAsync(_selectedIdea, _selectedIdeaCategory, _selectedIdeaElement.IdeaElement, ideaValueModel);

            var newIdeaValue = new IdeaValueViewModel(ideaValueModel, _selectedIdeaElement);
            // DICEVIEWMODEL!
            IdeaValues.Add(newIdeaValue);
            return newIdeaValue;
        }

        private void LoadElements()
        {
            IdeaValues = new ObservableCollection<IdeaValueViewModel>();
            List<IdeaValue> ideaValues = _selectedIdeaElement.IdeaElement.IdeaValues;
            //Debug.WriteLine(categories != null);
            if (ideaValues != null) ideaValues.ToList().ForEach(v => IdeaValues.Add(new IdeaValueViewModel(v, _selectedIdeaElement)));
        }
    }
}
