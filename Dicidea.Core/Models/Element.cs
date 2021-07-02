using System;
using System.Collections.Generic;
using System.Diagnostics;
using Dicidea.Core.Helper;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    public sealed class Element : NotifyDataErrorInfo<Element>
    {
        private string _name;
        private int _amount;
        private bool _active;
        private int _valueAmount;

        public Element(bool newElement)
        {
            Values = new List<Value>
            {
                new Value(true)
            };
            Id = Guid.NewGuid().ToString("N"); 
            Name = "";
            Amount = 1;
            ValueAmount = 1;
            Active = true;
            Rules.Add(new DelegateRule<Element>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
            Rules.Add(new DelegateRule<Element>(nameof(Values), "Everything has to have a name", e =>
            {
                bool hasNoErrors = true;
                foreach (var value in e.Values)
                {
                    if (value.HasErrors)
                    {
                        hasNoErrors = false;
                    }
                }

                return hasNoErrors;
            }));
        }
        public Element()
        {
            Rules.Add(new DelegateRule<Element>(nameof(Name), "The element has to have a name.", e => !string.IsNullOrWhiteSpace(e?.Name)));
        }

        [JsonProperty(PropertyName = "ElementId", Required = Required.Always)]
        public string Id { get; set; }

        public string Name { get => _name; set => SetProperty(ref _name, value); }

        public List<Value> Values { get; set; }
        public int Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }
        public int ValueAmount
        {
            get => _valueAmount;
            set => SetProperty(ref _valueAmount, value);
        }

        public bool Active
        {
            get => _active;
            set => SetProperty(ref _active, value);
        }
    }
}
