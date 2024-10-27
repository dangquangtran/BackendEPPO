using BusinessObjects.Models;
using DTOs.ContractDetails;
using DTOs.Contracts;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class ContractDetailServices: IContractDetailServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContractDetailServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ContractDetail>> GetListContractDetail(int page, int size)
        {
            return await _unitOfWork.ContractDetailRepository.GetAsync(includeProperties: "User, Plant", pageIndex: page, pageSize: size);
        }

        public async Task<ContractDetail> GetContractDetailByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.ContractDetailRepository.GetByID(Id));
        }
        public async Task UpdateContractDetail(UpdateContractDetailDTO contractDetail)
        {
            var entity = await Task.FromResult(_unitOfWork.ContractDetailRepository.GetByID(contractDetail.ContractDetailId));

            if (entity == null)
            {
                throw new Exception($"Contract Detail Detail with ID {contractDetail.ContractDetailId} not found.");
            }
            contractDetail.ContractId = contractDetail.ContractId;
            contractDetail.PlantId = contractDetail.PlantId;
            contractDetail.Quantity = contractDetail.Quantity;
            contractDetail.TotalPrice = contractDetail.TotalPrice;
            contractDetail.IsActive = contractDetail.IsActive;
            contractDetail.Status = contractDetail.Status;

            _unitOfWork.ContractDetailRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task CreateContractDetail(CreateContractDetailDTO contractDetail)
        {
            var entity = new ContractDetail
            {
                ContractId = contractDetail.ContractId,
                PlantId = contractDetail.PlantId,
                Quantity = contractDetail.Quantity,
                TotalPrice = contractDetail.TotalPrice,
                IsActive = contractDetail.IsActive,
                Status = 1,
            };
            _unitOfWork.ContractDetailRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
