using System;
using System.Collections.Generic;
using Dicidea.Core.Helper;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Dicidea.Core.Models
{
    public sealed class IdeaCategory : NotifyDataErrorInfo<IdeaCategory>
    {
        private string _name;
        private List<IdeaElement> _ideaElements;
        public IdeaCategory(string name)
        {
            IdeaElements = new List<IdeaElement>();
            Rules.Add(new DelegateRule<IdeaCategory>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
            Id = Guid.NewGuid().ToString("N");
            Name = name;
        }

        public IdeaCategory()
        {
            Rules.Add(new DelegateRule<IdeaCategory>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
        }

        [JsonProperty(PropertyName = "IdeaCategoryId", Required = Required.Always)]
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

        public List<IdeaElement> IdeaElements
        {
            get => _ideaElements; 
            set => SetProperty(ref _ideaElements, value);
        }
    }
}
