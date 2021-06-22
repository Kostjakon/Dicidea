using System.Collections.Generic;

namespace Dicidea.Core.Models
{
    public class IdeaCategory
    {
        public string Name { get; set; }
        public virtual List<IdeaElement> IdeaElements { get; set; }
    }
}
