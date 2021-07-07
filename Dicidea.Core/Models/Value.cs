using System;
using Dicidea.Core.Helper;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    /// <summary>
    /// Model Klasse für einen Wert in einem Würfel
    /// </summary>
    public class Value : NotifyDataErrorInfo<Value>
    {
        private string _name;
        private string _pictureUri;
        private bool _active;
        public Value(bool newValue)
        {
            Rules.Add(new DelegateRule<Value>(nameof(Name), "The value has to have a name.", v => !string.IsNullOrWhiteSpace(v?.Name)));
            Id = Guid.NewGuid().ToString("N");
            Name = "";
            Active = newValue;
        }
        public Value() { }

        [JsonProperty(PropertyName = "ValueId", Required = Required.Always)]
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

        public string PictureUri
        {
            get => _pictureUri;
            set => SetProperty(ref _pictureUri, value);
        }

        public bool Active
        {
            get => _active;
            set => SetProperty(ref _active, value);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
