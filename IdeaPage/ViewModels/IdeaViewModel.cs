using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    /// <summary>
    /// Kapselt die Idee, fügt UI-spezifische Eigenschaften hinzu (Editieren von Titel, Section und Beschreibung der Idee)
    /// und erstellt eine ListCollectionView der Kategorien der Idee.
    /// </summary>
    public class IdeaViewModel : NotifyPropertyChanges
    {
        private readonly IdeaCategoryListViewModel _ideaCategoryListViewModel;
        private ListCollectionView _groupedIdeaCategoriesView;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        /// <summary>
        /// Erzeugt die gruppierte Liste der Kategorien der Idee und setzt das EditCommand.
        /// </summary>
        /// <param name="idea">Idee für die das IdeaViewModel erstellt werden soll</param>
        /// <param name="ideaDataService">Wird zur Weitergabe an das IdeaCategoryListViewModel benötigt</param>
        /// <param name="dialogService">Wird zur Weitergabe an das IdeaCategoryListViewModel benötigt</param>
        public IdeaViewModel(Idea idea, IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            Idea = idea;

            IdeaViewModel self = this;
            if (_ideaCategoryListViewModel == null)
            {
                _ideaCategoryListViewModel = new IdeaCategoryListViewModel(self, ideaDataService, dialogService);
            }

            CreateGroupedView();
            EditCommand = new DelegateCommand(EditExecute);
        }
        /// <summary>
        /// Bool zum anzeigen und ausblenden der Textboxen
        /// </summary>
        public bool IsEditEnabled
        {
            get => _isEditEnabled;
            set => SetProperty(ref _isEditEnabled, value);
        }
        /// <summary>
        /// Bool zum anzeigen und ausblenden der Textblöcke
        /// </summary>
        public bool IsEditDisabled
        {
            get => _isEditDisabled;
            set => SetProperty(ref _isEditDisabled, value);
        }
        /// <summary>
        /// Der gruppierte <see cref="ListCollectionView" />, nach dem Anfangsbuchstaben des Sektionnamens gruppiert.
        /// </summary>
        public ListCollectionView GroupedIdeaCategoriesView
        {
            get => _groupedIdeaCategoriesView;
            set => SetProperty(ref _groupedIdeaCategoriesView, value);
        }
        public DelegateCommand EditCommand { get; set; }
        
        /// <summary>
        /// Zum Aktivieren und Deaktivieren des Editierens
        /// </summary>
        public void EditExecute()
        {
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

        }
        /// <summary>
        /// Ausgewählte Kategorie
        /// </summary>
        public IdeaCategoryViewModel SelectedIdeaCategory
        {
            get
            {
                if (GroupedIdeaCategoriesView != null)
                    return GroupedIdeaCategoriesView.CurrentItem as IdeaCategoryViewModel;
                else return null;
            }
            set => GroupedIdeaCategoriesView.MoveCurrentTo(value);
        }

        /// <summary>
        ///     Aktualisiert die Liste wenn der Name geändert wurde.
        /// </summary>
        /// <param name="propertyName">Name der geänderten Property</param>
        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(IdeaCategory.Name))
            {
                GroupedIdeaCategoriesView.Refresh();
            }
        }
        /// <summary>
        /// Funktion um das ideaCategoryListViewModel in einen ListCollectionView umzuwandeln. Dieser wird zur gruppierten Darstellung der Ideen Kategorien benötigt.
        /// </summary>
        private void CreateGroupedView()
        {
            ObservableCollection<IdeaCategoryViewModel> ideaCategoryViewModels =
                _ideaCategoryListViewModel.IdeaCategories;
            foreach (var ideaCategoryViewModel in ideaCategoryViewModels)
            {
                ideaCategoryViewModel.IdeaCategory.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "IdeaCategory.Name";
            GroupedIdeaCategoriesView = new ListCollectionView(ideaCategoryViewModels)
            {
                IsLiveSorting = true,
                SortDescriptions = {new SortDescription(propertyName, ListSortDirection.Ascending)}
            };

            GroupedIdeaCategoriesView.CurrentChanged +=
                (sender, args) => OnPropertyChanged(nameof(SelectedIdeaCategory));
        }
        /// <summary>
        /// Zum Löschen einer Kategorie einer Idee.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteIdeaCategoryAsync()
        {
            var selectedIdeaCategory = SelectedIdeaCategory;
            await _ideaCategoryListViewModel.DeleteIdeaCategoryAsync(selectedIdeaCategory);
            GroupedIdeaCategoriesView.Refresh();
        }

        /// <summary>
        /// Idea des IdeaViewModels
        /// </summary>
        public Idea Idea { get; }
    }
}
