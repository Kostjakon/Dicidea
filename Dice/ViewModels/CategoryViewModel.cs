using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
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
        private readonly ElementListViewModel _elementListViewModel;
        private readonly DiceViewModel _diceViewModel;
        private ListCollectionView _groupedElementsView;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;

        public CategoryViewModel(DiceViewModel dice, Category category, IDiceDataService diceDataService)
        {
            DeleteCommand = new DelegateCommand<object>(DeleteExecute);
            _diceViewModel = dice;
            Category = category;
            // TODO: SendMailCommand is not needed, because dice doesn't have an email
            var self = this;
            _elementListViewModel ??= new ElementListViewModel(dice, self, diceDataService);
            EditCommand = new DelegateCommand(EditExecute);
            ActivateCommand = new DelegateCommand(ActivateExecute);
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
        public DelegateCommand ActivateCommand { get; set; }

        public void ActivateExecute()
        {
            Category.Active = !Category.Active;
        }
        public void EditExecute()
        {
            Debug.WriteLine("Edit Dice");
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

        }
        public async void AddExecute()
        {
            await AddElementAsync();
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

        public ICommand DeleteCommand { get; set; }
        

        private async void DeleteExecute(object obj)
        {
            _diceViewModel.SelectedCategory = this;
            await _diceViewModel.DeleteCategoryAsync();
        }
        public async Task DeleteElementAsync()
        {
            Debug.WriteLine("Delete Element");
            await _elementListViewModel.DeleteElementAsync(SelectedElement);
            GroupedElementsView.Refresh();
        }
        public async Task AddElementAsync()
        {
            Debug.WriteLine("Add Category");
            await _elementListViewModel.AddElementAsync();
            //GroupedCategoriesView.Refresh();
        }
    }
}
