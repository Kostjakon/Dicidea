using System.Collections.Generic;
using System.Threading.Tasks;
using Dicidea.Core.Models;

namespace Dicidea.Core.Services
{
    /// <summary>
    /// Interface für einen Data-Service der Ideen verwaltet
    /// </summary>
    public interface IIdeaDataService
    {
        Task<List<Idea>> GetAllIdeasAsync();
        Task AddIdeasAsync(List<Idea> ideas);
        Task DeleteIdeaAsync(Idea idea);
        Task SaveIdeasAsync();
        Task<Idea> GetLastRolledIdeaAsync();
        Task DeleteIdeaCategoryAsync(Idea idea, IdeaCategory ideaCategory);
        Task DeleteIdeaElementAsync(IdeaCategory ideaCategory, IdeaElement ideaElement);
        Task DeleteIdeaValueAsync(IdeaElement ideaElement, IdeaValue ideaValue);
    }
}
