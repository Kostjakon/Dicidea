using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    public class IdeaViewModel : NotifyPropertyChanges
    {
        private readonly IdeaCategoryListViewModel _ideaCategoryListViewModel;
        private ListCollectionView _groupedIdeaCategoriesView;
        private bool _isEditEnabled;
        private readonly IDialogService _dialogService;

        private bool _isEditDisabled = true;

        //private readonly object _lock = new object();
        public IdeaViewModel(Idea idea, IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            //SendMailCommand = new DelegateCommand(SendMailCommand, CanSendMailExecute);
            if (GroupedIdeaCategoriesView != null)
            {
                Debug.WriteLine("Binding of GroupedCategoriesView in DiceViewModel");
                //System.Windows.Data.BindingOperations.EnableCollectionSynchronization(GroupedCategoriesView, _lock);
            }

            Idea = idea;

            IdeaViewModel self = this;
            if (_ideaCategoryListViewModel == null)
            {
                _ideaCategoryListViewModel = new IdeaCategoryListViewModel(self, ideaDataService, _dialogService);
            }

            CreateGroupedView();
            EditCommand = new DelegateCommand(EditExecute);
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


        public ListCollectionView GroupedIdeaCategoriesView
        {
            get => _groupedIdeaCategoriesView;
            set => SetProperty(ref _groupedIdeaCategoriesView, value);
        }

        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        

        public void EditExecute()
        {
            Debug.WriteLine("Edit Idea");
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

        }

        public IdeaCategoryViewModel SelectedIdeaCategory
        {
            get
            {
                if (GroupedIdeaCategoriesView != null)
                    return GroupedIdeaCategoriesView.CurrentItem as IdeaCategoryViewModel;
                else return null;
            }
            set => GroupedIdeaCategoriesView.MoveCurrentTo(value);
        }

        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(IdeaCategory.Name))
            {
                GroupedIdeaCategoriesView.Refresh();
            }
        }

        private void CreateGroupedView()
        {
            ObservableCollection<IdeaCategoryViewModel> ideaCategoryViewModels =
                _ideaCategoryListViewModel.IdeaCategories;
            foreach (var ideaCategoryViewModel in ideaCategoryViewModels)
            {
                ideaCategoryViewModel.IdeaCategory.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "IdeaCategory.Name";
            GroupedIdeaCategoriesView = new ListCollectionView(ideaCategoryViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = {new SortDescription(propertyName, ListSortDirection.Ascending)}
            };
            //GroupedIdeaCategoriesView.GroupDescriptions.Add(new PropertyGroupDescription
            //{
            //    PropertyName = propertyName,
            //    Converter = new NameToInitialConverter()
            //});

            GroupedIdeaCategoriesView.CurrentChanged +=
                (sender, args) => OnPropertyChanged(nameof(SelectedIdeaCategory));
        }

        public async Task DeleteIdeaCategoryAsync()
        {
            var selectedIdeaCategory = SelectedIdeaCategory;
            await _ideaCategoryListViewModel.DeleteIdeaCategoryAsync(selectedIdeaCategory);
            GroupedIdeaCategoriesView.Refresh();
        }


        public Idea Idea { get; }
    }
}
