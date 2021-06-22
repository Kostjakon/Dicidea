using System.Collections.Generic;
using System.Threading.Tasks;
using Dicidea.Core.Models;

namespace Dicidea.Core.Services
{
    public interface IRollEmSpaceDataService
    {
        Task<List<RollEmSpace>> GetAllRollEmSpacesAsync();
        Task AddRollEmSpaceAsync(RollEmSpace rollEmSpace);
        Task DeleteRollEmSpaceAsync(RollEmSpace rollEmSpace);
        Task SaveRollEmSpaceAsync();
        Task AddDiceAsync(RollEmSpace rollEmSpace);
        Task DeleteDiceAsync(RollEmSpace rollEmSpace, Dice dice);
    }
}
