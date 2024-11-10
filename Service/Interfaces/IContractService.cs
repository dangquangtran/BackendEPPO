using BusinessObjects.Models;
using DTOs.Contracts;
using DTOs.Wallet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IContractService
    {
        Task<IEnumerable<Contract>> GetListContract(int page, int size);
        Task<IEnumerable<Contract>> GetContractOfUser(int userID);
        Task<Contract> GetContractByID(int Id);
        Task UpdateContract(UpdateContractDTO contract);
        Task CreateContract(CreateContractDTO contract);
        Task<string> GenerateContractPdfAsync(CreateContractDTO contract);

        Task CreatePartnershipContract(CreateContractPartnershipDTO contract, int userID);
        Task<string> GenerateBusinessPartnershipContractPdfAsync(CreateContractPartnershipDTO contract , int userID);
    }
}
