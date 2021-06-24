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
        Task DeleteIdeaAsync(Idea idea);
        Task SaveIdeasAsync();
        Task<Idea> GetIdeaByIdAsync(string id);
        Task AddIdeaCategoryAsync(Idea idea, IdeaCategory ideaCategory);
        Task DeleteIdeaCategoryAsync(Idea idea, IdeaCategory ideaCategory);
        Task AddIdeaElementAsync(Idea idea, IdeaCategory ideaCategory, IdeaElement ideaElement);
        Task DeleteIdeaElementAsync(Idea idea, IdeaCategory ideaCategory, IdeaElement ideaElement);
        Task AddIdeaValueAsync(Idea idea, IdeaCategory ideaCategory, IdeaElement ideaElement, IdeaValue ideaValue);
        Task DeleteIdeaValueAsync(Idea idea, IdeaCategory ideaCategory, IdeaElement ideaElement, IdeaValue ideaValue);
    }
}
