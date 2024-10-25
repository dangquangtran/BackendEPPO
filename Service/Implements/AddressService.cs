using BusinessObjects.Models;
using DTOs.Address;
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
        public async Task CreateAddress(RequestAddress address)
        {
            var entity = new Address
            {
             UserId = address.UserId,
             CreationDate = DateTime.Now,
             Description = address.Description, 
             ModificationDate = DateTime.Now,  
             Status = 1,
            };

            _unitOfWork.AddressRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAddress(UpdateAddressDTO address)
        {

            var entity = await Task.FromResult(_unitOfWork.AddressRepository.GetByID(address.AddressId));

            if (entity == null)
            {
                throw new Exception($"Address with ID {address.AddressId} not found.");
            }
            address.UserId = address.UserId;
            address.Description = address.Description;
            address.ModificationDate = DateTime.Now;
            address.Status = address.Status;

            _unitOfWork.AddressRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
