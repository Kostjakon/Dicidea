using System;
using System.Collections.Generic;
using Dicidea.Core.Helper;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Dicidea.Core.Models
{
    public sealed class Idea : NotifyDataErrorInfo<Idea>
    {
        private string _name;
        private string _description;
        private List<IdeaCategory> _ideaCategories;
        private DateTime _rolledDate;
        public Idea(string name, string description)
        {
            Name = name;
            Description = description;
            IdeaCategories = new List<IdeaCategory>();
            Id = Guid.NewGuid().ToString("N");
            Rules.Add(new DelegateRule<Idea>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
            RolledDate = DateTime.Now;
        }

        public Idea()
        {
            Rules.Add(new DelegateRule<Idea>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
        }

        [JsonProperty(PropertyName = "IdeaId", Required = Required.Always)]
        public string Id
        {
            get; 
            set;
        }

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

        public DateTime RolledDate
        {
            get => _rolledDate;
            set => SetProperty(ref _rolledDate, value);
        }

        public List<IdeaCategory> IdeaCategories
        {
            get => _ideaCategories; 
            set => SetProperty(ref _ideaCategories, value);
        }
    }
}
