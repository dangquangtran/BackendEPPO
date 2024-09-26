using BusinessObjects.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PlantService : IPlantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Plant>> GetListPlants()
        {
            return await _unitOfWork.PlantRepository.GetAsync();
        }
    }
}
