using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dicidea.Core.Models
{
    public class IdeaFolder
    {
        public IdeaFolder()
        {
            Id = Guid.NewGuid().ToString("N");
        }
        [JsonProperty(PropertyName = "FolderId", Required = Required.Always)]
        public string Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual Dictionary<string, Idea> Ideas { get; set; }
        public virtual List<string> IdeaIds { get; set; }
    }
}
