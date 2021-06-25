using System;
using System.Collections.Generic;
using Dicidea.Core.Helper;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Dicidea.Core.Models
{
    public class IdeaCategory : NotifyDataErrorInfo<IdeaCategory>
    {
        public IdeaCategory(bool newIdea)
        {
            Rules.Add(new DelegateRule<IdeaCategory>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
            Id = Guid.NewGuid().ToString("N");
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

        private string _name;

        public string Name
        {
            get => _name; 
            set => SetProperty(ref _name, value);
        }
        public virtual List<IdeaElement> IdeaElements { get; set; }
    }
}
