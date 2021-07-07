using System.Diagnostics;
using Dicidea.Core.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    /// <summary>
    /// Kapselt den Wert eines Ideen Elements, fügt UI-spezifische Eigenschaften hinzu (Editieren und löschen)
    /// </summary>
    public class IdeaValueViewModel : BindableBase
    {
        private readonly IdeaElementViewModel _ideaElementViewModel;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        private readonly IDialogService _dialogService;
        /// <summary>
        /// Setzt das EditCommand und DeleteCommand.
        /// </summary>
        /// <param name="ideaValue">Wert für den das IdeaValueViewModel erstellt werden soll</param>
        /// <param name="ideaElementViewModel">Idee Element zu dem der Wert gehört, wird benötigt zum löschen eines Werts</param>
        /// <param name="dialogService">Wird zum Erzeugen eines Dialogs benötigt</param>
        public IdeaValueViewModel(IdeaValue ideaValue, IdeaElementViewModel ideaElementViewModel, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ideaElementViewModel = ideaElementViewModel;
            IdeaValue = ideaValue;
            EditCommand = new DelegateCommand(EditExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
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
        /// Zum Aktivieren und Deaktivieren des Editierens
        /// </summary>
        public void EditExecute()
        {
            Debug.WriteLine("Edit Value");
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;
        }
        /// <summary>
        /// Zum Löschen des Werts. Wird auf den Button geklickt wird zuerst ein Dialogangezeigt
        /// der mit Ja bestätigt werden muss, erst dann wird der Wert gelöscht.
        /// </summary>
        public async void DeleteExecute()
        {
            var selectedIdeaValue = this;
            bool delete = false;
            _dialogService.ShowDialog("ConfirmationDialog",
                new DialogParameters
                {
                    { "title", "Delete value?" },
                    { "message", $"Do you really want to delete the value '{selectedIdeaValue.IdeaValue.Name}'?" }
                },
                r =>
                {
                    if (r.Result == ButtonResult.None) return;
                    if (r.Result == ButtonResult.No) return;
                    if (r.Result == ButtonResult.Yes) delete = true;
                });
            if(!delete) return;
            _ideaElementViewModel.SelectedIdeaValue = selectedIdeaValue;
            await _ideaElementViewModel.DeleteIdeaValueAsync();
        }
        /// <summary>
        /// IdeaValue des IdeaValueViewModels
        /// </summary>
        public IdeaValue IdeaValue { get; }
    }
}
