﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;

namespace DicePage.ViewModels
{
    public class DiceListViewModel : NotifyPropertyChanges
    {
        private readonly IDiceDataService _diceDataService;
        private ObservableCollection<DiceViewModel> _allDice;

        public DiceListViewModel(IDiceDataService diceDataService)
        {
            _diceDataService = diceDataService;
            Task.Run(LoadDiceAsync).Wait();
        }

        public ObservableCollection<DiceViewModel> AllDice
        {
            get => _allDice;
            private set => SetProperty(ref _allDice, value);
        }

        public async Task DeleteDiceAsync(DiceViewModel dice)
        {
            AllDice.Remove(dice);
            await _diceDataService.DeleteDiceAsync(dice.Dice);
        }

        public async Task<DiceViewModel> AddDiceAsync()
        {
            var diceModel = new Dice(true);
            await _diceDataService.AddDiceAsync(diceModel);

            var newDice = new DiceViewModel(diceModel, _diceDataService);
            AllDice.Add(newDice);
            return newDice;
        }

        public async Task SaveDiceAsync()
        {
            await _diceDataService.SaveDiceAsync();
        }

        private async Task LoadDiceAsync()
        {
            AllDice = new ObservableCollection<DiceViewModel>();
            List<Dice> dice = await _diceDataService.GetAllDiceAsync();
            dice.ToList().ForEach(d => AllDice.Add(new DiceViewModel(d, _diceDataService)));
        }
    }
}