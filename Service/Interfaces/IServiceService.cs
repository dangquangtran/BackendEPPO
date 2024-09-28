using BusinessObjects.Models;
using DTOs.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IServiceService
    {
        Task<IEnumerable<ServicesDTO>> GetListServices();
    }
}
