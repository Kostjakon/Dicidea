using System.Collections.Generic;
using System.Threading.Tasks;
using Dicidea.Core.Models;

namespace Dicidea.Core.Services
{
    public interface IDiceDataService
    {
        Task<List<Dice>> GetAllDiceAsync();
        Task AddDiceAsync(Dice dice);
        Task DeleteDiceAsync(Dice dice);
        Task SaveDiceAsync();
        Task<Dice> GetLastRolledDiceAsync();
        Task AddCategoryAsync(Dice dice, Category category);
        Task DeleteCategoryAsync(Dice dice, Category category);
        Task AddElementAsync(Dice dice, Category category, Element element);
        Task DeleteElementAsync(Dice dice, Category category, Element element);
        Task AddValueAsync(Dice dice, Category category, Element element, Value value);
        Task DeleteValueAsync(Dice dice, Category category, Element element, Value value);
    }
}
