using BusinessObjects.Models;
using DTOs.Services;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ServicesDTO>> GetListServices()
        {
            return (IEnumerable<ServicesDTO>)await _unitOfWork.ServicesRepository.GetAsync();
        }
    }
}
