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
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    public class IdeaElementViewModel : NotifyPropertyChanges
    {
        private readonly IdeaCategoryViewModel _ideaCategoryViewModel;
        private readonly IdeaValueListViewModel _ideaValueListViewModel;
        private ListCollectionView _groupedIdeaValuesView;
        private readonly IIdeaDataService _ideaDataService;
        private readonly IDialogService _dialogService;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        public IdeaElementViewModel(IdeaElement ideaElement, IdeaCategoryViewModel ideaCategoryViewModel, IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ideaCategoryViewModel = ideaCategoryViewModel;
            _ideaDataService = ideaDataService;
            FlipCommand = new DelegateCommand<object>(Flip, CanFlip);
            IdeaElement = ideaElement;
            EditCommand = new DelegateCommand(EditExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            IdeaElementViewModel self = this;
            if (_ideaValueListViewModel == null)
            {
                _ideaValueListViewModel = new IdeaValueListViewModel(self, ideaDataService, _dialogService);
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
        
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }

        public void EditExecute()
        {
            Debug.WriteLine("Edit Idea Element");
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;
        }
        public async void DeleteExecute()
        {
            var selectedIdeaElement = this;
            bool delete = false;
            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete element?" },
                    { "message", $"Do you really want to delete the element '{selectedIdeaElement.IdeaElement.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if(!delete) return;
            _ideaCategoryViewModel.SelectedIdeaElement = selectedIdeaElement;
            await _ideaCategoryViewModel.DeleteIdeaElementAsync(); 
        }

        public IdeaElement IdeaElement { get; }
        public ICommand FlipCommand { get; set; }

        private bool CanFlip(object obj)
        {
            return true;
        }
        
        public async Task DeleteIdeaValueAsync()
        {
            await _ideaValueListViewModel.DeleteIdeaValueAsync(SelectedIdeaValue);
            GroupedIdeaValuesView.Refresh();
        }

        private void Flip(object obj)
        {
            _ideaCategoryViewModel.SelectedIdeaElement = this;
        }
    }
}
