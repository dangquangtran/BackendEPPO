using AutoMapper;
using BusinessObjects.Models;
using DTOs.Address;
using DTOs.Plant;
using DTOs.Room;
using PdfSharp;
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
        private readonly IMapper _mapper;
        public AddressService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Address>> GetLisAddress(int page, int size)
        {
            return await _unitOfWork.AddressRepository.GetAsync(filter: c => c.Status != 0, pageIndex: page, pageSize: size);
        }
        public async Task<IEnumerable<Address>> GetLisAddressByUserID(int userID)
        {
            var address = _unitOfWork.AddressRepository.Get(
                filter: c => c.UserId == userID && c.Status != 0
            );
            return _mapper.Map<IEnumerable<Address>>(address);
        }

        public async Task<Address> GetAddressByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.AddressRepository.GetByID(Id,  includeProperties: "User"));
        }
        public async Task CreateAddress(CreateAddressDTO address , int userID)
        {
            var entity = new Address
            {
                 UserId = userID,
                 CreationDate = DateTime.Now,
                 Description = address.Description, 
                 ModificationDate = DateTime.Now,  
                 Status = 1,
            };

            _unitOfWork.AddressRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAddress(UpdateAddressDTO address , int addressID , int userID)
        {

            var entity = await Task.FromResult(_unitOfWork.AddressRepository.GetByID(addressID));

            if (entity == null)
            {
                throw new Exception($"Address with ID {addressID} not found.");
            }
            entity.UserId = userID;
            if (!string.IsNullOrWhiteSpace(address.Description))
            {
                entity.Description = address.Description;
            }
            entity.Description = address.Description;
            entity.ModificationDate = DateTime.Now;
            entity.Status = address.Status;

            _unitOfWork.AddressRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }


        public async Task DeleteAddress(DeleteAddressDTO address)
        {

            var entity = await Task.FromResult(_unitOfWork.AddressRepository.GetByID(address.AddressId));

            if (entity == null)
            {
                throw new Exception($"Address with ID {address.AddressId} not found.");
            }
       
            entity.Status = 0;

            _unitOfWork.AddressRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
