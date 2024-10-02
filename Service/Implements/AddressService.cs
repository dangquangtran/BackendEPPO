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
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Address>> GetLisAddress(int page, int size)
        {
            return await _unitOfWork.AddressRepository.GetAsync(pageIndex: page, pageSize: size);
        }
        public async Task<Address> GetAddressByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.AddressRepository.GetByID(Id));
        }
    }
}
