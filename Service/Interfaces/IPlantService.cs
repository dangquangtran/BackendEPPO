using BusinessObjects.Models;
using DTOs.Plant;
using Microsoft.AspNetCore.Http;
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
        IEnumerable<PlantVM> GetAllPlants(int pageIndex, int pageSize);
        PlantVM GetPlantById(int id);
        Task CreatePlant(CreatePlantDTO createPlant, List<IFormFile> imageFiles);
        void UpdatePlant(UpdatePlantDTO updatePlant);
        IEnumerable<PlantVM> GetPlantsByCategoryId(int categoryId, int pageIndex, int pageSize);
        IEnumerable<PlantVM> GetListPlantsByTypeEcommerceId(int pageIndex, int pageSize, int typeEcommerceId);
    }
}
