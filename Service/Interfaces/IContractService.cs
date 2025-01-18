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
        Task<Contract> GetContractByID2(int Id, int code);
        Task<Contract> GetContractByOrderId(int orderId);
        Task UpdateContract(UpdateContractDTO contract);
        Task<int> CreateContract(CreateContractDTO contract, int userID);
        Task<int> CreateContract2(CreateContractDTO contract, int userID);
        Task<string> GenerateContractPdfAsync(CreateContractDTO contract , int userId);

        Task<int> CreatePartnershipContract(CreateContractPartnershipDTO contract, int userID);
        Task<string> GenerateBusinessPartnershipContractPdfAsync(CreateContractPartnershipDTO contract , int userID);

        Task<IEnumerable<Contract>> GetListContractStatus(int page, int size,int status);
        Task IsSignedPartnershipContract(IsSignedPartnershipContract contract, int contractId);
        Task<Contract?> GetActiveContractByUserId(int userId);
        Task<int> CreateContractAddendum(CreateContractDTO contract, int userId);
        Task<IEnumerable<Contract>> SearchContract(int pageIndex, int pageSize, string keyword);
        Task<string> GenerateContractPdfAsyncv2(CreateContractDTO contract, int userId);

        Task<IEnumerable<Contract>> GetListContractWithOrderID(int orderId);
    }
}
