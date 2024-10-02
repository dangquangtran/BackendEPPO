using BusinessObjects.Models;
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
    }
}
