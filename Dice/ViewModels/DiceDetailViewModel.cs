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
    public class DiceDetailViewModel : NotifyPropertyChanges, INavigationAware, IActiveAware
    {

        private bool _isActive;
        private DiceListViewModel _diceListViewModel;
        //private readonly IDialogCoordinator _dialogCoordinator;
        private ListCollectionView _groupedDiceView;
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;
        private readonly IDialogService _dialogService;
        private bool _showSaved = false;
        private bool _isSaving = false;


        public DiceDetailViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            GoToDiceOverviewCommand = new DelegateCommand<object>(GoToDiceOverview, CanGoToDiceOverview);
            AddCommand = new DelegateCommand(AddExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            EditCommand = new DelegateCommand(EditExecute);
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
        public ICommand GoToDiceOverviewCommand { get; private set; }

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
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        private async void AddExecute()
        {
            Debug.WriteLine("Add Category");
            await SelectedDice.AddCategoryAsync();
        }
        private void EditExecute()
        {
            //Task.Run(SelectedDice.EditCategoryAsync);
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

        public bool IsEditEnabled => GroupedDiceView.CurrentItem != null;

        public DiceViewModel SelectedDice
        {
            get
            {
                if (GroupedDiceView != null) return GroupedDiceView.CurrentItem as DiceViewModel;
                else return null;
            }
            set => GroupedDiceView.MoveCurrentTo(value);
        }

        private async void DeleteExecute(object obj)
        {
            var selectedDice = SelectedDice;
            bool delete = false;
            if (selectedDice == null) return;
            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete dice?" },
                    { "message", $"Do you really want to delete the dice '{selectedDice.Dice.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if (!delete) return;
            await _diceListViewModel.DeleteDiceAsync(selectedDice);
            GroupedDiceView.Refresh();
            
        }

        private async void NewExecute(object obj)
        {
            var newDice = await _diceListViewModel.AddDiceAsync();
            GroupedDiceView.Refresh();
            GroupedDiceView.MoveCurrentTo(newDice);

            newDice.Dice.WhenPropertyChanged.Subscribe(OnNext);
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
            GroupedDiceView.GroupDescriptions.Add(new PropertyGroupDescription
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
                        if (GroupedDiceView.Count > 0) _parameters.Add("selectedDice", GroupedDiceView.GetItemAt(0));
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
        

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveChanged();
            }
        }

        private void OnIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler IsActiveChanged;
    }
}
