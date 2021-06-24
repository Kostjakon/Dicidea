﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Prism.Commands;

namespace DicePage.ViewModels
{
    public class ValueViewModel : NotifyPropertyChanges
    {
        private readonly ElementViewModel _elementViewModel;
        private bool _isEditEnabled;
        private bool _isEditDisabled = true;
        public ValueViewModel(Value value, ElementViewModel elementViewModel)
        {
            _elementViewModel = elementViewModel;
            Value = value;
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
            _elementViewModel.SelectedValue = this;
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
