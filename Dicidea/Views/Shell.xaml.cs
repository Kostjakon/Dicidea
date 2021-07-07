using System;
using System.Windows;

namespace Dicidea.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell
    {
        public Shell()
        {
            InitializeComponent();
            this.RefreshMaximizeRestoreButton();
        }

        
        /// <summary>
        /// Funktion zum Minimieren des Fensters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Funktion zum Maximieren des Fensters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
            this.RefreshMaximizeRestoreButton();
        }

        /// <summary>
        /// Funktion zum Schließen des Fensters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Funktion zum aktualisieren des Icons des Minimieren/Maximieren Buttons
        /// </summary>
        private void RefreshMaximizeRestoreButton()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.MaxButton.Visibility = Visibility.Collapsed;
                this.RestoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.MaxButton.Visibility = Visibility.Visible;
                this.RestoreButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Function die bei jeder Statusänderung des Fensters aufgerufen wird
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowStateChanged(object sender, EventArgs e)
        {
            this.RefreshMaximizeRestoreButton();
        }
    }
}
