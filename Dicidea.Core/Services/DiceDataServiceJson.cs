using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dicidea.Core.Models;
using Newtonsoft.Json;

namespace Dicidea.Core.Services
{
    public class DiceDataServiceJson : IDiceDataService
    {
        private readonly List<Dice> _allDice;

        public DiceDataServiceJson()
        {
            _allDice = LoadDiceAsync().Result;
            //SaveDiceAsync(_allDice);
        }

        private string FileName => Path.Combine(FolderName, "dice.json");

        private string FolderName =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dicidea");

        public virtual async Task<List<Dice>> GetAllDiceAsync()
        {
            await Task.CompletedTask;
            return _allDice;
        }

        public virtual async Task AddDiceAsync(Dice dice)
        {
            await Task.CompletedTask;
            _allDice.Add(dice);
        }

        public virtual async Task DeleteDiceAsync(Dice dice)
        {
            await Task.CompletedTask;
            _allDice.Remove(dice);

            // In ViewModel von RollEmSpaces muss es registriert werden wenn sich etwas im ViewModel von Dice ändert (Zum Beispiel ein Würfel wird gelöscht oder geändert)
            /*
            foreach (var rollEmSpace in RollEmSpaceDataService.GetAllRollEmSpaces())
            {
                if (rollEmSpace.Value.DiceIds.Contains(dice.Id))
                {
                    rollEmSpace.Value.DiceIds.Remove(dice.Id);
                    rollEmSpace.Value.Dices.Remove(dice.Id);
                }
            }*/
        }

        public virtual async Task<Dice> GetDiceByIdAsync(string id)
        {
            await Task.CompletedTask;
            return _allDice.Find(d => d.Id == id);
        }

        public virtual async Task SaveDiceAsync()
        {
            await Task.CompletedTask;
            await SaveDiceAsync(_allDice.Where(d => !d.HasErrors));
        }

        public virtual async Task AddCategoryAsync(Dice dice, Category category)
        {
            await Task.CompletedTask;
            dice.Categories.Add(category);
        }

        public virtual async Task DeleteCategoryAsync(Dice dice, Category category)
        {
            await Task.CompletedTask;
            dice.Categories.Remove(category);
        }

        public virtual async Task AddElementAsync(Dice dice, Category category, Element element)
        {
            await Task.CompletedTask;
            category.Elements.Add(element);
        }

        public virtual async Task DeleteElementAsync(Dice dice, Category category, Element element)
        {
            await Task.CompletedTask;
            category.Elements.Remove(element);
        }

        public virtual async Task AddValueAsync(Dice dice, Category category, Element element, Value value)
        {
            await Task.CompletedTask;
            element.Values.Add(value);
        }

        public virtual async Task DeleteValueAsync(Dice dice, Category category, Element element, Value value)
        {
            await Task.CompletedTask;
            element.Values.Remove(value);
        }

        private async Task<List<Dice>> LoadDiceAsync()
        {
            await Task.Delay(0);
            //Console.WriteLine("Load Dice");

            try
            {

                if (!File.Exists(FileName)) return LoadSampleDice();

                Debug.WriteLine("Dice are loaded from File");
                string data = File.ReadAllText(FileName);
                Debug.WriteLine("Deserialize loaded dice");
                List<Dice> loadedDice = JsonConvert.DeserializeObject<List<Dice>>(data);
                Debug.WriteLine("All dice to list");
                List<Dice> allDice = loadedDice.OrderBy(d => d.Name).ToList();
                return allDice?.Count > 0 ? allDice : LoadSampleDice();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine($"Fehler beim Laden: {e.Message}");
                throw;
            }
        }

        private List<Dice> LoadSampleDice()
        {
            Dice dice1 = new Dice(true);
            dice1.Name = "Sims";
            dice1.Description = "This is a dice to roll Sims";
            Category category1 = new Category(true);
            category1.Name = "Genetics";
            Element element1 = new Element(true);
            element1.Name = "Eyecolor";
            Value value1 = new Value(true);
            value1.Name = "Green";
            Value value2 = new Value(true);
            value2.Name = "Brown";
            Value value3 = new Value(true);
            value3.Name = "Blue";
            Element element2 = new Element(true);
            element2.Name = "Haircolor";
            Value value4 = new Value(true);
            value4.Name = "Blonde";
            Value value5 = new Value(true);
            value5.Name = "Brown";
            Value value6 = new Value(true);
            value6.Name = "Black";

            element1.Values.Add(value1);
            element1.Values.Add(value2);
            element1.Values.Add(value3);
            element2.Values.Add(value4);
            element2.Values.Add(value5);
            element2.Values.Add(value6);

            category1.Elements.Add(element1);
            category1.Elements.Add(element2);


            Category category2 = new Category(true);
            category2.Name = "Style";
            category2.Elements = new List<Element>();
            Element element3 = new Element(true);
            element3.Name = "Hairstyle";
            element3.Values = new List<Value>();
            Value value7 = new Value(true);
            value7.Name = "Short";
            Value value8 = new Value(true);
            value8.Name = "Long";
            Value value9 = new Value(true);
            value9.Name = "Middle";
            element3.Values.Add(value7);
            element3.Values.Add(value8);
            element3.Values.Add(value9);
            Element element4 = new Element(true);
            element4.Name = "Clothing Style";
            element4.Values = new List<Value>();
            Value value10 = new Value(true);
            value10.Name = "Elegant";
            Value value11 = new Value(true);
            value11.Name = "Casual";
            Value value12 = new Value(true);
            value12.Name = "Sporty";

            element4.Values.Add(value10);
            element4.Values.Add(value11);
            element4.Values.Add(value12);

            category2.Elements.Add(element3);
            category2.Elements.Add(element4);


            dice1.Categories.Add(category1);
            dice1.Categories.Add(category2);

            Dice dice2 = new Dice(true);

            List<Dice> tmp = new List<Dice>();
            dice2.Name = "Empty Dice";
            dice2.Description = "This is an empty dice for testing purposes. Also the description is extra long.";
            Category category3 = new Category(true);
            category3.Name = "Empty Category";
            Element element5 = new Element(true);
            element5.Name = "Empty Element";
            Value value13 = new Value(true);
            value7.Name = "Empty Value";

            element5.Values.Add(value13);
            category3.Elements.Add(element5);
            dice2.Categories.Add(category3);


            tmp.Add(dice1);
            tmp.Add(dice2);

            List<Dice> sortedTmp = tmp.OrderBy(d => d.Name).ToList();

            Console.WriteLine("Load Sample Data");

            return sortedTmp;
        }

        private async Task SaveDiceAsync(IEnumerable<Dice> dice)
        {
            await Task.Delay(0);
            try
            {
                if (!Directory.Exists(FolderName))
                {
                    Directory.CreateDirectory(FolderName);
                }
                string data = JsonConvert.SerializeObject(_allDice);
                File.WriteAllText(FileName, data);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine($"Fehler beim Speichern: {e.Message}");
                throw;
            }
        }
    }
}
