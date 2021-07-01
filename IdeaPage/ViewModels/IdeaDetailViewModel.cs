using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Dicidea.Core.Constants;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using IdeaPage.Views;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    public class IdeaDetailViewModel : NotifyPropertyChanges, INavigationAware
    {
        private string _filterText;
        private SortOrder _sortOrder = SortOrder.Unsorted;
        private ListCollectionView _groupedIdeaView;
        private IdeaListViewModel _ideaListViewModel;
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;
        private readonly IDialogService _dialogService;
        private bool _showSaved = false;
        private bool _isSaving = false;

        public IdeaDetailViewModel(IRegionManager regionManager, IDialogService dialogService)
        {

            _dialogService = dialogService;
            _regionManager = regionManager;
            if (IdeaView != null)
            {
                IdeaView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedIdea));
                IdeaView.Refresh();
            }
            SortCommand = new DelegateCommand(SortExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            SaveCommand = new DelegateCommand(SaveExecute);
        }

        public bool ShowSaved
        {
            get => _showSaved;
            set => SetProperty(ref _showSaved, value);
        }
        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public bool IsEditEnabled
        {
            get;
            private set;
        }

        public IdeaListViewModel IdeaListViewModel
        {
            get => _ideaListViewModel;
        }

        public SortOrder SortOrder
        {
            get => _sortOrder;
            set
            {
                SetProperty(ref _sortOrder, value);
                switch (value)
                {
                    case SortOrder.Ascending:
                        SortAscending();
                        break;
                    case SortOrder.Descending:
                        SortDescending();
                        break;
                    case SortOrder.Unsorted:
                        Unsort();
                        break;
                }
            }
        }

        public DelegateCommand SortCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        public DelegateCommand GoToIdeaOverview =>
            new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(IdeaOverview), _parameters);
                _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
            });

        public ListCollectionView IdeaView { get; private set; }

        public IRegionManager RegionManager { get => _regionManager; }

        public string FilterText
        {
            get => _filterText;
            set
            {
                SetProperty(ref _filterText, value);
                Filter();
            }
        }


        public IdeaViewModel SelectedIdea
        {
            get
            {
                if (GroupedIdeaView != null) return GroupedIdeaView.CurrentItem as IdeaViewModel;
                else return null;
            }
            set => GroupedIdeaView.MoveCurrentTo(value);
        }

        public ListCollectionView GroupedIdeaView
        {
            get => _groupedIdeaView;
            set => SetProperty(ref _groupedIdeaView, value);
        }

        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Idea.Name))
            {
                GroupedIdeaView.Refresh();
            }
        }

        private void Unsort()
        {
            IdeaView.IsLiveSorting = false;
            IdeaView.CustomSort = null;
        }

        private void SortAscending()
        {
            IdeaView.IsLiveSorting = true;
            IdeaView.CustomSort = Comparer<IdeaViewModel>.Create((i1, i2) => string.Compare(i1.Idea.Name, i2.Idea.Name, StringComparison.OrdinalIgnoreCase));
        }

        private void SortDescending()
        {
            IdeaView.IsLiveSorting = true;
            IdeaView.CustomSort = Comparer<IdeaViewModel>.Create((i1, i2) => string.Compare(i2.Idea.Name, i1.Idea.Name, StringComparison.OrdinalIgnoreCase));
        }

        private void SortExecute()
        {
            switch (SortOrder)
            {
                case SortOrder.Descending:
                    SortOrder = SortOrder.Unsorted;
                    break;
                case SortOrder.Unsorted:
                    SortOrder = SortOrder.Ascending;
                    break;
                case SortOrder.Ascending:
                    SortOrder = SortOrder.Descending;
                    break;
            }
        }
        

        private async void SaveExecute()
        {
            IsSaving = true;
            await Task.Delay(3000);
            await _ideaListViewModel.SaveIdeasAsync();
            IsSaving = false;
            ShowSaved = true;
            await Task.Delay(3000);
            ShowSaved = false;
        }

        private async void DeleteExecute()
        {
            var selectedIdea = SelectedIdea;
            bool delete = false;
            if (selectedIdea == null) return;
            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete idea?" },
                    { "message", $"Do you really want to delete '{selectedIdea.Idea.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if(!delete) return;
            await _ideaListViewModel.DeleteIdeaAsync(SelectedIdea);
            GroupedIdeaView.Refresh();
        }


        private void CreateGroupedView()
        {
            ObservableCollection<IdeaViewModel> ideaViewModels = _ideaListViewModel.AllIdeas;
            foreach (var ideaViewModel in ideaViewModels)
            {
                ideaViewModel.Idea.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "Idea.SectionName";
            GroupedIdeaView = new ListCollectionView(ideaViewModels)
            {
                IsLiveSorting = true,
                IsLiveGrouping = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            GroupedIdeaView.GroupDescriptions.Add(new PropertyGroupDescription
            {
                PropertyName = propertyName
            });
            GroupedIdeaView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedIdea));
        }

        private void Filter()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                IdeaView.Filter = o => true;
            }
            else
            {
                IdeaView.IsLiveFiltering = true;
                IdeaView.Filter = o =>
                {
                    if (o is IdeaViewModel vm)
                        return vm.Idea.Name?.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0;
                    return true;
                };
            }
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _parameters = navigationContext.Parameters;
            if (navigationContext != null)
            {
                Debug.WriteLine("Navigated to IdeaDetailView");
                if (navigationContext.Parameters["ideaListViewModel"] != null)
                {
                    Debug.WriteLine("Set ideaListViewModel");
                    _ideaListViewModel = navigationContext.Parameters.GetValue<IdeaListViewModel>("ideaListViewModel");
                        //new IdeaListViewModel(navigationContext.Parameters.GetValue<IIdeaDataService>("ideaDataService"));
                    CreateGroupedView();
                    if (navigationContext.Parameters["selectedIdea"] != null)
                    {
                        IdeaViewModel selectedIdea = navigationContext.Parameters["selectedIdea"] as IdeaViewModel;
                        Debug.WriteLine("Selected Idea is: " + selectedIdea.Idea.Id);
                        GroupedIdeaView.MoveCurrentTo(selectedIdea);
                        GroupedIdeaView.Refresh();
                        IdeaViewModel idea = GroupedIdeaView.CurrentItem as IdeaViewModel;
                        Debug.WriteLine(idea == selectedIdea);
                    }
                    else
                    {
                        GroupedIdeaView.MoveCurrentToFirst();
                        if (GroupedIdeaView.Count > 0) _parameters.Add("selectedIdea", GroupedIdeaView.GetItemAt(0));
                    }
                }
                SortCommand = new DelegateCommand(SortExecute);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }



        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Debug.WriteLine("Not implemented, navigated from IdeaOverview to some other side");
        }
    }
}
