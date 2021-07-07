using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DicePage.ViewModels;
using DicePage.Views;
using Dicidea.Core.Constants;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using IdeaPage.ViewModels;
using Prism.Regions;
using Prism.Services.Dialogs;
using RollEmSpacePage.Views;

namespace RollEmSpacePage.ViewModels
{
    /// <summary>
    /// ViewModel für den <see cref="RollEmSpaceDetail" />. Verwendet das <see cref="IdeaListViewModel" /> als
    /// Datenquelle, verwendet aber einen <see cref="ListCollectionView" /> für das Binden an die ListView.
    /// Verwendet außerdem das <see cref="DiceViewModel" /> eines ausgewählten Würfels als Datenquelle für die
    /// Würfeloptionen
    /// </summary>
    public class RollEmSpaceDetailViewModel : NotifyPropertyChanges, INavigationAware
    {
        // temporäre Ideenliste für das würfeln
        private IdeaListViewModel _ideaListViewModel;
        // Haupt Ideenliste mit allen gespeicherten Ideen
        private IdeaListViewModel _mainIdeaListViewModel;
        private ListCollectionView _groupedIdeasView;
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;
        private DiceViewModel _selectedDice;
        private DiceListViewModel _diceListViewModel;
        private readonly IDialogService _dialogService;
        private readonly Random _random = new Random();
        private bool _showSaved;
        private bool _isSaving;
        private bool _showRolling;

