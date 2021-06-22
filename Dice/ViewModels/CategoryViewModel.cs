using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Commands;

namespace DicePage.ViewModels
{
    public class CategoryViewModel : NotifyPropertyChanges
    {
        private ElementListViewModel _elementListViewModel;
        private ListCollectionView _groupedElementsView;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;

        public CategoryViewModel(Dice dice, Category category, IDiceDataService diceDataService)
        {
            FlipCommand = new DelegateCommand<object>(Flip, CanFlip);
            Category = category;
            // TODO: SendMailCommand is not needed, because dice doesn't have an email
            CategoryViewModel self = this;
            if (_elementListViewModel == null)
            {
                _elementListViewModel = new ElementListViewModel(dice, self, diceDataService);
            }
            EditCommand = new DelegateCommand(EditExecute);
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
            Debug.WriteLine("Edit Dice");
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

        }

        public ListCollectionView GroupedElementsView
        {
            get => _groupedElementsView;
            set => SetProperty(ref _groupedElementsView, value);
        }


        public ElementViewModel SelectedElement
        {
            get
            {
                if (GroupedElementsView != null) return GroupedElementsView.CurrentItem as ElementViewModel;
                else return null;
            }
            set => GroupedElementsView.MoveCurrentTo(value);
        }

        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Category.Name))
            {
                //GroupedElementsView.Refresh();
            }
        }

        private void CreateGroupedView()
        {
            ObservableCollection<ElementViewModel> elementViewModels = _elementListViewModel.Elements;
            foreach (var elementViewModel in elementViewModels)
            {
                elementViewModel.Element.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "Category.Name";
            GroupedElementsView = new ListCollectionView(elementViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            GroupedElementsView.GroupDescriptions.Add(new PropertyGroupDescription
            {
                PropertyName = propertyName,
                Converter = new NameToInitialConverter()
            });
            GroupedElementsView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedElement));
        }


        public Category Category { get; }

        public ICommand FlipCommand { get; set; }

        private bool CanFlip(object obj)
        {
            return true;
        }

        private void Flip(object obj)
        {
            Debug.WriteLine("Flip Category: " + Category.Name);
        }
    }
}
