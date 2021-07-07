using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Dicidea.Core.Models;
using Prism.Services.Dialogs;

namespace Dicidea.ViewModels
{
    /// <summary>
    ///     ViewModel für den SavedDiceDialog.
    /// </summary>
    public class SavedDiceDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand<string> _okDialogCommand;
        public DelegateCommand<string> OkDialogCommand =>
            _okDialogCommand ?? (_okDialogCommand = new DelegateCommand<string>(OkDialog));
        

        private List<Dice> _dice;
        public List<Dice> Dice
        {
            get { return _dice; }
            set { SetProperty(ref _dice, value); }
        }

        private string _title = "Title";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        protected virtual void OkDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
            {
                result = ButtonResult.OK;
            }
            else if (parameter?.ToLower() == "false")
            {
                result = ButtonResult.Cancel;
            }
            Debug.WriteLine("Buttonresult: " + result);
            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {

        }

        /// <summary>
        /// Legt beim öffnen des Dialogs eine Liste der übergebenen Würfel an und setzt den Titel des Dialogs
        /// </summary>
        /// <param name="parameters">Liste mit den Paramtern für den Titel und der Liste der gespeicherten Würfel</param>
        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            Dice = parameters.GetValue<List<Dice>>("allDice");
            Title = parameters.GetValue<string>("title");
        }
    }
}
