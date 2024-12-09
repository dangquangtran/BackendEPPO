using BusinessObjects.Models;
using DTOs.Plant;
using DTOs.User;
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
        IEnumerable<PlantVM> GetAllPlantsToResgister(int pageIndex, int pageSize); 
        PlantVM GetPlantById(int id);
        Task CreatePlantByOwner(CreatePlantDTO createPlant, IFormFile mainImageFile, List<IFormFile> imageFiles, int userId, string code);
     
        IEnumerable<PlantVM> GetPlantsByCategoryId(int pageIndex, int pageSize, int categoryId);
        IEnumerable<PlantVM> GetListPlantsByTypeEcommerceId(int pageIndex, int pageSize, int typeEcommerceId);
        IEnumerable<PlantVM> GetListPlantsByTypeEcommerceAndCategory(int pageIndex, int pageSize, int typeEcommerceId, int categoryId);
        Task<IEnumerable<PlantVM>> SearchPlantKeyType(int pageIndex, int pageSize, int typeEcommerceId, string keyword);


        Task<IEnumerable<PlantVM>> CheckPlantInCart(List<int> PlantId);
        IEnumerable<PlantVM> SearchPlants(string keyword,int typeEcommerceId, int pageIndex, int pageSize);


        Task CreatePlantByOwner(CreatePlantDTOByOwner plant , string userId);

        Task CreatePlantByOwner(CreatePlantDTOTokenOwner createPlant, IFormFile mainImageFile, List<IFormFile> imageFiles , int userId);
        Task UpdatePlantByManager(UpdatePlantDTO updatePlant,int plantId , IFormFile mainImageFile, List<IFormFile> newImageFiles);

        IEnumerable<PlantVM> GetListPlantOfOwnerByTypeEcommerceId(int pageIndex, int pageSize, int? typeEcommerceId, string code);

        Task UpdatePlantStatus(UpdatePlantStatus updatePlant, int plantId);

        Task UpdatePlantIdByManager(UpdatePlantIdDTO updatePlant,int plantId, IFormFile mainImageFile);

        Task<int> CountShipByPlant(int plantId);

        IEnumerable<PlantVM> GetListPlantsByTypeEcommerceIdManage(int pageIndex, int pageSize, int typeEcommerceId);

        IEnumerable<PlantVM> ViewPlantsToAccept(int pageIndex, int pageSize,  string code, int typeEcommerceId);
        IEnumerable<PlantVM> ViewPlantsWaitAccept(int pageIndex, int pageSize, string code);
        IEnumerable<PlantVM> ViewPlantsUnAccept(int pageIndex, int pageSize, string code);

        Task<bool> CancelContractPlant(int plantId);
        Task<int> CalculateDeposit(int plantId);
    }
}
