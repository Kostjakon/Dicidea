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
        private int _amount;
        private bool _active;

        public Category(bool newCategory)
        {
            Rules.Add(new DelegateRule<Category>(nameof(Name), "The category has to have a name.", c => !string.IsNullOrWhiteSpace(c?.Name)));
            Id = Guid.NewGuid().ToString("N");
            Name = " ";
            Description = " ";
            Amount = 1;
            Active = true;
            Elements = new List<Element>
            {
                new Element(true)
            };
            Rules.Add(new DelegateRule<Category>(nameof(Elements), "Everything has to have a name", c =>
            {
                bool hasNoErrors = true;
                foreach (var element in c.Elements)
                {
                    if (element.HasErrors)
                    {
                        hasNoErrors = false;
                    }
                }

                return hasNoErrors;
            }));
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
