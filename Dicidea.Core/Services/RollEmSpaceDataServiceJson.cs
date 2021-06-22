using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dicidea.Core.Models;
using Newtonsoft.Json;

namespace Dicidea.Core.Services
{
    public class RollEmSpaceDataServiceJson : IRollEmSpaceDataService
    {
        private readonly List<RollEmSpace> _allRollEmSpaces;
        private readonly IDiceDataService _diceDataService;

        public RollEmSpaceDataServiceJson(IDiceDataService diceDataService)
        {
            _allRollEmSpaces = LoadRollEmSpacesAsync().Result;
            _diceDataService = diceDataService;
        }

        private string FileName => Path.Combine(FolderName, "rollemspaces.json");

        private string FolderName =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dicidea");

        public virtual async Task<List<RollEmSpace>> GetAllRollEmSpacesAsync()
        {
            await Task.CompletedTask;
            return _allRollEmSpaces;
        }

        public virtual async Task AddRollEmSpaceAsync(RollEmSpace rollEmSpace)
        {
            await Task.CompletedTask;
            _allRollEmSpaces.Add(rollEmSpace);
        }

        public virtual async Task DeleteRollEmSpaceAsync(RollEmSpace rollEmSpace)
        {
            await Task.CompletedTask;
            _allRollEmSpaces.Remove(rollEmSpace);
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

        public virtual async Task SaveRollEmSpaceAsync()
        {
            await Task.CompletedTask;
            await SaveRollEmSpacesAsync(_allRollEmSpaces.Where(r => !r.HasErrors));
        }


        public virtual async Task AddDiceAsync(RollEmSpace rollEmSpace)
        {
            await Task.CompletedTask;
            Dice dice = new Dice(true);
            rollEmSpace.Dices.Add(dice);
            rollEmSpace.DiceIds.Add(dice.Id);
        }

        public virtual async Task DeleteDiceAsync(RollEmSpace rollEmSpace, Dice dice)
        {
            await Task.CompletedTask;
            rollEmSpace.Dices.Remove(dice);
            rollEmSpace.DiceIds.Remove(dice.Id);
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

        private async Task FillRollEmSpaceDiceLists()
        {
            foreach (RollEmSpace rollEmSpace in _allRollEmSpaces)
            {
                foreach (var diceId in rollEmSpace.DiceIds)
                {
                    rollEmSpace.Dices.Add( await _diceDataService.GetDiceByIdAsync(diceId));
                }
            }
        }

        private async Task<List<RollEmSpace>> LoadRollEmSpacesAsync()
        {
            await Task.Delay(0);

            try
            {

                if (!File.Exists(FileName))
                    return LoadSampleRollEmSpaces();

                string data = File.ReadAllText(FileName);
                List<RollEmSpace> allrollEmSpaces = JsonConvert.DeserializeObject<List<RollEmSpace>>(data);
                return allrollEmSpaces?.Count >= 1 ? allrollEmSpaces : LoadSampleRollEmSpaces();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine($"Fehler beim Laden: {e.Message}");
                throw;
            }
        }

        private List<RollEmSpace> LoadSampleRollEmSpaces()
        {
            RollEmSpace rollEmSpace = new RollEmSpace();
            rollEmSpace.Name = "Sims 4";
            rollEmSpace.Description = "A Workspace with Sims 4 Dice";
            rollEmSpace.Dices = new List<Dice>();
            rollEmSpace.DiceIds = new List<string>();
            Dice dice = new Dice(true);
            dice.Name = "Sims";
            dice.Description = "This is a dice to roll Sims";
            //dice.Categories = new List<Category>();
            Category category = new Category(true);
            category.Name = "Genetics";
            //category.Elements = new List<Element>();
            Element element = new Element(true);
            element.Name = "Eyecolor";
            //element.Values = new List<Value>();
            Value value1 = new Value(true);
            value1.Name = "Green";
            Value value2 = new Value(true);
            value2.Name = "Brown";
            Value value3 = new Value(true);
            value3.Name = "Blue";
            element.Values.Add(value1);
            element.Values.Add(value2);
            element.Values.Add(value3);
            category.Elements.Add(element);
            dice.Categories.Add(category);
            rollEmSpace.Dices.Add(dice);
            rollEmSpace.DiceIds.Add(dice.Id);
            List<RollEmSpace> tmp = new List<RollEmSpace>();
            tmp.Add(rollEmSpace);

            return tmp;
        }

        private async Task SaveRollEmSpacesAsync(IEnumerable<RollEmSpace> rollEmSpaces)
        {
            await Task.Delay(0);
            try
            {
                if (!Directory.Exists(FolderName))
                {
                    Directory.CreateDirectory(FolderName);
                }
                string data = JsonConvert.SerializeObject(_allRollEmSpaces);
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
