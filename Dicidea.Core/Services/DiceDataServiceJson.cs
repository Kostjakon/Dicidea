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
            //Console.WriteLine("Load Dice");

            try
            {

                if (!File.Exists(FileName)) return LoadSampleDice();

                string data = File.ReadAllText(FileName);
                List<Dice> allDice = JsonConvert.DeserializeObject<List<Dice>>(data);
                return allDice;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine($"Fehler beim Laden: {e.Message}");
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
                    LastUsed = DateTime.Today,
                    Categories = new List<Category>()
                    {
                        new Category(){
                            Id = Guid.NewGuid().ToString("N"),
                            Name = "Genetics",
                            Description = "Genetics of a sim",
                            Elements = new List<Element>()
                            {
                                new Element()
                                {
                                    Id = Guid.NewGuid().ToString("N"),
                                    Name = "Eyecolor",
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
                            Elements = new List<Element>()
                            {
                                new Element()
                                {
                                    Id = Guid.NewGuid().ToString("N"),
                                    Name = "Hairstyle",
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
