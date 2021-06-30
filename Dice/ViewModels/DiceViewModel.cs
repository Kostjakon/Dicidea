using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Commands;
using Prism.Services.Dialogs;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace DicePage.ViewModels
{
    public class DiceViewModel : NotifyPropertyChanges
    {
        private CategoryListViewModel _categoryListViewModel;
        private ListCollectionView _groupedCategoriesView;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        private ElementViewModel _selectedElement;

        private readonly IDialogService _dialogService;
        //private readonly object _lock = new object();
        public DiceViewModel(Dice dice, IDiceDataService diceDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            //SendMailCommand = new DelegateCommand(SendMailCommand, CanSendMailExecute);
            if(GroupedCategoriesView != null)
            {
                Debug.WriteLine("Binding of GroupedCategoriesView in DiceViewModel");
                //System.Windows.Data.BindingOperations.EnableCollectionSynchronization(GroupedCategoriesView, _lock);
            }
            Dice = dice;
            
            DiceViewModel self = this;
            if (_categoryListViewModel == null)
            {
                _categoryListViewModel = new CategoryListViewModel(self, diceDataService, _dialogService);
            }
            CreateGroupedView();
            AddCommand = new DelegateCommand(AddExecute);
            EditCommand = new DelegateCommand(EditExecute);
        }

        public ElementViewModel SelectedElement
        {
            get => _selectedElement;
            set => SetProperty(ref _selectedElement, value);
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


        public ListCollectionView GroupedCategoriesView
        {
            get => _groupedCategoriesView;
            set => SetProperty(ref _groupedCategoriesView, value);
        }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        private async void AddExecute()
        {
            Debug.WriteLine("Add Category");
            //await Application.Current.Dispatcher.BeginInvoke(() => Task.Run(_categoryListViewModel.AddCategoryAsync));
            /*
            Thread thread = new Thread(delegate()
            {
                AddCategory();
            });
            thread.IsBackground = true;
            thread.Start();
            */
            await _categoryListViewModel.AddCategoryAsync();
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
            Debug.WriteLine("Edit Dice");
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

        }

        public CategoryViewModel SelectedCategory
        {
            get
            {
                if (GroupedCategoriesView != null) return GroupedCategoriesView.CurrentItem as CategoryViewModel;
                else return null;
            }
            set => GroupedCategoriesView.MoveCurrentTo(value);
        }

        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Category.Name))
            {
                //GroupedCategoriesView.Refresh(); <- Hier ist das Problem
            }
        }

        private void CreateGroupedView()
        {
            ObservableCollection<CategoryViewModel> categoryViewModels = _categoryListViewModel.Categories;
            //foreach (var categoryViewModel in categoryViewModels)
            //{
            //    categoryViewModel.Category.WhenPropertyChanged.Subscribe(OnNext);
            //}

            var propertyName = "Category.Name";
            GroupedCategoriesView = new ListCollectionView(categoryViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            //GroupedCategoriesView.GroupDescriptions.Add(new PropertyGroupDescription
            //{
            //    PropertyName = propertyName,
            //    Converter = new NameToInitialConverter()
            //});
            
            GroupedCategoriesView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedCategory));
        }

        public async Task AddCategoryAsync()
        {
            Debug.WriteLine("Add Category");
            await _categoryListViewModel.AddCategoryAsync();
            //GroupedCategoriesView.Refresh();
        }
        public async Task DeleteCategoryAsync()
        {
            await _categoryListViewModel.DeleteCategoryAsync(SelectedCategory);
            GroupedCategoriesView.Refresh();
        }


        public Dice Dice { get; }

    }
}
