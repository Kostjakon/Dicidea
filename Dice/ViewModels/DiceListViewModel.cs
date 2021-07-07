using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;
using Prism.Services.Dialogs;

namespace DicePage.ViewModels
{
    /// <summary>
    /// ViewModel für eine Liste von Würfeln. Die Würfel werden aus dem im Konstruktor übergebenen
    /// <see cref="DiceViewModel" /> gelesen und in eine ObservableCollection umgewandelt.
    /// </summary>
    public class DiceListViewModel : NotifyPropertyChanges
    {
        private readonly IDiceDataService _diceDataService;
        private ObservableCollection<DiceViewModel> _allDice;
        private readonly IDialogService _dialogService;
        /// <summary>
        /// Der Würfel DataService und der DialogService werden hier gesetzt, die Würfel aus dem
        /// Würfel DataService geladen und in eine ObservableCollection umgewandelt.
        /// </summary>
        /// <param name="diceDataService">Benötigt um die Würfel aus der Datenquelle zu laden</param>
        /// <param name="dialogService">Benötigt zur Weitergabe an das IdeaValueViewModel</param>
        public DiceListViewModel(IDiceDataService diceDataService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _diceDataService = diceDataService;
            LoadDiceAsync().Wait();
        }
        /// <summary>
        /// Observable Liste der DiceViewModels für alle Würfel die über den DataService geladen wurden
        /// </summary>
        public ObservableCollection<DiceViewModel> AllDice
        {
            get => _allDice;
            private set => SetProperty(ref _allDice, value);
        }
        /// <summary>
        /// Zum Löschen eines Würfels
        /// </summary>
        /// <param name="dice">Würfel der gelöscht werden soll</param>
        /// <returns></returns>
        public async Task DeleteDiceAsync(DiceViewModel dice)
        {
            AllDice.Remove(dice);
            await _diceDataService.DeleteDiceAsync(dice.Dice);
        }
        /// <summary>
        /// Zum hinzufügen eines neuen Würfels.
        /// </summary>
        /// <returns></returns>
        public async Task<DiceViewModel> AddDiceAsync()
        {
            var diceModel = new Dice(true);
            await _diceDataService.AddDiceAsync(diceModel);

            var newDice = new DiceViewModel(diceModel, _diceDataService, _dialogService);
            AllDice.Add(newDice);
            return newDice;
        }
        /// <summary>
        /// Zum Speichern der Würfel
        /// </summary>
        /// <returns></returns>
        public async Task SaveDiceAsync()
        {
            await _diceDataService.SaveDiceAsync();
        }
        /// <summary>
        /// Zum Speichern der Würfel ohne Dialog der gespeicherten Würfel
        /// </summary>
        /// <returns></returns>
        public async Task SaveRolledDiceAsync()
        {
            await _diceDataService.SaveRolledDiceAsync();
        }
        /// <summary>
        /// Zum Laden aller Würfel
        /// </summary>
        /// <returns></returns>
        private async Task LoadDiceAsync()
        {
            AllDice = new ObservableCollection<DiceViewModel>();
            List<Dice> dice = await _diceDataService.GetAllDiceAsync();
            dice.ToList().ForEach(d => AllDice.Add(new DiceViewModel(d, _diceDataService, _dialogService)));
        }
    }
}
