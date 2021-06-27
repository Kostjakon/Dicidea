using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DicePage.ViewModels;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using IdeaPage.ViewModels;
using Prism.Regions;
using RollEmSpacePage.Views;

namespace RollEmSpacePage.ViewModels
{
    public class RollEmSpaceDetailViewModel : NotifyPropertyChanges, INavigationAware
    {

        private bool _isActive;
        private IdeaListViewModel _ideaListViewModel;
        //private readonly IDialogCoordinator _dialogCoordinator;
        private ListCollectionView _groupedIdeasView;
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;
        private DiceViewModel _selectedDice;
        private IIdeaDataService _ideaDataService;
        private DiceListViewModel _diceListViewModel;
        private Random _random = new Random();

        public RollEmSpaceDetailViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            GoToDiceCommand = new DelegateCommand<object>(GoToDice);
            GoToRollEmSpaceOverviewCommand = new DelegateCommand<object>(GoToRollEmSpaceOverview);
            RollCommand = new DelegateCommand(RollExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            EditCommand = new DelegateCommand(EditExecute);
            SaveCommand = new DelegateCommand(SaveExecute);
        }
        public ICommand GoToDiceCommand { get; private set; }
        public ICommand GoToRollEmSpaceOverviewCommand { get; private set; }
        

        private void GoToDice(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.LeftBottomContentRegion, nameof(DiceDetail), _parameters);
        }
        private void GoToRollEmSpaceOverview(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(RollEmSpaceOverview), _parameters);
            _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
        }

        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand RollCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        private async void RollExecute()
        {
            // TODO: Logic to roll ideas
            //IdeaCategoryListViewModel ideaCategory = new IdeaCategoryListViewModel(Idea, _ideaDataService);
            List<Idea> rolledIdeas = new List<Idea>();
            for (int j = 0; j < SelectedDice.Dice.Amount; j++)
            {
                Idea idea = new Idea(SelectedDice.Dice.Name, SelectedDice.Dice.Description);
                rolledIdeas.Add(idea);
                List<Category> Categories = SelectedDice.Dice.Categories;
                for (int i = 0; i < Categories.Count; i++)
                {
                    if (Categories[i].Active)
                    {
                        for (int l = 0; l < Categories[i].Amount; l++)
                        {
                            IdeaCategory ideaCategory = new IdeaCategory(Categories[i].Name);
                            idea.IdeaCategories.Add(ideaCategory);
                            List<Element> Elements = Categories[i].Elements;
                            for (int a = 0; a < Elements.Count; a++)
                            {
                                if (Elements[a].Active)
                                {
                                    
                                    for (int m = 0; m < Elements[a].Amount; m++)
                                    {
                                        IdeaElement ideaElement = new IdeaElement(Elements[a].Name);
                                        ideaCategory.IdeaElements.Add(ideaElement);
                                        List<Value> Values = Elements[a].Values;
                                        for (int k = 0; k < Elements[a].ValueAmount; k++)
                                        {
                                            bool valid = false;
                                            Value value = new Value(true);
                                            do
                                            {
                                                int index = _random.Next(Values.Count);
                                                value = Values[index];
                                                if (value.Active)
                                                {
                                                    valid = true;
                                                }
                                            } while (!valid);
                                            ideaElement.IdeaValues.Add(new IdeaValue(value.Name));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            _ideaListViewModel = new IdeaListViewModel(rolledIdeas);
            CreateGroupedView();
            SelectedDice.Dice.LastUsed = DateTime.Now;
            await _diceListViewModel.SaveDiceAsync();
            //await SelectedDice.AddCategoryAsync();
        }
        private void EditExecute()
        {
            //Task.Run(SelectedDice.EditCategoryAsync);
        }

        public IRegionManager RegionManager { get; private set; }
        public ListCollectionView GroupedIdeasView
        {
            get => _groupedIdeasView;
            set => SetProperty(ref _groupedIdeasView, value);
        }

        public bool IsEditEnabled => GroupedIdeasView.CurrentItem != null;

        public IdeaViewModel SelectedIdea
        {
            get
            {
                if (GroupedIdeasView != null) return GroupedIdeasView.CurrentItem as IdeaViewModel;
                return null;
            }
            set => GroupedIdeasView.MoveCurrentTo(value);
        }
        public DiceViewModel SelectedDice
        {
            get => _selectedDice;
            
            set => SetProperty(ref _selectedDice, value);
        }

        private async void DeleteExecute()
        {
            var selectedIdea = SelectedIdea;
            if (selectedIdea == null) return;
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
            await _ideaListViewModel.DeleteIdeaAsync(selectedIdea);
            //}
        }

        private async void NewExecute()
        {
            var newIdea = await _ideaListViewModel.AddIdeaAsync();
            GroupedIdeasView.Refresh();
            GroupedIdeasView.MoveCurrentTo(newIdea);

            newIdea.Idea.WhenPropertyChanged.Subscribe(OnNext);
        }

        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Dice.Name))
            {
                GroupedIdeasView.Refresh();
            }
        }

        private async void SaveExecute()
        {
            await _ideaListViewModel.SaveIdeasAsync();
        }

        private void CreateGroupedView()
        {
            ObservableCollection<IdeaViewModel> ideaViewModels = _ideaListViewModel.AllIdeas;
            foreach (var ideaViewModel in ideaViewModels)
            {
                ideaViewModel.Idea.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "Idea.Name";
            GroupedIdeasView = new ListCollectionView(ideaViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            GroupedIdeasView.GroupDescriptions.Add(new PropertyGroupDescription
            {
                PropertyName = propertyName,
                Converter = new NameToInitialConverter()
            });
            GroupedIdeasView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedIdea));
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Debug.WriteLine("Navigated to Detail Dice");
            if (navigationContext != null)
            {
                _parameters = navigationContext.Parameters;
                //navigationContext.Parameters;
                if (navigationContext.Parameters["diceListViewModel"] != null)
                {
                    _diceListViewModel = navigationContext.Parameters.GetValue<DiceListViewModel>("diceListViewModel");
                    _ideaDataService = navigationContext.Parameters.GetValue<IIdeaDataService>("ideaDataService");
                    _ideaListViewModel = new IdeaListViewModel(_ideaDataService);
                    CreateGroupedView();
                    if (navigationContext.Parameters["selectedDice"] != null)
                    {
                        Debug.WriteLine("Selected dice is not null");
                        Debug.WriteLine("Selected Dice: " + navigationContext.Parameters.GetValue<DiceViewModel>("selectedDice").Dice.Name);
                        SelectedDice = navigationContext.Parameters.GetValue<DiceViewModel>("selectedDice");
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
