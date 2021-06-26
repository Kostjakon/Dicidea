using System;
using System.Collections.Generic;
using System.ComponentModel;
using Dicidea.Core.Helper;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Dicidea.Core.Models
{
    public sealed class IdeaElement : NotifyDataErrorInfo<IdeaElement>
    {
        private string _name;
        private List<IdeaValue> _ideaValues;
        public IdeaElement(string name)
        {
            Rules.Add(new DelegateRule<IdeaElement>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
            Id = Guid.NewGuid().ToString("N");
            Name = name;
            IdeaValues = new List<IdeaValue>();
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

        public List<IdeaValue> IdeaValues
        {
            get => _ideaValues; 
            set => SetProperty(ref _ideaValues, value);
        }
    }
}
