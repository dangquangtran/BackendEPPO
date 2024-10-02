using BusinessObjects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Service.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetLisAddress(int page, int size);
        Task<Address> GetAddressByID(int Id);
    }
}
