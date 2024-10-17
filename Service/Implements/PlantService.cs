using AutoMapper;
using BusinessObjects.Models;
using DTOs.Plant;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PlantService : IPlantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlantService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Plant>> GetListPlants(int page, int size)
        {
            return await _unitOfWork.PlantRepository.GetAsync(pageIndex: page, pageSize: size);
        }
        public async Task<Plant> GetPlantByID(int id)
        {
            return await Task.FromResult(_unitOfWork.PlantRepository.GetByID(id));
        }
        public async Task<IEnumerable<Plant>> GetListPlantByCategory(int Id)
        {
            return await _unitOfWork.PlantRepository.GetAsync();
        }
        public IEnumerable<PlantVM> GetAllPlants(int pageIndex, int pageSize)
        {
            var plants = _unitOfWork.PlantRepository.Get(filter: c => c.Status != 0, pageIndex : pageIndex, pageSize : pageSize);
            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }

        public Plant GetPlantById(int id)
        {
            return _unitOfWork.PlantRepository.GetByID(id);
        }

        public void CreatePlant(CreatePlantDTO createPlant)
        {
            Plant plant = _mapper.Map<Plant>(createPlant);
            plant.CreationDate = DateTime.Now;
            plant.Status = 1;
            plant.IsActive = true;
            _unitOfWork.PlantRepository.Insert(plant);
            _unitOfWork.Save();
        }
        public void UpdatePlant(UpdatePlantDTO updatePlant)
        {
            Plant plant = _mapper.Map<Plant>(updatePlant);
            plant.ModificationDate = DateTime.Now;
            _unitOfWork.PlantRepository.Update(plant);
            _unitOfWork.Save();
        }
        public IEnumerable<PlantVM> GetPlantsByCategoryId(int categoryId, int pageIndex, int pageSize)
        {
            var plants = _unitOfWork.PlantRepository.Get(
                filter: c => c.CategoryId == categoryId && c.Status != 0, 
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }

    }
}
