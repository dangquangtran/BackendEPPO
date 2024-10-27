using BusinessObjects.Models;
using DTOs.Contracts;
using Repository.Interfaces;
using Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class ContractService: IContractService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContractService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Contract>> GetListContract(int page, int size)
        {
            return await _unitOfWork.ContractRepository.GetAsync(includeProperties: "User", pageIndex: page, pageSize: size);
        }

        public async Task<Contract> GetContractByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.ContractRepository.GetByID(Id));
        }

        public async Task UpdateContract(UpdateContractDTO contract)
        {
            var entity = await Task.FromResult(_unitOfWork.ContractRepository.GetByID(contract.ContractId));

            if (entity == null)
            {
                throw new Exception($"Contract with ID {contract.ContractId} not found.");
            }
            contract.UserId = contract.UserId;
            contract.ContractNumber = contract.ContractNumber;
            contract.Description = contract.Description;
            contract.CreationContractDate = contract.CreationContractDate;
            contract.EndContractDate = contract.EndContractDate;
            contract.TotalAmount = contract.TotalAmount;
            contract.CreatedAt = contract.CreatedAt;
            contract.UpdatedAt = contract.UpdatedAt;
            contract.TypeContract = contract.TypeContract;
            contract.ContractUrl = contract.ContractUrl;
            contract.IsActive = contract.IsActive;
            contract.Status = contract.Status;

            _unitOfWork.ContractRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task CreateContract(CreateContractDTO contract)
        {
            var entity = new Contract
            {
                UserId = contract.UserId,
                ContractNumber = contract.ContractNumber,
                Description = contract.Description,
                CreationContractDate = contract.CreationContractDate,
                EndContractDate = contract.EndContractDate,
                TotalAmount = contract.TotalAmount,
                CreatedAt = contract.CreatedAt,
                UpdatedAt = contract.UpdatedAt,
                TypeContract = contract.TypeContract,
                ContractUrl = contract.ContractUrl,
                IsActive = contract.IsActive,
                Status = 1,
            };
            _unitOfWork.ContractRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }
    }
    
}
