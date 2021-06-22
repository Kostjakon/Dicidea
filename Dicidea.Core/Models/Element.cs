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

        public Element(bool newElement)
        {
            Id = Guid.NewGuid().ToString("N"); 
            Name = "New Element";
            Values = new List<Value>
            {
                new Value(true)
            };
        }
        public Element() { }

        [JsonProperty(PropertyName = "ElementId", Required = Required.Always)]
        public string Id { get; set; }

        public string Name { get => _name; set => SetProperty(ref _name, value); }

        public List<Value> Values { get; set; }
        [JsonIgnore]
        public int Amount { get; set; }
        [JsonIgnore]
        public bool Active { get; set; }
    }
}
