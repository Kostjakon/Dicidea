using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Converters;
using Dicidea.Core.Models;
using Dicidea.Core.Helper;
using Prism;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace DicePage.ViewModels
{
    public class DiceDetailViewModel : NotifyPropertyChanges, INavigationAware
    {
        
        private DiceListViewModel _diceListViewModel;
        //private readonly IDialogCoordinator _dialogCoordinator;
        private ListCollectionView _groupedDiceView;
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        private bool _showSaved;
        private bool _isSaving;


        public DiceDetailViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            GoToDiceOverviewCommand = new DelegateCommand<object>(GoToDiceOverview, CanGoToDiceOverview);
            AddCommand = new DelegateCommand(AddExecute);
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
        public ICommand GoToDiceOverviewCommand { get; }

        private bool CanGoToDiceOverview(object obj)
        {
            return true;
        }

        public NavigationParameters Parameters
        {
            get;
            set;
        }

        private void GoToDiceOverview(object obj)
        {
            Parameters.Add("diceListViewModel", _diceListViewModel);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceOverview), Parameters);
            _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
        }

        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        private async void AddExecute()
        {
            Debug.WriteLine("Add Category");
            await SelectedDice.AddCategoryAsync();
        }

        private async void DeleteExecute()
        {
            var selectedCategory = SelectedDice.SelectedCategory;
            bool delete = false;
            if (selectedCategory == null) return;
            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete category?" },
                    { "message", $"Do you really want to delete the category '{selectedCategory.Category.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if (!delete) return;
            await SelectedDice.DeleteCategoryAsync();
        }
        private async void SaveExecute()
        {
            IsSaving = true;
            await Task.Delay(3000);
            await _diceListViewModel.SaveDiceAsync();
            IsSaving = false;
            ShowSaved = true;
            await Task.Delay(3000);
            ShowSaved = false;
        }

        public IRegionManager RegionManager { get; private set; }
        public ListCollectionView GroupedDiceView
        {
            get => _groupedDiceView;
            set => SetProperty(ref _groupedDiceView, value);
        }

        public DiceViewModel SelectedDice
        {
            get
            {
                if (GroupedDiceView != null) return GroupedDiceView.CurrentItem as DiceViewModel;
                else return null;
            }
            set => GroupedDiceView.MoveCurrentTo(value);
        }

        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Dice.Name))
            {
                GroupedDiceView.Refresh();
            }
        }

        private void CreateGroupedView()
        {
            ObservableCollection<DiceViewModel> diceViewModels = _diceListViewModel.AllDice;
            foreach (var diceViewModel in diceViewModels)
            {
                diceViewModel.Dice.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "Dice.Name";
            GroupedDiceView = new ListCollectionView(diceViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = {new SortDescription(propertyName, ListSortDirection.Ascending)}
            };
            GroupedDiceView.GroupDescriptions?.Add(new PropertyGroupDescription
            {
                PropertyName = propertyName,
                Converter = new NameToInitialConverter()
            });
            GroupedDiceView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedDice));
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Debug.WriteLine("Navigated to Detail Dice");
            if (navigationContext != null)
            {
                Parameters = navigationContext.Parameters;
                if(navigationContext.Parameters["diceListViewModel"] != null)
                {
                    _diceListViewModel = navigationContext.Parameters["diceListViewModel"] as DiceListViewModel;
                    CreateGroupedView();
                    if (navigationContext.Parameters["selectedDice"] != null)
                    {
                        DiceViewModel selectedDice = navigationContext.Parameters["selectedDice"] as DiceViewModel;
                        GroupedDiceView.MoveCurrentTo(selectedDice);
                        GroupedDiceView.Refresh();
                    }
                    else
                    {
                        GroupedDiceView.MoveCurrentToFirst();
                        GroupedDiceView.Refresh();
                        if (GroupedDiceView.Count > 0) Parameters.Add("selectedDice", GroupedDiceView.GetItemAt(0));
                    }
                }


                if (navigationContext.Parameters["regionManager"] != null)
                {
                    RegionManager = navigationContext.Parameters["regionManager"] as IRegionManager;
                }
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }



        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Debug.WriteLine("Not implemented, navigated from DiceDetail to some other side");
        }
    }
}
