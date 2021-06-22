using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    public class IdeaDice
    {
        public IdeaDice()
        {
            Id = Guid.NewGuid().ToString("N");
        }
        [JsonProperty(PropertyName = "IdeaDiceId", Required = Required.Always)]
        public string Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual List<IdeaCategory> IdeaCategorys { get; set; }
    }
}
