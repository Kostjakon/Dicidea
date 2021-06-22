﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Commands;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace DicePage.ViewModels
{
    public class DiceViewModel : NotifyPropertyChanges
    {
        private CategoryListViewModel _categoryListViewModel;
        private ListCollectionView _groupedCategoriesView;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        public DiceViewModel(Dice dice, IDiceDataService diceDataService)
        {
            //SendMailCommand = new DelegateCommand(SendMailCommand, CanSendMailExecute);
            Dice = dice;
            
            DiceViewModel self = this;
            if (_categoryListViewModel == null)
            {
                _categoryListViewModel = new CategoryListViewModel(self, diceDataService);
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
            //await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, () => Task.Run(_categoryListViewModel.AddCategoryAsync));
            await Task.Run(_categoryListViewModel.AddCategoryAsync);
            GroupedCategoriesView.Refresh();
        }

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
            foreach (var categoryViewModel in categoryViewModels)
            {
                categoryViewModel.Category.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "Category.Name";
            GroupedCategoriesView = new ListCollectionView(categoryViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            GroupedCategoriesView.GroupDescriptions.Add(new PropertyGroupDescription
            {
                PropertyName = propertyName,
                Converter = new NameToInitialConverter()
            });
            GroupedCategoriesView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedCategory));
        }

        public async void AddCategoryAsync()
        {
            Debug.WriteLine("Add Category");
            await Task.Run(_categoryListViewModel.AddCategoryAsync);
            //GroupedCategoriesView.Refresh();
        }
        public void EditDiceAsync()
        {
            Debug.WriteLine("Edit Dice");
        }
        public async void DeleteCategoryAsync()
        {
            Debug.WriteLine("Delete Dice");
            _categoryListViewModel.DeleteCategoryAsync(SelectedCategory);
            GroupedCategoriesView.Refresh();
        }


        public Dice Dice { get; }

    }
}
