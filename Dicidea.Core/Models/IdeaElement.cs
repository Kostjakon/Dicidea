using System;
using System.Collections.Generic;
using Dicidea.Core.Helper;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    /// <summary>
    /// Modelklasse für ein Element einer Idee
    /// </summary>
    public sealed class IdeaElement : NotifyDataErrorInfo<IdeaElement>
    {
        private string _name;
        private List<IdeaValue> _ideaValues;
        public IdeaElement(string name)
        {
            IdeaValues = new List<IdeaValue>();
            Rules.Add(new DelegateRule<IdeaElement>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
            Id = Guid.NewGuid().ToString("N");
            Name = name;
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

        /// <summary>
        /// Funktion zum überprüfen ob die Liste der Werte bereits einen Wert mit diesem Namen hat
        /// </summary>
        /// <param name="name">Name des Werts nach dem überprüft werden soll</param>
        /// <returns></returns>
        public bool AlreadyHasValue(string name)
        {
            bool hasValue = false;
            foreach (IdeaValue ideaValue in IdeaValues)
            {
                if(ideaValue.Name == name) hasValue = true;
            }
            return hasValue;
        }
    }
}
