using System;
using Dicidea.Core.Helper;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    public class Value : NotifyDataErrorInfo<Value>
    {
        private string _name;
        private string _pictureUri;
        public Value(bool newValue)
        {
            Rules.Add(new DelegateRule<Value>(nameof(Name), "The value has to have a name.", v => !string.IsNullOrWhiteSpace(v?.Name)));
            Id = Guid.NewGuid().ToString("N");
            Name = "";
        }
        public Value() { }

        [JsonProperty(PropertyName = "ValueId", Required = Required.Always)]
        public string Id
        {
            get;
            set;
        }

        /*
        public string Color
        {
            get;
            set;
        }*/

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

        [JsonIgnore]
        public bool Active
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
