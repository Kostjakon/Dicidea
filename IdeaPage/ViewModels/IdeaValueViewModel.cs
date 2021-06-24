using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Dicidea.Core.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace IdeaPage.ViewModels
{
    public class IdeaValueViewModel : BindableBase
    {
        private readonly IdeaElementViewModel _ideaElementViewModel;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        public IdeaValueViewModel(IdeaValue ideaValue, IdeaElementViewModel ideaElementViewModel)
        {
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

        // TODO: Edit to change value from the active one to anotherone from the dice
        public void EditExecute()
        {
            Debug.WriteLine("Edit Value");
            IsEditEnabled = !IsEditEnabled;
            IsEditDisabled = !IsEditDisabled;

        }
        public async void DeleteExecute()
        {
            _ideaElementViewModel.SelectedIdeaValue = this;
            await _ideaElementViewModel.DeleteIdeaValueAsync();
        }

        public bool IsSelected
        {
            get => _ideaElementViewModel.SelectedIdeaValue == this;
        }

        public IdeaElement IdeaElement
        {
            get => _ideaElementViewModel.IdeaElement;
        }

        public IdeaValue IdeaValue { get; }
    }
}
