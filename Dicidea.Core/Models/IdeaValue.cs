using System;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace Dicidea.Core.Models
{
    public class IdeaValue : BindableBase
    {
        private string _name;
        public IdeaValue(bool newIdea)
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public IdeaValue() { }

        [JsonProperty(PropertyName = "IdeaValueId", Required = Required.Always)]
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
            get;
            set;
        }
    }
}
