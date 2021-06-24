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
    public class IdeaCategoryViewModel : NotifyPropertyChanges
    {
        private readonly IdeaElementListViewModel _ideaElementListViewModel;
        private readonly IdeaViewModel _ideaViewModel;
        private ListCollectionView _groupedIdeaElementsView;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;

        public IdeaCategoryViewModel(IdeaViewModel idea, IdeaCategory ideaCategory, IIdeaDataService ideaDataService)
        {
            DeleteCommand = new DelegateCommand<object>(DeleteExecute);
            _ideaViewModel = idea;
            IdeaCategory = ideaCategory;
            var self = this;
            _ideaElementListViewModel ??= new IdeaElementListViewModel(idea.Idea, self, ideaDataService);
            EditCommand = new DelegateCommand(EditExecute);
            AddCommand = new DelegateCommand(AddExecute);
            CreateGroupedView();
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

        public void EditExecute()
        {
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

        }
        public async void AddExecute()
        {
            await AddIdeaElementAsync();
        }

        public ListCollectionView GroupedIdeaElementsView
        {
            get => _groupedIdeaElementsView;
            set => SetProperty(ref _groupedIdeaElementsView, value);
        }


        public IdeaElementViewModel SelectedIdeaElement
        {
            get
            {
                if (GroupedIdeaElementsView != null) return GroupedIdeaElementsView.CurrentItem as IdeaElementViewModel;
                else return null;
            }
            set => GroupedIdeaElementsView.MoveCurrentTo(value);
        }

        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(IdeaCategory.Name))
            {
                //GroupedElementsView.Refresh();
            }
        }

        private void CreateGroupedView()
        {
            ObservableCollection<IdeaElementViewModel> ideaElementViewModels = _ideaElementListViewModel.IdeaElements;
            foreach (var ideaElementViewModel in ideaElementViewModels)
            {
                ideaElementViewModel.IdeaElement.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "IdeaCategory.Name";
            GroupedIdeaElementsView = new ListCollectionView(ideaElementViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            GroupedIdeaElementsView.GroupDescriptions.Add(new PropertyGroupDescription
            {
                PropertyName = propertyName,
                Converter = new NameToInitialConverter()
            });
            GroupedIdeaElementsView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedIdeaElement));
        }


        public IdeaCategory IdeaCategory { get; }

        public ICommand DeleteCommand { get; set; }


        private async void DeleteExecute(object obj)
        {
            _ideaViewModel.SelectedIdeaCategory = this;
            await _ideaViewModel.DeleteIdeaCategoryAsync();
        }
        public async Task DeleteIdeaElementAsync()
        {
            Debug.WriteLine("Delete IdeaElement");
            await _ideaElementListViewModel.DeleteIdeaElementAsync(SelectedIdeaElement);
            GroupedIdeaElementsView.Refresh();
        }
        public async Task AddIdeaElementAsync()
        {
            Debug.WriteLine("Add IdeaCategory");
            await _ideaElementListViewModel.AddIdeaElementAsync();
            //GroupedCategoriesView.Refresh();
        }
    }
}
