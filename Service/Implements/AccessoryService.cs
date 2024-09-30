using BusinessObjects.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class AccessoryService : IAccessoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccessoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Accessory>> GetListAccessory()
        {
            return await _unitOfWork.AccessoryRepository.GetAsync();
        }
        public async Task<Accessory> GetAccessoryByID(int id)
        {
            return await Task.FromResult(_unitOfWork.AccessoryRepository.GetByID(id));
        }
    }
}
