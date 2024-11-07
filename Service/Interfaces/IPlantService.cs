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
        Task UpdatePlant(UpdatePlantDTO updatePlant, List<IFormFile> newImageFiles);
        IEnumerable<PlantVM> GetPlantsByCategoryId(int pageIndex, int pageSize, int categoryId);
        IEnumerable<PlantVM> GetListPlantsByTypeEcommerceId(int pageIndex, int pageSize, int typeEcommerceId);
        IEnumerable<PlantVM> GetListPlantsByTypeEcommerceAndCategory(int pageIndex, int pageSize, int typeEcommerceId, int categoryId);
    }
}
