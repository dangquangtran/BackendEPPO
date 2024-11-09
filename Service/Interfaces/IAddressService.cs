using BusinessObjects.Models;
using DTOs.Address;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Service.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetLisAddress(int page, int size);
        Task<Address> GetAddressByID(int Id);

        Task CreateAddress(CreateAddressDTO address, int userID);
        Task UpdateAddress(UpdateAddressDTO address);
        Task DeleteAddress(DeleteAddressDTO address);
        Task<IEnumerable<Address>> GetLisAddressByUserID(int userID);
    }
}
