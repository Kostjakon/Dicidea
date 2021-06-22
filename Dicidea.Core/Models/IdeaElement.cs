using System.Collections.Generic;

namespace Dicidea.Core.Models
{
    public class IdeaElement
    {
        public string Name { get; set; }
        public virtual List<IdeaValue> IdeaValues { get; set; }
    }
}
