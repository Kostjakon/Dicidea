using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace IdeaPage.ViewModels
{
    public class IdeaElementViewModel : NotifyPropertyChanges
    {
        private readonly IdeaCategoryViewModel _ideaCategoryViewModel;
        private IdeaValueListViewModel _ideaValueListViewModel;
        private ListCollectionView _groupedIdeaValuesView;
        private IIdeaDataService _ideaDataService;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        public IdeaElementViewModel(IdeaElement ideaElement, IdeaCategoryViewModel ideaCategoryViewModel, Idea idea, IIdeaDataService ideaDataService)
        {
            _ideaCategoryViewModel = ideaCategoryViewModel;
            FlipCommand = new DelegateCommand<object>(Flip, CanFlip);
            IdeaElement = ideaElement;
            EditCommand = new DelegateCommand(EditExecute);
            AddCommand = new DelegateCommand(AddExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            IdeaElementViewModel self = this;
            if (_ideaValueListViewModel == null)
            {
                _ideaValueListViewModel = new IdeaValueListViewModel(idea, _ideaCategoryViewModel.IdeaCategory, self, ideaDataService);
            }
            CreateGroupedView();
        }

        public ListCollectionView GroupedIdeaValuesView
        {
            get => _groupedIdeaValuesView;
            set => SetProperty(ref _groupedIdeaValuesView, value);
        }

        public IdeaValueViewModel SelectedIdeaValue
        {
            get
            {
                if (GroupedIdeaValuesView != null) return GroupedIdeaValuesView.CurrentItem as IdeaValueViewModel;
                return null;
            }
            set => GroupedIdeaValuesView.MoveCurrentTo(value);
        }
        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(IdeaElement.Name))
            {
                //GroupedElementsView.Refresh();
            }
        }

        private void CreateGroupedView()
        {
            ObservableCollection<IdeaValueViewModel> ideaValueViewModels = _ideaValueListViewModel.IdeaValues;
            foreach (var ideaValueViewModel in ideaValueViewModels)
            {
                ideaValueViewModel.IdeaValue.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "IdeaElement.Name";
            GroupedIdeaValuesView = new ListCollectionView(ideaValueViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            if (GroupedIdeaValuesView.GroupDescriptions != null)
                GroupedIdeaValuesView.GroupDescriptions.Add(new PropertyGroupDescription
                {
                    PropertyName = propertyName,
                    Converter = new NameToInitialConverter()
                });
            GroupedIdeaValuesView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedIdeaValue));
        }

        public bool IsEditEnabled
        {
            get => _isEditEnabled;
            set => SetProperty(ref _isEditEnabled, value);
        }
        public bool IsEditDisabled
        {
            get => _isEditDisabled;
            set => SetProperty(ref _isEditDisabled, value);
        }

        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }

        public void EditExecute()
        {
            Debug.WriteLine("Edit Idea Element");
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;
        }
        public async void AddExecute()
        {
            await AddIdeaValueAsync();
        }
        public async void DeleteExecute()
        {
            _ideaCategoryViewModel.SelectedIdeaElement = this;
            await _ideaCategoryViewModel.DeleteIdeaElementAsync();
        }

        public bool IsSelected
        {
            get => _ideaCategoryViewModel.SelectedIdeaElement == this;
        }

        public IdeaCategory IdeaCategory
        {
            get => _ideaCategoryViewModel.IdeaCategory;
        }

        public IdeaElement IdeaElement { get; }
        public ICommand FlipCommand { get; set; }

        private bool CanFlip(object obj)
        {
            return true;
        }

        public async Task AddIdeaValueAsync()
        {
            Debug.WriteLine("Add Category");
            await _ideaValueListViewModel.AddIdeaValueAsync();
            //GroupedCategoriesView.Refresh();
        }
        public async Task DeleteIdeaValueAsync()
        {
            Debug.WriteLine("Delete Value");
            await _ideaValueListViewModel.DeleteIdeaValueAsync(SelectedIdeaValue);
            GroupedIdeaValuesView.Refresh();
        }

        private void Flip(object obj)
        {
            _ideaCategoryViewModel.SelectedIdeaElement = this;
        }
    }
}
