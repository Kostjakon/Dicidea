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
using Prism.Services.Dialogs;

namespace DicePage.ViewModels
{
    public class ElementViewModel : NotifyPropertyChanges
    {
        private readonly CategoryViewModel _categoryViewModel;
        private readonly IDialogService _dialogService;
        private readonly ValueListViewModel _valueListViewModel;
        private ListCollectionView _groupedValuesView;
        private readonly DiceViewModel _diceViewModel;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        public ElementViewModel(Element element, CategoryViewModel categoryViewModel, DiceViewModel diceViewModel, IDiceDataService diceDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _categoryViewModel = categoryViewModel;
            _diceViewModel = diceViewModel;
            FlipCommand = new DelegateCommand<object>(Flip, CanFlip);
            Element = element;
            EditCommand = new DelegateCommand(EditExecute);
            AddCommand = new DelegateCommand(AddExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            ActivateCommand = new DelegateCommand(ActivateExecute);
            ElementViewModel self = this;
            if (_valueListViewModel == null)
            {
                _valueListViewModel = new ValueListViewModel(diceViewModel.Dice, _categoryViewModel.Category, self, diceDataService, _dialogService);
            }
            CreateGroupedView();
        }

        public ListCollectionView GroupedValuesView
        {
            get => _groupedValuesView;
            set => SetProperty(ref _groupedValuesView, value);
        }

        public ValueViewModel SelectedValue
        {
            get
            {
                if (GroupedValuesView != null) return GroupedValuesView.CurrentItem as ValueViewModel;
                return null;
            }
            set => GroupedValuesView.MoveCurrentTo(value);
        }
        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Element.Name))
            {
                //GroupedElementsView.Refresh();
            }
        }

        private void CreateGroupedView()
        {
            ObservableCollection<ValueViewModel> valueViewModels = _valueListViewModel.Values;
            foreach (var valueViewModel in valueViewModels)
            {
                valueViewModel.Value.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "Element.Name";
            GroupedValuesView = new ListCollectionView(valueViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            if (GroupedValuesView.GroupDescriptions != null)
                GroupedValuesView.GroupDescriptions.Add(new PropertyGroupDescription
                {
                    PropertyName = propertyName,
                    Converter = new NameToInitialConverter()
                });
            GroupedValuesView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedValue));
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
        public DelegateCommand ActivateCommand { get; set; }

        public void ActivateExecute()
        {
            Element.Active = !Element.Active;
        }

        public void EditExecute()
        {
            Debug.WriteLine("Edit Dice");
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;
        }
        public async void AddExecute()
        {
            await AddValueAsync();
        }
        public async void DeleteExecute()
        {
            var selectedElement = this;
            bool delete = false;
            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete element?" },
                    { "message", $"Do you really want to delete the element '{selectedElement.Element.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if (!delete) return;
            _categoryViewModel.SelectedElement = selectedElement;
            await _categoryViewModel.DeleteElementAsync();
        }

        public bool IsSelected
        {
            get => _categoryViewModel.SelectedElement == this;
        }

        public Category Category
        {
            get => _categoryViewModel.Category;
        }
        public Dice Dice
        {
            get => _diceViewModel.Dice;
        }

        public Element Element { get; }
        public ICommand FlipCommand { get; set; }

        private bool CanFlip(object obj)
        {
            return true;
        }

        public async Task AddValueAsync()
        {
            Debug.WriteLine("Add Category");
            await _valueListViewModel.AddValueAsync();
            //GroupedCategoriesView.Refresh();
        }
        public async Task DeleteValueAsync()
        {
            await _valueListViewModel.DeleteValueAsync(SelectedValue);
            GroupedValuesView.Refresh();
        }

        private void Flip(object obj)
        {
            _diceViewModel.SelectedElement = this;
            _categoryViewModel.SelectedElement = this;
            _diceViewModel.SelectedCategory = _categoryViewModel;
            Debug.WriteLine("Flip Element: " + Element.Name);
            Debug.WriteLine("Is selected: "+IsSelected);
            Debug.WriteLine(_categoryViewModel.Category.Name);
        }
    }
}
