using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace DicePage.ViewModels
{
    /// <summary>
    /// Kapselt den Wert eines Elements, fügt UI-spezifische Eigenschaften hinzu (Editieren und löschen)
    /// </summary>
    public class ValueViewModel : NotifyPropertyChanges
    {
        private readonly ElementViewModel _elementViewModel;
        private readonly IDialogService _dialogService;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        /// <summary>
        /// Setzt den dialogService, das elementviewModel, den Wert, das EditCommand, das ActivateCommand und das DeleteCommand.
        /// </summary>
        /// <param name="value">Wert für den das ValueViewModel erstellt werden soll</param>
        /// <param name="elementViewModel">ElementViewModel zu dem der Wert gehört, wird benötigt zum löschen eines Werts</param>
        /// <param name="dialogService">Wird zum Erzeugen eines Dialogs benötigt</param>
        public ValueViewModel(Value value, ElementViewModel elementViewModel, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _elementViewModel = elementViewModel;
            Value = value;
            EditCommand = new DelegateCommand(EditExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            ActivateCommand = new DelegateCommand(ActivateExecute);
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
        public DelegateCommand ActivateCommand { get; set; }
        /// <summary>
        /// Zum Updaten des CanBeUnique Werts des Elements beim aktivieren
        /// und deaktivieren der Active Checkbox des Werts
        /// </summary>
        public void ActivateExecute()
        {
            _elementViewModel.Element.UpdateCanBeUnique();
        }
        /// <summary>
        /// Zum Aktivieren und Deaktivieren des Editierens
        /// </summary>
        public void EditExecute()
        {
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

        }
        /// <summary>
        /// Zum Löschen des Werts. Wird auf den Button geklickt wird zuerst ein Dialogangezeigt
        /// der mit Ja bestätigt werden muss, erst dann wird der Wert gelöscht.
        /// </summary>
        public async void DeleteExecute()
        {
            var selectedValue = this;
            bool delete = false;
            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete value?" },
                    { "message", $"Do you really want to delete the value '{selectedValue.Value.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if(!delete) return;

            _elementViewModel.SelectedValue = selectedValue;
            await _elementViewModel.DeleteValueAsync();
        }
        /// <summary>
        /// Bool der angibt ob der Wert ausgewählt ist.
        /// </summary>
        public bool IsSelected
        {
            get => _elementViewModel.SelectedValue == this;
        }
        /// <summary>
        /// Das Element zu dem der Wert gehört
        /// </summary>
        public Element Element
        {
            get => _elementViewModel.Element;
        }
        /// <summary>
        /// Der Wert der zum ValueViewModel gehört
        /// </summary>
        public Value Value { get; }
        
    }
}
