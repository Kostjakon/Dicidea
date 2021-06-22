using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    public class Idea
    {
        public Idea()
        {
            Id = Guid.NewGuid().ToString("N");
        }
        [JsonProperty(PropertyName = "IdeaId", Required = Required.Always)]
        public string Id { get; }
        public virtual List<IdeaDice> IdeaDices { get; set; }
    }
}
