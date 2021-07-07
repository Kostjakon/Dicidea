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
    /// Kapselt das Element einer Ideen Kategorie, fügt UI-spezifische Eigenschaften hinzu (Löschen des Elements)
    /// und erstellt eine ListCollectionView der Werte der Idee.
    /// </summary>
    public class IdeaElementViewModel : NotifyPropertyChanges
    {
        private readonly IdeaCategoryViewModel _ideaCategoryViewModel;
        private readonly IdeaValueListViewModel _ideaValueListViewModel;
        private ListCollectionView _groupedIdeaValuesView;
        private readonly IDialogService _dialogService;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        /// <summary>
        /// Erzeugt die gruppierte Liste der Werte des Ideen Elements und setzt das EditCommand und DeleteCommand.
        /// </summary>
        /// <param name="ideaElement">Element für den das IdeaElementViewModel erstellt werden soll</param>
        /// <param name="ideaCategoryViewModel">Idee Kategorie zu der das Element gehört, wird benötigt zum löschen eines Elements</param>
        /// <param name="ideaDataService">Wird zur Weitergabe an das IdeaValueListViewModel benötigt</param>
        /// <param name="dialogService">Wird zum Erzeugen eines Dialogs benötigt</param>
        public IdeaElementViewModel(IdeaElement ideaElement, IdeaCategoryViewModel ideaCategoryViewModel, IIdeaDataService ideaDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ideaCategoryViewModel = ideaCategoryViewModel;
            FlipCommand = new DelegateCommand<object>(Flip, CanFlip);
            IdeaElement = ideaElement;
            EditCommand = new DelegateCommand(EditExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            IdeaElementViewModel self = this;
            if (_ideaValueListViewModel == null)
            {
                _ideaValueListViewModel = new IdeaValueListViewModel(self, ideaDataService, _dialogService);
            }
            CreateGroupedView();
        }
        /// <summary>
        /// Gruppierte Liste der Ideen Werte
        /// </summary>
        public ListCollectionView GroupedIdeaValuesView
        {
            get => _groupedIdeaValuesView;
            set => SetProperty(ref _groupedIdeaValuesView, value);
        }
        /// <summary>
        /// Ausgewählter Ideen Wert
        /// </summary>
        public IdeaValueViewModel SelectedIdeaValue
        {
            get
            {
                if (GroupedIdeaValuesView != null) return GroupedIdeaValuesView.CurrentItem as IdeaValueViewModel;
                return null;
            }
            set => GroupedIdeaValuesView.MoveCurrentTo(value);
        }
        /// <summary>
        /// Aktualisiert die Liste wenn der Name geändert wurde.
        /// </summary>
        /// <param name="propertyName">Name der geänderten Property</param>
        private void OnNext(string propertyName)
        {
            if (propertyName == nameof(IdeaValue.Name))
            {
                GroupedIdeaValuesView.Refresh();
            }
        }
        /// <summary>
        /// Funktion um das ideaValueListViewModel in einen ListCollectionView umzuwandeln. Diese wird zur gruppierten Darstellung der Ideen Werte benötigt.
        /// </summary>
        private void CreateGroupedView()
        {
            ObservableCollection<IdeaValueViewModel> ideaValueViewModels = _ideaValueListViewModel.IdeaValues;
            foreach (var ideaValueViewModel in ideaValueViewModels)
            {
                ideaValueViewModel.IdeaValue.WhenPropertyChanged.Subscribe(OnNext);
            }

            var propertyName = "IdeaValue.Name";
            GroupedIdeaValuesView = new ListCollectionView(ideaValueViewModels)
            {
                SortDescriptions = { new SortDescription(propertyName, ListSortDirection.Ascending) }
            };
            if (GroupedIdeaValuesView.GroupDescriptions != null)
                GroupedIdeaValuesView.GroupDescriptions.Add(new PropertyGroupDescription
                {
                    PropertyName = propertyName,
                    Converter = new NameToInitialConverter()
                });
            GroupedIdeaValuesView.CurrentChanged += (sender, args) => OnPropertyChanged(nameof(SelectedIdeaValue));
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
        public DelegateCommand DeleteCommand { get; set; }
        /// <summary>
        /// Zum editieren von Ideen Elementen
        /// </summary>
        public void EditExecute()
        {
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;
        }
        /// <summary>
        /// Zum Löschen von Ideen Elementen. Wird auf den Button geklickt wird zuerst ein Dialogangezeigt
        /// der mit Ja bestätigt werden muss, erst dann wird das Element gelöscht.
        /// </summary>
        public async void DeleteExecute()
        {
            var selectedIdeaElement = this;
            bool delete = false;
            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete element?" },
                    { "message", $"Do you really want to delete the element '{selectedIdeaElement.IdeaElement.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if(!delete) return;
            _ideaCategoryViewModel.SelectedIdeaElement = selectedIdeaElement;
            await _ideaCategoryViewModel.DeleteIdeaElementAsync(); 
        }
        /// <summary>
        /// IdeaElement des IdeaElementViewModels
        /// </summary>
        public IdeaElement IdeaElement { get; }
        public ICommand FlipCommand { get; set; }

        private bool CanFlip(object obj)
        {
            return true;
        }
        /// <summary>
        /// zum Löschen eines Wertes aus der Liste von Ideen Werten.
        /// Wird vom IdeaValueViewModel verwendet.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteIdeaValueAsync()
        {
            await _ideaValueListViewModel.DeleteIdeaValueAsync(SelectedIdeaValue);
            GroupedIdeaValuesView.Refresh();
        }
        /// <summary>
        /// Zum auswählen eines Elements das beim Flippen angezeigt werden soll.
        /// </summary>
        /// <param name="obj"></param>
        private void Flip(object obj)
        {
            _ideaCategoryViewModel.SelectedIdeaElement = this;
        }
    }
}
