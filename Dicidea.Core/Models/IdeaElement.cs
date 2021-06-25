using System;
using System.Collections.Generic;
using System.ComponentModel;
using Dicidea.Core.Helper;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Dicidea.Core.Models
{
    public class IdeaElement : NotifyDataErrorInfo<IdeaElement>
    {
        private string _name;
        public IdeaElement(bool newIdea)
        {
            Rules.Add(new DelegateRule<IdeaElement>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
            Id = Guid.NewGuid().ToString("N");
        }

        public IdeaElement()
        {
            Rules.Add(new DelegateRule<IdeaElement>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
        }

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
