using System;
using System.Collections.Generic;
using Dicidea.Core.Models;

namespace Dicidea.Core.SampleDice
{
    /// <summary>
    /// Beispiel Würfel
    /// </summary>
    public static class SimsDice
    {
        public static Dice GetSimsDice()
        {
            Dice dice = new Dice()
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Sims",
                Description = "This is a dice to roll Sims",
                LastUsed = DateTime.Now,
                Active = true,
                Amount = 1,
                Categories = new List<Category>()
                {
                    new Category()
                    {
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
                                ValueAmount = 1,
                                Values = new List<Value>()
                                {
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Black"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Dark Brown"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Brown"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Light Brown"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Hazel Green"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Light Hazel Green"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Light Green"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Green"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Hazel Blue"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Dark Hazel Blue"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Aqua"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Light Blue"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Blue"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Gray Blue"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Gray"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Gray Brown"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Amber"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Purple"
                                    }
                                }
                            },
                            new Element()
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                Name = "Haircolor",
                                Active = true,
                                Amount = 1,
                                ValueAmount = 1,
                                Values = new List<Value>()
                                {
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Neutral Black"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Black"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Dark Brown"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Warm Brown"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Brown"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Light Brown"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Red"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Auburn"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Orange"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Neutral Blonde"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Light Blonde"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Blonde"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Dirty Blonde"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Platinum"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "White"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "White Blonde"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Gray"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Purple Pastel"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Hot Pink"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Dark Blue"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Turquoise"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Green"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Salt Brown"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Salt Black"
                                    }
                                }
                            },
                            new Element()
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                Name = "Skincolor",
                                Active = true,
                                Amount = 1,
                                ValueAmount = 1,
                                Values = new List<Value>()
                                {
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Toffee"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Mocha"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Golden"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Almond"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Beige"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Vanilla"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Ebony"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Fair"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Ivory"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Sand"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Honey"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Bronze"
                                    }
                                }
                            },
                            new Element()
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                Name = "Skintone",
                                Active = true,
                                Amount = 1,
                                ValueAmount = 1,
                                Values = new List<Value>()
                                {
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Warm"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Neutral"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Cold"
                                    }
                                }
                            },
                            new Element()
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                Name = "Voice",
                                Active = true,
                                Amount = 1,
                                ValueAmount = 1,
                                Values = new List<Value>()
                                {
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Warm"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Clear"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Brass"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Sweet"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Melodic"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Lilted"
                                    }
                                }
                            },
                            new Element()
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                Name = "Walking Styles",
                                Active = true,
                                Amount = 1,
                                ValueAmount = 1,
                                Values = new List<Value>()
                                {
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Default"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Perky"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Snooty"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Swagger"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Feminine"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Tough"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Goofy"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Sluggish"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Bouncy"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Creepy"
                                    }
                                }
                            },
                            new Element()
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                Name = "Age",
                                Active = true,
                                Amount = 1,
                                ValueAmount = 1,
                                Values = new List<Value>()
                                {
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Toddler"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Child"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Teen"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Young Adult"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Adult"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Senior"
                                    }
                                }
                            },
                            new Element()
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                Name = "Gender",
                                Active = true,
                                Amount = 1,
                                ValueAmount = 1,
                                Values = new List<Value>()
                                {
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Male"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Female"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Trans Male"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Trans Female"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Other"
                                    }
                                }
                            },
                        }
                    },
                    new Category()
                    {
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
                                ValueAmount = 1,
                                Values = new List<Value>()
                                {
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Short"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Middle"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Long"
                                    }
                                }
                            },
                            new Element()
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                Name = "Clothing Style",
                                Active = true,
                                Amount = 1,
                                ValueAmount = 1,
                                Values = new List<Value>()
                                {
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Elegant"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Casual"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Sporty"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Daring"
                                    }
                                }
                            },
                        }
                    },
                    new Category()
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Name = "Character",
                        Description = "The character of a sim",
                        Active = true,
                        Amount = 1,
                        Elements = new List<Element>()
                        {
                            new Element()
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                Name = "Traits",
                                Active = true,
                                Amount = 1,
                                ValueAmount = 3,
                                Values = new List<Value>()
                                {
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Active"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Cheerful"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Creative"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Genius"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Gloomy"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Goofball"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Hot-Headed"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Romantic"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Self-Assured"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Unflirty"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Art Lover"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Bookworm"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Foodie"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Geek"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Maker"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Music Lover"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Perfectionist"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Adventurous"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Ambitious"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Cat Lover"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Child of the Ocean"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Child of the Islands"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Childish"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Clumsy"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Dance Machine"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Dog Lover"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Erratic"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Freegan"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Glutton"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Green Fiend"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Kleptomaniac"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Lazy"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Loves Outdoors"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Materialistic"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Neat"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Recycle Disciple"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Slob"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Snob"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Squeamish"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Vegetatian"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Bro"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Evil"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Family-oriented"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Good"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Hates Children"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Insider"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Jealous"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Loner"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Mean"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Noncommittal"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Outgoing"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Paranoid"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Proper"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Self-Absorbed"
                                    }
                                }
                            },
                            new Element()
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                Name = "Aspirations",
                                Active = true,
                                Amount = 1,
                                ValueAmount = 1,
                                Values = new List<Value>()
                                {
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Friend of the Animals"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Bodybuilder"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Extreme Sports Enthusiast"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "The Positivity Challenge"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Painter Extraordinaire"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Musical Genius"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Bestselling Author"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Master Actor/Actress"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Master Maker"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Lord/Lady of the Knits"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Public Enemy"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Chief of Mischief"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Successful Lineage"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Big Happy Family"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Vampire Family"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Super Parent"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Master Chef"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Master Mixologist"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Fabulously Wealthy"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Mansion Baron"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Renaissance Sim"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Nerd Brain"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Computer Whiz"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Master Vampire"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Archaeology Scholar"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Spellcraft & Sorcery"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Academic"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Serial Romantic"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Soulmate"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "City Native"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Stranger Ville Mystery"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Beach Life"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Mt. Komorebi Sightseer"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Freelance Botanist"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "The Curator"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Angling Ace"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Outdoor Enthusiast"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Jungle Explorer"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Purveyor of Potions"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Eco Innovator"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Joke Star"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Party Animal"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Friend of the World"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Leader of the Pack"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "Good Vampire"
                                    },
                                    new Value()
                                    {
                                        Id = Guid.NewGuid().ToString("N"),
                                        Active = true,
                                        Name = "World-Famous Celebrity"
                                    }
                                }
                            },
                        }
                    },
                }
            };
            return dice;
        }
    }
}
