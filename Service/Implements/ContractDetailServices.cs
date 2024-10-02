using BusinessObjects.Models;
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
    }
}
