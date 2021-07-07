using System;
using System.Collections.Generic;
using Dicidea.Core.Helper;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    /// <summary>
    /// Model Klasse einer Kategorie eines Würfels
    /// </summary>
    public sealed class Category : NotifyDataErrorInfo<Category>
    {
        private string _name;
        private string _description;
        private int _amount;
        private bool _active;

        public Category(bool newCategory)
        {
            Elements = new List<Element>
            {
                new Element(newCategory)
            };
            Rules.Add(new DelegateRule<Category>(nameof(Name), "The category has to have a name.", c => !string.IsNullOrWhiteSpace(c?.Name)));
            Id = Guid.NewGuid().ToString("N");
            Name = " ";
            Description = " ";
            Amount = 1;
            Active = true;
        }
        public Category()
        {
            Rules.Add(new DelegateRule<Category>(nameof(Name), "The category has to have a name.", c => !string.IsNullOrWhiteSpace(c?.Name)));
        }

        [JsonProperty(PropertyName = "CategoryId", Required = Required.Always)]
        public string Id { get; set; }

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }

        public List<Element> Elements { get; set; }
        public int Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public bool Active
        {
            get => _active;
            set => SetProperty(ref _active, value);
        }

    }
}
