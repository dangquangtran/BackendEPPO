using BusinessObjects.Models;
using DTOs.Plant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPlantService
    {
        Task<IEnumerable<Plant>> GetListPlants(int page, int size);
        Task<Plant> GetPlantByID(int Id);
        Task<IEnumerable<Plant>> GetListPlantByCategory(int Id);
        IEnumerable<PlantVM> GetAllPlants();
        Plant GetPlantById(int id);
        void CreatePlant(CreatePlantDTO createPlant);
        void UpdatePlant(UpdatePlantDTO updatePlant);
    }
}
