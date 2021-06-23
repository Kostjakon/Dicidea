using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Dicidea.Core.Models
{
    public class Idea : BindableBase
    {
        private string _name;
        private string _description;
        public Idea(bool newIdea)
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public Idea(){}

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
        public virtual List<IdeaCategory> IdeaCategories { get; set; }
    }
}
