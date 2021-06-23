using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dicidea.Core.Helper;
using Dicidea.Core.Models;
using Dicidea.Core.Services;

namespace DicePage.ViewModels
{
    public class CategoryListViewModel : NotifyPropertyChanges
    {
        private ObservableCollection<CategoryViewModel> _categories;
        private DiceViewModel _selectedDice;
        private readonly IDiceDataService _diceDataService;

        public CategoryListViewModel(DiceViewModel dice, IDiceDataService diceDataService)
        {
            _diceDataService = diceDataService;
            _selectedDice = dice;
            //Debug.WriteLine(dice.Dice.Name);
            //Task.Run(LoadCategories);
            LoadCategories();
        }

        public ObservableCollection<CategoryViewModel> Categories
        {
            get => _categories;
            private set => SetProperty(ref _categories, value);
        }

        public async Task DeleteCategoryAsync(CategoryViewModel category)
        {
            Categories.Remove(category);
            await _diceDataService.DeleteCategoryAsync(_selectedDice.Dice, category.Category);
        }

        public async Task<CategoryViewModel> AddCategoryAsync()
        {
            Debug.WriteLine("Add Category in CategoryListViewModel");
            var categoryModel = new Category(true);
            await _diceDataService.AddCategoryAsync(_selectedDice.Dice, categoryModel);

            Debug.WriteLine("Add Category in CategoryListViewModel - After Adding to diceDataService");
            var newCategory = new CategoryViewModel(_selectedDice, categoryModel, _diceDataService);
            Debug.WriteLine("Add Category in CategoryListViewModel - After Creating new CategoryViewModel");
            // DICEVIEWMODEL!
            Categories.Add(newCategory);
            Debug.WriteLine("Add Category in CategoryListViewModel - After Adding to Categories");

            return newCategory;
        }

        private void LoadCategories()
        {
            Categories = new ObservableCollection<CategoryViewModel>();
            List<Category> categories = _selectedDice.Dice.Categories;
            //Debug.WriteLine(categories != null);
            if(categories!= null) categories.ToList().ForEach(c => Categories.Add(new CategoryViewModel(_selectedDice, c, _diceDataService)));
        }
    }
}
