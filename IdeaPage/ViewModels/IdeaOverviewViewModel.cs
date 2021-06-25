using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Prism.Regions;

namespace IdeaPage.ViewModels
{
    public class IdeaOverviewViewModel : NotifyPropertyChanges, INavigationAware
    {
        private string _filterText;
        private SortOrder _sortOrder = SortOrder.Unsorted;
        private ListCollectionView _groupedIdeaView;
        private IdeaListViewModel _ideaListViewModel;
        private readonly IRegionManager _regionManager;

        public IdeaOverviewViewModel(IRegionManager regionManager)
        {

            _regionManager = regionManager;
            if (IdeaView != null)
            {
                IdeaView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedIdea));
                IdeaView.Refresh();
            }
            SortCommand = new DelegateCommand(SortExecute);
            AddCommand = new DelegateCommand(AddExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            SaveCommand = new DelegateCommand(SaveExecute);
        }

        public bool IsEditEnabled
        {
            get;
            private set;
        }

        public NavigationParameters Parameters
        {
            get;
            set;
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
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

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

        private async void AddExecute()
        {
            IdeaViewModel result = await _ideaListViewModel.AddIdeaAsync();
            GroupedIdeaView.MoveCurrentTo(result);
            GroupedIdeaView.Refresh();

        }
        private async void SaveExecute()
        {
            await _ideaListViewModel.SaveIdeasAsync();
        }

        private async void DeleteExecute()
        {
            Debug.WriteLine("Delete Idea");
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

            var propertyName = "Idea.Name";
            GroupedIdeaView = new ListCollectionView(ideaViewModels)
            {
                IsLiveSorting = true,
                IsLiveGrouping = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            GroupedIdeaView.GroupDescriptions.Add(new PropertyGroupDescription
            {
                PropertyName = propertyName,
                Converter = new NameToInitialConverter()
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
            Parameters = navigationContext.Parameters;
            if (navigationContext != null)
            {
                Debug.WriteLine(navigationContext.Parameters);
                if (navigationContext.Parameters["ideaListViewModel"] != null)
                {
                    _ideaListViewModel = navigationContext.Parameters["ideaListViewModel"] as IdeaListViewModel;
                    CreateGroupedView();
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
