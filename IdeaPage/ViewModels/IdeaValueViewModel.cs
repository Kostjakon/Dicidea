using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace IdeaPage.ViewModels
{
    public class IdeaValueViewModel : BindableBase
    {
        private readonly IdeaElementViewModel _ideaElementViewModel;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        private readonly IDialogService _dialogService;
        private readonly IIdeaDataService _ideaDataService;
        public IdeaValueViewModel(IdeaValue ideaValue, IdeaElementViewModel ideaElementViewModel, IDialogService dialogService, IIdeaDataService ideaDataService)
        {
            _ideaDataService = ideaDataService;
            _dialogService = dialogService;
            _ideaElementViewModel = ideaElementViewModel;
            IdeaValue = ideaValue;
            EditCommand = new DelegateCommand(EditExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
        }

        public bool IsEditEnabled
        {
            get => _isEditEnabled;
            set => SetProperty(ref _isEditEnabled, value);
        }
        public bool IsEditDisabled
        {
            get => _isEditDisabled;
            set => SetProperty(ref _isEditDisabled, value);
        }

        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        
        public void EditExecute()
        {
            Debug.WriteLine("Edit Value");
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

        }
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

        public IdeaValue IdeaValue { get; }
    }
}
