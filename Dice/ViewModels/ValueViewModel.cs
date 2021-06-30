using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace DicePage.ViewModels
{
    public class ValueViewModel : NotifyPropertyChanges
    {
        private readonly ElementViewModel _elementViewModel;
        private readonly IDialogService _dialogService;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        public ValueViewModel(Value value, ElementViewModel elementViewModel, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _elementViewModel = elementViewModel;
            Value = value;
            EditCommand = new DelegateCommand(EditExecute);
            DeleteCommand = new DelegateCommand(DeleteExecute);
            ActivateCommand = new DelegateCommand(ActivateExecute);
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
        public DelegateCommand ActivateCommand { get; set; }

        public void ActivateExecute()
        {
            Value.Active = !Value.Active;
        }

        public void EditExecute()
        {
            Debug.WriteLine("Edit Value");
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

        }
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

        public bool IsSelected
        {
            get => _elementViewModel.SelectedValue == this;
        }

        public Element Element
        {
            get => _elementViewModel.Element;
        }

        public Value Value { get; }
        
    }
}
