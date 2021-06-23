using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Dicidea.Core.Models
{
    public class IdeaCategory : BindableBase
    {
        public IdeaCategory(bool newIdea)
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public IdeaCategory() { }

        [JsonProperty(PropertyName = "IdeaCategoryId", Required = Required.Always)]
        public string Id
        {
            get;
            set;
        }

        private string _name;

        public string Name
        {
            get => _name; 
            set => SetProperty(ref _name, value);
        }
        public virtual List<IdeaElement> IdeaElements { get; set; }
    }
}
