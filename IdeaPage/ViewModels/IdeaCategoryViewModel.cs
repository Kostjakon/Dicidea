using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Dicidea.Core.Converters;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    /// <summary>
    /// Kapselt die Kategorie einer Idee, fügt UI-spezifische Eigenschaften hinzu (Löschen der Kategorie)
    /// und erstellt eine ListCollectionView der Kategorien der Idee.
    /// </summary>
    public class IdeaCategoryViewModel : NotifyPropertyChanges
    {
        private readonly IdeaElementListViewModel _ideaElementListViewModel;
        private readonly IdeaViewModel _ideaViewModel;
        private ListCollectionView _groupedIdeaElementsView;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        private readonly IDialogService _dialogService;
        /// <summary>
        /// Erzeugt die gruppierte Liste der Elemente der Ideen Kategorie und setzt das EditCommand und DeleteCommand.
        /// </summary>
        /// <param name="idea">Idee zu der die Kategorie gehört, wird benötigt zum löschen der Kategorie</param>
        /// <param name="ideaCategory">Kategorie für die das IdeaCategoryViewModel erstellt werden soll</param>
        /// <param name="ideaDataService">Wird zur Weitergabe an das IdeaValueListViewModel benötigt</param>
        /// <param name="dialogService">Wird zum Erzeugen eines Dialogs benötigt</param>
        public IdeaCategoryViewModel(IdeaViewModel idea, IdeaCategory ideaCategory, IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ideaViewModel = idea;
            IdeaCategory = ideaCategory;
            _ideaElementListViewModel ??= new IdeaElementListViewModel(this, ideaDataService, _dialogService);
            EditCommand = new DelegateCommand(EditExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            CreateGroupedView();
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
        
        public DelegateCommand EditCommand { get; set; }
        /// <summary>
        /// Zum editieren von Ideen Elementen
        /// </summary>
        public void EditExecute()
        {
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;
        }
        /// <summary>
        /// Gruppierte Liste der Ideen Elemente
        /// </summary>
        public ListCollectionView GroupedIdeaElementsView
        {
            get => _groupedIdeaElementsView;
            set => SetProperty(ref _groupedIdeaElementsView, value);
        }
        /// <summary>
        /// Das ausgewählte Element
        /// </summary>
        public IdeaElementViewModel SelectedIdeaElement
        {
            get
            {
                if (GroupedIdeaElementsView != null) return GroupedIdeaElementsView.CurrentItem as IdeaElementViewModel;
                else return null;
            }
            set => GroupedIdeaElementsView.MoveCurrentTo(value);
        }
        /// <summary>
        ///     Aktualisiert die Liste wenn der Name geändert wurde.
        /// </summary>
        /// <param name="propertyName">Name der geänderten Property</param>
        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(IdeaCategory.Name))
            {
                //GroupedElementsView.Refresh();
            }
        }
        /// <summary>
        /// Funktion um das ideaElementListViewModel in einen ListCollectionView umzuwandeln. Dieser wird zur gruppierten Darstellung der Ideen Elemente benötigt.
        /// </summary>
        private void CreateGroupedView()
        {
            ObservableCollection<IdeaElementViewModel> ideaElementViewModels = _ideaElementListViewModel.IdeaElements;
            foreach (var ideaElementViewModel in ideaElementViewModels)
            {
                ideaElementViewModel.IdeaElement.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "IdeaElement.Name";
            GroupedIdeaElementsView = new ListCollectionView(ideaElementViewModels)
            {
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

        /// <summary>
        /// Die zum ViewModel gehörige IdeaCategory
        /// </summary>
        public IdeaCategory IdeaCategory { get; }

        public ICommand DeleteCommand { get; set; }

        /// <summary>
        /// Zum Löschen von Ideen Kategorien. Wird auf den Button geklickt wird zuerst ein Dialogangezeigt
        /// der mit Ja bestätigt werden muss, erst dann wird die Kategorie gelöscht.
        /// </summary>
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
        /// <summary>
        /// Zum Löschen von Ideen Elementen aus der Kategorie. Wird über das IdeaElementViewModel verwendet.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteIdeaElementAsync()
        {
            await _ideaElementListViewModel.DeleteIdeaElementAsync(SelectedIdeaElement);
            GroupedIdeaElementsView.Refresh();
        }
    }
}
