using System;
using System.Collections.Generic;
using System.Diagnostics;
using Dicidea.Core.Helper;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    public sealed class Dice : NotifyDataErrorInfo<Dice>
    {
        private string _name;
        private string _description;

        public Dice(bool newDice)
        {
            Rules.Add(new DelegateRule<Dice>(nameof(Name), "The dice has to have a name.", d => !string.IsNullOrWhiteSpace(d?.Name)));
            Id = Guid.NewGuid().ToString("N");
            Debug.WriteLine("New dice");
            Name = "";
            Categories = new List<Category>
            {
                new Category(true)
            };
        }
        public Dice()
        {
            Rules.Add(new DelegateRule<Dice>(nameof(Name), "The dice has to have a name.", d => !string.IsNullOrWhiteSpace(d?.Name)));
            //Id ??= Guid.NewGuid().ToString("N");
            Debug.WriteLine("Loaded dice");
        }

        [JsonProperty(PropertyName = "DiceId", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Name
        {
            get => _name; 
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description; 
            set => SetProperty(ref _description, value);
        }

        public List<Category> Categories { get; set; }

        [JsonIgnore]
        public int Amount { get; set; }
        [JsonIgnore]
        public bool Active { get; set; }
    }
}
