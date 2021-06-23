using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Dicidea.Core.Models
{
    public class IdeaElement : BindableBase
    {
        private string _name;
        public IdeaElement(bool newIdea)
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public IdeaElement() { }

        [JsonProperty(PropertyName = "IdeaElementId", Required = Required.Always)]
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
        public virtual List<IdeaValue> IdeaValues { get; set; }
    }
}