        /// <summary>
        /// Erzeugt die verschiedenen Commands und erhält und setzt den RegionManager und den DialogService
        /// </summary>
        /// <param name="regionManager">Benötigt zum navigieren</param>
        /// <param name="dialogService">Benötigt um Dialoge zu erzeugen</param>
        public RollEmSpaceDetailViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            GoToDiceCommand = new DelegateCommand<object>(GoToDice);
            GoToRollEmSpaceOverviewCommand = new DelegateCommand<object>(GoToRollEmSpaceOverview);
            RollCommand = new DelegateCommand(RollExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            SaveCommand = new DelegateCommand(SaveExecute);
        }
        public ICommand GoToDiceCommand { get; private set; }
        public ICommand GoToRollEmSpaceOverviewCommand { get; private set; }
        /// <summary>
        /// Bool zum anzeigen und ausblenden der Saved Anzeige
        /// </summary>
        public bool ShowSaved
        {
            get => _showSaved;
            set => SetProperty(ref _showSaved, value);
        }
        /// <summary>
        /// Bool zum anzeigen und ausblenden der Rolling Anzeige
        /// </summary>
        public bool ShowRolling
        {
            get => _showRolling;
            set => SetProperty(ref _showRolling, value);
        }
        /// <summary>
        /// Bool zum anzeigen der Saving Anzeige
        /// </summary>
        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }
        /// <summary>
        /// Funktion zum navigieren zur Würfel Detail Seite des selectierten Würfels
        /// </summary>
        /// <param name="obj"></param>
        private void GoToDice(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.LeftBottomContentRegion, nameof(DiceDetail), _parameters);
        }
        /// <summary>
        /// Funktion zum navigieren zurück zur RollEm Übersicht
        /// </summary>
        /// <param name="obj"></param>
        private void GoToRollEmSpaceOverview(object obj)
        {
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(RollEmSpaceOverview), _parameters);
            _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
        }

        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand RollCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        /// <summary>
        /// Funktion zum checken ob der aktive Würfel Fehler hat. Sie gibt einen Bool Wert zurück.
        /// </summary>
        /// <returns>True: Würfel hat Error, False: Würfel hat keine Error</returns>
        private bool AreThereErrors()
        {
            if (!SelectedDice.Dice.HasErrors)
            {
                foreach (Category category in SelectedDice.Dice.Categories)
                {
                    if (!category.HasErrors)
                    {
                        foreach (Element element in category.Elements)
                        {
                            if (!element.HasErrors)
                            {
                                foreach (Value value in element.Values)
                                {
                                    if (value.HasErrors)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }
        /// <summary>
        /// Funktion zum Rollen von Würfeln. Hier wird gecheckt ob das Würfeln mit dem Würfel erlaubt ist.
        /// </summary>
        private async void RollExecute()
        {
            if (!AreThereErrors())
            {
                ShowRolling = true;
                await Task.Delay(10);
                RollIdeas();
                ShowRolling = false;
                SelectedDice.Dice.LastUsed = DateTime.Now;
                await _diceListViewModel.SaveRolledDiceAsync();
            }
            else
            {
                _dialogService.ShowDialog("ErrorDialog",
                    new DialogParameters
                    {
                        { "title", "Error" },
                        { "message", $"Your dice has to be error free to roll ideas!" }
                    },
                    r =>
                    {
                        if (r.Result == ButtonResult.None) return;
                        if (r.Result == ButtonResult.OK) return;
                        if (r.Result == ButtonResult.Cancel) { }
                    });
            }

        }

        /// <summary>
        /// Funktion die die Logik zum Würfeln beinhaltet. Hierbei wird eine neue Ideen Liste erzeugt und in das temporäre IdeaListViewModel gespeichert.
        /// </summary>
        private void RollIdeas()
        {
            List<Idea> rolledIdeas = new List<Idea>();
            for (int j = 0; j < SelectedDice.Dice.Amount; j++)
            {
                Idea idea = new Idea($"{j + 1}. {SelectedDice.Dice.Name} idea", SelectedDice.Dice.Name, SelectedDice.Dice.Description);
                rolledIdeas.Add(idea);
                if (SelectedDice.Dice.Categories != null)
                {
                    List<Category> categories = SelectedDice.Dice.Categories;
                    for (int i = 0; i < categories.Count; i++)
                    {
                        if (categories[i].Active)
                        {
                            for (int l = 0; l < categories[i].Amount; l++)
                            {
                                IdeaCategory ideaCategory = new IdeaCategory(categories[i].Name);
                                idea.IdeaCategories.Add(ideaCategory);
                                if (categories[i].Elements != null)
                                {
                                    List<Element> elements = categories[i].Elements;
                                    for (int a = 0; a < elements.Count; a++)
                                    {
                                        if (elements[a].Active)
                                        {

                                            for (int m = 0; m < elements[a].Amount; m++)
                                            {
                                                IdeaElement ideaElement = new IdeaElement(elements[a].Name);
                                                ideaCategory.IdeaElements.Add(ideaElement);
                                                if (elements[a].Values != null && elements[a].Values.Count > 0)
                                                {
                                                    List<Value> values = elements[a].Values;
                                                    for (int k = 0; k < elements[a].ValueAmount; k++)
                                                    {
                                                        bool valid = false;
                                                        Value value;
                                                        do
                                                        {
                                                            int index = _random.Next(values.Count);
                                                            value = values[index];
                                                            if (value.Active)
                                                            {
                                                                if (elements[a].OnlyUnique && elements[a].CanBeUnique)
                                                                {
                                                                    if (!ideaElement.AlreadyHasValue(value.Name))
                                                                    {
                                                                        valid = true;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    valid = true;
                                                                }
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
                    }
                }
                
            }
            _ideaListViewModel = new IdeaListViewModel(rolledIdeas, _dialogService);
            CreateGroupedView();
        }

        public IRegionManager RegionManager { get; private set; }
        /// <summary>
        ///     Der gruppierte <see cref="ListCollectionView" />, nach dem Anfangsbuchstaben des Ideenamens gruppiert.
        /// </summary>
        public ListCollectionView GroupedIdeasView
        {
            get => _groupedIdeasView;
            set => SetProperty(ref _groupedIdeasView, value);
        }
        /// <summary>
        /// Die ausgewählte Idee
        /// </summary>
        public IdeaViewModel SelectedIdea
        {
            get
            {
                if (GroupedIdeasView != null) return GroupedIdeasView.CurrentItem as IdeaViewModel;
                return null;
            }
            set => GroupedIdeasView.MoveCurrentTo(value);
        }
        /// <summary>
        /// Der aktive Würfel für die Würfelfunktionen auf der Seite
        /// </summary>
        public DiceViewModel SelectedDice
        {
            get => _selectedDice;
            
            set => SetProperty(ref _selectedDice, value);
        }
        /// <summary>
        /// Zum Löschen einer Idee.
        /// Wird auf den Löschen Button geklickt wird ein Dialog aufgerufen um zu fragen ob die Idee gelöscht werden soll
        /// </summary>
        private async void DeleteExecute()
        {
            bool delete = false;
            var selectedIdea = SelectedIdea;
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
            if(delete) await _ideaListViewModel.DeleteIdeaAsync(selectedIdea);
        }

        /// <summary>
        ///     Aktualisiert die Liste wenn der Name geändert wurde.
        /// </summary>
        /// <param name="propertyName">Name der geänderten Property</param>
        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Dice.Name))
            {
                GroupedIdeasView.Refresh();
            }
        }
        /// <summary>
        /// Zum Speichern der ausgewählten Ideen. Diese werden zuerst zur Haupt Ideen Liste hinzugefügt und diese dann gespeichert.
        /// </summary>
        private async void SaveExecute()
        {
            IsSaving = true;
            //await Task.Delay(3000);
            await _mainIdeaListViewModel.AddIdeasAsync(_ideaListViewModel.Ideas);
            await _mainIdeaListViewModel.SaveIdeasAsync();
            IsSaving = false;
            ShowSaved = true;
            await Task.Delay(3000);
            ShowSaved = false;
        }

        /// <summary>
        /// Funktion um das ideaListViewModel in einen ListCollectionView umzuwandeln. Diese wird zur gruppierten Darstellung der gewürfelten Ideen benötigt.
        /// </summary>
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
            if (GroupedIdeasView.GroupDescriptions != null)
                GroupedIdeasView.GroupDescriptions.Add(new PropertyGroupDescription
                {
                    PropertyName = propertyName,
                    Converter = new NameToInitialConverter()
                });
            GroupedIdeasView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedIdea));
        }

        /// <summary>
        /// Wird aufgerufen wenn zu dieser Seite navigiert wird. Die übergebenen Parameter werden zwischengespeichert, der zuvor doppegeklickte Würfel gesetzt,
        /// die übergebenen DiceListViewModel und IdeaListViewModel gesetzt und mit einer leeren Ideen Liste die gruppierte Liste erzeugt
        /// </summary>
        /// <param name="navigationContext">NavigationContext der die NavigationParameter beinhaltet.</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Debug.WriteLine("Navigated to Detail Dice");
            if (navigationContext != null)
            {
                _parameters = navigationContext.Parameters;
                if (navigationContext.Parameters["diceListViewModel"] != null)
                {
                    _diceListViewModel = navigationContext.Parameters.GetValue<DiceListViewModel>("diceListViewModel");
                    _ideaListViewModel = new IdeaListViewModel(new List<Idea>(), _dialogService);
                    _mainIdeaListViewModel = navigationContext.Parameters.GetValue<IdeaListViewModel>("ideaListViewModel");
                    CreateGroupedView();
                    if (navigationContext.Parameters["selectedDice"] != null)
                    {
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
        { }
    }
}
