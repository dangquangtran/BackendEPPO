using BusinessObjects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IContractService
    {
        Task<IEnumerable<Contract>> GetListContract();
    }
}
