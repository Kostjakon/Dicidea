using System;
using Dicidea.Core.Helper;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    public class Value : NotifyPropertyChanges
    {
        private string _name;
        private string _pictureUri;
        public Value(bool newValue)
        {
            Id = Guid.NewGuid().ToString("N");
            Name = "New Value";
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
