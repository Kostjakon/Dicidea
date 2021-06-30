using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dicidea.Core.Models;

namespace Dicidea.Core.Services
{
    public interface IIdeaDataService
    {
        Task<List<Idea>> GetAllIdeasAsync();
        Task AddIdeaAsync(Idea idea);
        Task AddIdeasAsync(List<Idea> ideas);
        Task DeleteIdeaAsync(Idea idea);
        Task SaveIdeasAsync();
        Task<Idea> GetLastRolledIdeaAsync();
        Task DeleteIdeaCategoryAsync(Idea idea, IdeaCategory ideaCategory);
        Task DeleteIdeaElementAsync(IdeaCategory ideaCategory, IdeaElement ideaElement);
        Task DeleteIdeaValueAsync(IdeaElement ideaElement, IdeaValue ideaValue);
    }
}
