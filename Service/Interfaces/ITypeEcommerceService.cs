using BusinessObjects.Models;
using DTOs.TypeEcommerce;
using DTOs.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ITypeEcommerceService
    {

        Task<IEnumerable<TypeEcommerce>> GetListTypeEcommerce(int page, int size);
        Task<TypeEcommerce> GetTypeEcommerceByID(int Id);
        Task UpdateTypeEcommerce(UpdateTypeEcommerceDTO typeEcommerce);
        Task CreateTypeEcommerce(CreateTypeEcommerceDTO typeEcommerce); 
    }
}
