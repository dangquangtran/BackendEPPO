using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IContractDetailServices
    {
        Task<IEnumerable<ContractDetail>> GetListContractDetail(int page, int size);
        Task<ContractDetail> GetContractDetailByID(int Id);
    }
}
