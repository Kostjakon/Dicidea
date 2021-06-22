using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Converters;
using Dicidea.Core.Models;
using Dicidea.Core.Helper;
using MahApps.Metro.Controls.Dialogs;
using Prism;
using Prism.Commands;
using Prism.Regions;

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


        public DiceDetailViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            GoToDiceOverviewCommand = new DelegateCommand<object>(GoToDiceOverview, CanGoToDiceOverview);
            AddCommand = new DelegateCommand(AddExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            EditCommand = new DelegateCommand(EditExecute);
            SaveCommand = new DelegateCommand(SaveExecute);
        }
        public ICommand GoToDiceOverviewCommand { get; private set; }

        private bool CanGoToDiceOverview(object obj)
        {
            return true;
        }

        private void GoToDiceOverview(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(DiceOverview), _parameters);
            _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
        }

        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        private void AddExecute()
        {
            Debug.WriteLine("Add Category");
            Task.Run(SelectedDice.AddCategoryAsync);
        }
        private void EditExecute()
        {
            //Task.Run(SelectedDice.EditCategoryAsync);
        }
        private async void DeleteExecute()
        {
            await Task.Run(SelectedDice.DeleteCategoryAsync);
        }
        private async void SaveExecute()
        {
            await Task.Run(_diceListViewModel.SaveDiceAsync);
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
            if(selectedDice == null) return;
            /*
            var result = await _dialogCoordinator.ShowMessageAsync(this, "Delete dice?", $"Are you sure you want to delete '{selectedDice.Dice.Name}'?",
                MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
                {
                    AffirmativeButtonText = "Yes",
                    NegativeButtonText = "No",
                    AnimateHide = false,
                    AnimateShow = false,
                    DefaultButtonFocus = MessageDialogResult.Negative
                });
            */
            //if (result == MessageDialogResult.Affirmative)
            //{
                await _diceListViewModel.DeleteDiceAsync(selectedDice);
            //}
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

        private async void SaveExecute(object obj)
        {
            await _diceListViewModel.SaveDiceAsync();
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
                _parameters = new NavigationParameters();
                _parameters.Add("diceListViewModel", navigationContext.Parameters["diceListViewModel"] as DiceListViewModel);
                    //navigationContext.Parameters;
                if(navigationContext.Parameters["diceListViewModel"] != null)
                {
                    _diceListViewModel = navigationContext.Parameters["diceListViewModel"] as DiceListViewModel;
                    CreateGroupedView();
                    if (navigationContext.Parameters["selectedDice"] != null)
                    {
                        Debug.WriteLine("Selected dice is not null");
                        DiceViewModel selectedDice = navigationContext.Parameters["selectedDice"] as DiceViewModel;
                        Debug.WriteLine("Selected Dice is: " + selectedDice.Dice.Name);
                        GroupedDiceView.MoveCurrentTo(selectedDice);
                        GroupedDiceView.Refresh();
                        Debug.WriteLine(GroupedDiceView.CurrentItem == selectedDice);
                    }
                    else
                    {
                        Debug.WriteLine("Selected dice is null");
                        GroupedDiceView.MoveCurrentToFirst();
                        if(GroupedDiceView.Count > 0) _parameters.Add("selectedDice", GroupedDiceView.GetItemAt(0));
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
