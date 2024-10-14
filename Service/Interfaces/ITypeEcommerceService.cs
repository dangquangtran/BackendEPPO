using BusinessObjects.Models;
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
    }
}
