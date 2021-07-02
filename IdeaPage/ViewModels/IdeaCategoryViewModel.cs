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
    public class IdeaCategoryViewModel : NotifyPropertyChanges
    {
        private readonly IdeaElementListViewModel _ideaElementListViewModel;
        private readonly IdeaViewModel _ideaViewModel;
        private readonly IIdeaDataService _ideaDataService;
        private ListCollectionView _groupedIdeaElementsView;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        private readonly IDialogService _dialogService;

        public IdeaCategoryViewModel(IdeaViewModel idea, IdeaCategory ideaCategory, IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _ideaDataService = ideaDataService;
            _dialogService = dialogService;
            _ideaViewModel = idea;
            IdeaCategory = ideaCategory;
            _ideaElementListViewModel ??= new IdeaElementListViewModel(this, ideaDataService, _dialogService);
            EditCommand = new DelegateCommand(EditExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
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
        public DelegateCommand DeleteSelfCommand { get; set; }

        public void EditExecute()
        {
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

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
            if (GroupedIdeaElementsView.GroupDescriptions != null)
                GroupedIdeaElementsView.GroupDescriptions.Add(new PropertyGroupDescription
                {
                    PropertyName = propertyName,
                    Converter = new NameToInitialConverter()
                });
            GroupedIdeaElementsView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedIdeaElement));
        }


        public IdeaCategory IdeaCategory { get; }

        public ICommand DeleteCommand { get; set; }


        private async void DeleteExecute()
        {
            var selectedIdeaCategory = this;
            bool delete = false;
            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete category?" },
                    { "message", $"Do you really want to delete the category '{selectedIdeaCategory.IdeaCategory.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if (!delete) return;
            _ideaViewModel.SelectedIdeaCategory = selectedIdeaCategory;
            await _ideaViewModel.DeleteIdeaCategoryAsync();
        }
        public async Task DeleteIdeaElementAsync()
        {
            await _ideaElementListViewModel.DeleteIdeaElementAsync(SelectedIdeaElement);
            GroupedIdeaElementsView.Refresh();
        }
    }
}
