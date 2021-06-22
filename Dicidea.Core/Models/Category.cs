using System;
using System.Collections.Generic;
using Dicidea.Core.Helper;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    public sealed class Category : NotifyDataErrorInfo<Category>
    {
        private string _name;
        private string _description;

        public Category(bool newCategory)
        {
            Rules.Add(new DelegateRule<Category>(nameof(Name), "The dice has to have a name.", c => !string.IsNullOrWhiteSpace(c?.Name)));
            Id = Guid.NewGuid().ToString("N");
            Name = "New Category";
            Elements = new List<Element>
            {
                new Element(true)
            };
        }
        public Category()
        {
            Rules.Add(new DelegateRule<Category>(nameof(Name), "The dice has to have a name.", c => !string.IsNullOrWhiteSpace(c?.Name)));
        }

        [JsonProperty(PropertyName = "CategoryId", Required = Required.Always)]
        public string Id { get; set; }

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }

        public List<Element> Elements { get; set; }
        [JsonIgnore]
        public int Amount { get; set; }
        [JsonIgnore]
        public bool Active { get; set; }
        
    }
}
