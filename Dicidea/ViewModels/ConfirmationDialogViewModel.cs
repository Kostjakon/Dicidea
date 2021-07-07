using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using Prism.Services.Dialogs;

namespace Dicidea.ViewModels
{
    /// <summary>
    ///     ViewModel für den ConfirmationDialog.
    /// </summary>
    public class ConfirmationDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand<string> _closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<string>(CloseDialog));

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _title = "Title";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
            {
                result = ButtonResult.Yes;
            }
            else if (parameter?.ToLower() == "false")
            {
                result = ButtonResult.No;
            }
            Debug.WriteLine("Buttonresult: "+result);
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
        /// Setzt beim Öffnen des Dialogs den Titel und die Nachricht
        /// </summary>
        /// <param name="parameters">Liste mit den Paramtern für den Titel und die Nachricht</param>
        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
            Title = parameters.GetValue<string>("title");
        }
    }
}
