using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;
using Dicidea.Core.Constants;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using IdeaPage.Views;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    /// <summary>
    /// ViewModel für den <see cref="IdeaDetail" />. Verwendet das <see cref="IdeaListViewModel" /> als
    /// Datenquelle, verwendet aber einen <see cref="ListCollectionView" /> für das Binden an die ListView.
    /// </summary>
    public class IdeaDetailViewModel : NotifyPropertyChanges, INavigationAware
    {
        private string _filterText;
        private SortOrder _sortOrder = SortOrder.Unsorted;
        private ListCollectionView _groupedIdeaView;
        private IdeaListViewModel _ideaListViewModel;
        private readonly IRegionManager _regionManager;
        private NavigationParameters _parameters;
        private readonly IDialogService _dialogService;
        private bool _showSaved;
        private bool _isSaving;
        /// <summary>
        /// Erzeugt die verschiedenen Commands und erhält und setzt den RegionManager und den DialogService
        /// </summary>
        /// <param name="regionManager">Benötigt zum navigieren</param>
        /// <param name="dialogService">Benötigt um Dialoge zu erzeugen</param>
        public IdeaDetailViewModel(IRegionManager regionManager, IDialogService dialogService)
        {

            _dialogService = dialogService;
            _regionManager = regionManager;
            SortCommand = new DelegateCommand(SortExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            SaveCommand = new DelegateCommand(SaveExecute);
        }
        /// <summary>
        /// Bool zum anzeigen und ausblenden der Saved Anzeige
        /// </summary>
        public bool ShowSaved
        {
            get => _showSaved;
            set => SetProperty(ref _showSaved, value);
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
        /// Die verschiedenen Sortiermöglichkeiten
        /// </summary>
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
        /// <summary>
        /// Zum Navigieren zur Ideen Übersicht. Es wird die Parameterliste mit übergeben.
        /// </summary>
        public DelegateCommand GoToIdeaOverview =>
            new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(IdeaOverview), _parameters);
                _regionManager.Regions[RegionNames.LeftBottomContentRegion].RemoveAll();
            });
        /// <summary>
        /// Der Text zum filtern. Noch nicht implementiert.
        /// </summary>
        public string FilterText
        {
            get => _filterText;
            set
            {
                SetProperty(ref _filterText, value);
                Filter();
            }
        }
        /// <summary>
        /// Die ausgewählte Idee
        /// </summary>
        public IdeaViewModel SelectedIdea
        {
            get
            {
                if (GroupedIdeaView != null) return GroupedIdeaView.CurrentItem as IdeaViewModel;
                return null;
            }
            set => GroupedIdeaView.MoveCurrentTo(value);
        }
        /// <summary>
        /// Die gruppierte Liste der Ideen.
        /// </summary>
        public ListCollectionView GroupedIdeaView
        {
            get => _groupedIdeaView;
            set => SetProperty(ref _groupedIdeaView, value);
        }
        /// <summary>
        /// Aktualisiert die Liste wenn der Name geändert wurde.
        /// </summary>
        /// <param name="propertyName">Name der geänderten Property</param>
        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(Idea.Name))
            {
                GroupedIdeaView.Refresh();
            }
        }

        /// <summary>
        /// Unsortierte Liste
        /// </summary>
        private void Unsort()
        {
            GroupedIdeaView.IsLiveSorting = false;
            GroupedIdeaView.CustomSort = null;
        }

        /// <summary>
        /// Aufsteigende Sortierung
        /// </summary>
        private void SortAscending()
        {
            GroupedIdeaView.IsLiveSorting = true;
            GroupedIdeaView.CustomSort = Comparer<IdeaViewModel>.Create((i1, i2) => string.Compare(i1.Idea.Name, i2.Idea.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Absteigende Sortierung
        /// </summary>
        private void SortDescending()
        {
            GroupedIdeaView.IsLiveSorting = true;
            GroupedIdeaView.CustomSort = Comparer<IdeaViewModel>.Create((i1, i2) => string.Compare(i2.Idea.Name, i1.Idea.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Funktion für das Sort Command. Noch nicht in der View implementiert.
        /// </summary>
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


        /// <summary>
        /// Zum Speichern der Ideen.
        /// </summary>
        private async void SaveExecute()
        {
            IsSaving = true;
            //await Task.Delay(3000);
            await _ideaListViewModel.SaveIdeasAsync();
            IsSaving = false;
            ShowSaved = true;
            await Task.Delay(3000);
            ShowSaved = false;
        }

        /// <summary>
        /// Zum Löschen einer Idee.
        /// Wird auf den Löschen Button geklickt wird ein Dialog aufgerufen um zu fragen ob die Idee gelöscht werden soll
        /// </summary>
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

            var propertyName = "Idea.SectionName";
            GroupedIdeaView = new ListCollectionView(ideaViewModels)
            {
                IsLiveSorting = true,
                IsLiveGrouping = true,
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            if (GroupedIdeaView.GroupDescriptions != null)
                GroupedIdeaView.GroupDescriptions.Add(new PropertyGroupDescription
                {
                    PropertyName = propertyName
                });
            GroupedIdeaView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedIdea));
        }
        /// <summary>
        /// Funktion zum Filtern der Würfel. Noch nicht implementiert!
        /// </summary>
        private void Filter()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
            {
                GroupedIdeaView.Filter = o => true;
            }
            else
            {
                GroupedIdeaView.IsLiveFiltering = true;
                GroupedIdeaView.Filter = o =>
                {
                    if (o is IdeaViewModel vm)
                        return vm.Idea.Name?.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0;
                    return true;
                };
            }
        }

        /// <summary>
        /// Wird aufgerufen wenn zu dieser Seite navigiert wird. Die übergebenen Parameter werden zwischengespeichert
        /// und eine gruppierte Liste der Ideen erzeugt. Außerdem wird die übergebene Idee in der Liste der Ideen ausgewählt.
        /// </summary>
        /// <param name="navigationContext">NavigationContext der die NavigationParameter beinhaltet.</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _parameters = navigationContext.Parameters;
            if (navigationContext.Parameters["ideaListViewModel"] != null)
            {
                _ideaListViewModel = navigationContext.Parameters.GetValue<IdeaListViewModel>("ideaListViewModel");
                CreateGroupedView();
                if (navigationContext.Parameters["selectedIdea"] != null)
                {
                    IdeaViewModel selectedIdea = navigationContext.Parameters["selectedIdea"] as IdeaViewModel;
                    GroupedIdeaView.MoveCurrentTo(selectedIdea);
                    GroupedIdeaView.Refresh();
                }
                else
                {
                    GroupedIdeaView.MoveCurrentToFirst();
                    if (GroupedIdeaView.Count > 0) _parameters.Add("selectedIdea", GroupedIdeaView.GetItemAt(0));
                }
            }
            SortCommand = new DelegateCommand(SortExecute);
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
