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

namespace IdeaPage.ViewModels
{
    public class IdeaViewModel : NotifyPropertyChanges
    {
        private IdeaCategoryListViewModel _ideaCategoryListViewModel;
        private ListCollectionView _groupedIdeaCategoriesView;
        private bool _isEditEnabled;

        private bool _isEditDisabled = true;

        //private readonly object _lock = new object();
        public IdeaViewModel(Idea idea, IIdeaDataService ideaDataService)
        {
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
                _ideaCategoryListViewModel = new IdeaCategoryListViewModel(self, ideaDataService);
            }

            CreateGroupedView();
            AddCommand = new DelegateCommand(AddExecute);
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

        private async void AddExecute()
        {
            Debug.WriteLine("Add IdeaCategory");
            await _ideaCategoryListViewModel.AddIdeaCategoryAsync();
            //GroupedCategoriesView.Refresh();

        }

        /*
        public void AddCategory()
        {
            Dispatcher.BeginInvoke((Action) (async () =>
            {
                await Task.Run(_categoryListViewModel.AddCategoryAsync);
            }));
        }
        */

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
                //GroupedCategoriesView.Refresh(); <- Hier ist das Problem
            }
        }

        private void CreateGroupedView()
        {
            ObservableCollection<IdeaCategoryViewModel> ideaCategoryViewModels =
                _ideaCategoryListViewModel.IdeaCategories;
            //foreach (var categoryViewModel in categoryViewModels)
            //{
            //    categoryViewModel.Category.WhenPropertyChanged.Subscribe(OnNext);
            //}

            var propertyName = "IdeaCategory.Name";
            GroupedIdeaCategoriesView = new ListCollectionView(ideaCategoryViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = {new SortDescription(propertyName, ListSortDirection.Ascending)}
            };
            //GroupedCategoriesView.GroupDescriptions.Add(new PropertyGroupDescription
            //{
            //    PropertyName = propertyName,
            //    Converter = new NameToInitialConverter()
            //});

            GroupedIdeaCategoriesView.CurrentChanged +=
                (sender, args) => OnPropertyChanged(nameof(SelectedIdeaCategory));
        }

        public async Task AddIdeaCategoryAsync()
        {
            Debug.WriteLine("Add Idea Category");
            await _ideaCategoryListViewModel.AddIdeaCategoryAsync();
            //GroupedCategoriesView.Refresh();
        }

        public async Task DeleteIdeaCategoryAsync()
        {
            Debug.WriteLine("Delete Dice");
            await _ideaCategoryListViewModel.DeleteIdeaCategoryAsync(SelectedIdeaCategory);
            GroupedIdeaCategoriesView.Refresh();
        }


        public Idea Idea { get; }
    }
}
