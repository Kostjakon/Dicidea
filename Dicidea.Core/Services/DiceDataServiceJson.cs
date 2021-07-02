using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dicidea.Core.Models;
using Newtonsoft.Json;
using Prism.Services.Dialogs;

namespace Dicidea.Core.Services
{
    public class DiceDataServiceJson : IDiceDataService
    {
        private readonly List<Dice> _allDice;
        private readonly IDialogService _dialogService;
        public DiceDataServiceJson(IDialogService dialogService)
        {
            _dialogService = dialogService;
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
        }

        /*
        public virtual async Task<Dice> GetDiceByIdAsync(string id)
        {
            await Task.CompletedTask;
            return _allDice.Find(d => d.Id == id);
        }*/
        public virtual async Task<Dice> GetLastRolledDiceAsync()
        {
            await Task.CompletedTask;
            return _allDice.OrderByDescending(d => d.LastUsed).First();
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

            try
            {

                if (!File.Exists(FileName)) return LoadSampleDice();

                string data = File.ReadAllText(FileName);
                List<Dice> allDice = JsonConvert.DeserializeObject<List<Dice>>(data);
                return allDice;
            }
            catch (Exception e)
            {
                _dialogService.ShowDialog("ErrorDialog",
                    new DialogParameters
                    {
                        { "title", "Error" },
                        { "message", $"Loading of file '{FileName}' failed!\nError: '{e.Message}" }
                    },
                    r =>
                    {
                        if (r.Result == ButtonResult.None) return;
                        if (r.Result == ButtonResult.OK) return;
                        if (r.Result == ButtonResult.Cancel) { }
                    });
                throw;
                throw;
            }
        }

        private List<Dice> LoadSampleDice()
        {
            Debug.WriteLine("Load Sample Dice");
            List<Dice> tmp = new List<Dice>()
            {
                new Dice()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "Sims",
                    Description = "This is a dice to roll Sims",
                    LastUsed = DateTime.Now,
                    Active = true,
                    Amount = 1,
                    Categories = new List<Category>()
                    {
                        new Category(){
                            Id = Guid.NewGuid().ToString("N"),
                            Name = "Genetics",
                            Description = "Genetics of a sim",
                            Active = true,
                            Amount = 1,
                            Elements = new List<Element>()
                            {
                                new Element()
                                {
                                    Id = Guid.NewGuid().ToString("N"),
                                    Name = "Eyecolor",
                                    Active = true,
                                    Amount = 1,
                                    Values = new List<Value>()
                                    {
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Green"
                                        },
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Blue"
                                        },
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Brown"
                                        }
                                    }
                                },new Element()
                                {
                                    Id = Guid.NewGuid().ToString("N"),
                                    Name = "Haircolor",
                                    Active = true,
                                    Amount = 1,
                                    Values = new List<Value>()
                                    {
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Blonde"
                                        },
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Red"
                                        },
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Brunette"
                                        },
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Black"
                                        }
                                    }
                                },
                            }
                        },new Category(){
                            Id = Guid.NewGuid().ToString("N"),
                            Name = "Style",
                            Description = "Style of a sim",
                            Active = true,
                            Amount = 1,
                            Elements = new List<Element>()
                            {
                                new Element()
                                {
                                    Id = Guid.NewGuid().ToString("N"),
                                    Name = "Hairstyle",
                                    Active = true,
                                    Amount = 1,
                                    Values = new List<Value>()
                                    {
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Short"
                                        },
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Middle"
                                        },
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Long"
                                        }
                                    }
                                },new Element()
                                {
                                    Id = Guid.NewGuid().ToString("N"),
                                    Name = "Clothing Style",
                                    Active = true,
                                    Amount = 1,
                                    Values = new List<Value>()
                                    {
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Elegant"
                                        },
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Casual"
                                        },
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Sporty"
                                        },
                                        new Value()
                                        {
                                            Id = Guid.NewGuid().ToString("N"),
                                            Name = "Daring"
                                        }
                                    }
                                },
                            }
                        },
                    }
                }
            };

            return tmp;
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
                string data = JsonConvert.SerializeObject(dice);
                File.WriteAllText(FileName, data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _dialogService.ShowDialog("ErrorDialog",
                    new DialogParameters
                    {
                        { "title", "Error" },
                        { "message", $"Saving in path '{FolderName}' failed!\nError: '{e.Message}" }
                    },
                    r =>
                    {
                        if (r.Result == ButtonResult.None) return;
                        if (r.Result == ButtonResult.OK) return;
                        if (r.Result == ButtonResult.Cancel) { }
                    });
                throw;
            }
        }
    }
}
