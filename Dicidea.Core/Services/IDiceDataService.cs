using System.Collections.Generic;
using System.Threading.Tasks;
using Dicidea.Core.Models;

namespace Dicidea.Core.Services
{
    /// <summary>
    /// Interface für einen Data-Service der Würfel verwaltet
    /// </summary>
    public interface IDiceDataService
    {
        Task<List<Dice>> GetAllDiceAsync();
        Task AddDiceAsync(Dice dice);
        Task DeleteDiceAsync(Dice dice);
        Task SaveDiceAsync();
        Task SaveRolledDiceAsync();
        Task<Dice> GetLastRolledDiceAsync();
        Task AddCategoryAsync(Dice dice, Category category);
        Task DeleteCategoryAsync(Dice dice, Category category);
        Task AddElementAsync(Category category, Element element);
        Task DeleteElementAsync(Category category, Element element);
        Task AddValueAsync(Element element, Value value);
        Task DeleteValueAsync(Element element, Value value);
    }
}
