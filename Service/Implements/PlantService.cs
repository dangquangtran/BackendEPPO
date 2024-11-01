﻿using AutoMapper;
using BusinessObjects.Models;
using DTOs.Plant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository.Interfaces;
using Service.Implements;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PlantService : IPlantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly FirebaseStorageService _firebaseStorageService;

        public PlantService(IUnitOfWork unitOfWork, IMapper mapper, FirebaseStorageService firebaseStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _firebaseStorageService = firebaseStorageService;
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
            var plants = _unitOfWork.PlantRepository.Get(filter: c => c.Status != 0, pageIndex : pageIndex, pageSize : pageSize, includeProperties: "ImagePlants");
            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }

        public PlantVM GetPlantById(int id)
        {
            var plant = _unitOfWork.PlantRepository.GetByID(id, includeProperties: "ImagePlants");
            return _mapper.Map<PlantVM>(plant);
        }

        public async Task CreatePlant(CreatePlantDTO createPlant, List<IFormFile> imageFiles)
        {
            Plant plant = _mapper.Map<Plant>(createPlant);
            plant.CreationDate = DateTime.Now;
            plant.Status = 1;
            plant.IsActive = true;
            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var imageFile in imageFiles)
                {
                    using var stream = imageFile.OpenReadStream();
                    string fileName = imageFile.FileName;

                    // Upload từng hình ảnh lên Firebase và lấy URL
                    string imageUrl = await _firebaseStorageService.UploadImageAsync(stream, fileName);

                    // Lưu đường dẫn hình ảnh vào ImagePlant
                    ImagePlant imagePlant = new ImagePlant
                    {
                        PlantId = plant.PlantId, // Liên kết hình ảnh với plant đã tạo
                        ImageUrl = imageUrl
                    };

                    // Thêm imagePlant vào danh sách các ảnh của plant
                    plant.ImagePlants.Add(imagePlant);
                }
            }
            _unitOfWork.PlantRepository.Insert(plant);
            _unitOfWork.Save();
        }
        public async Task UpdatePlant(UpdatePlantDTO updatePlant, List<IFormFile> newImageFiles)
        {
            // Lấy thông tin plant từ DB
            var plant = _unitOfWork.PlantRepository.GetByID(updatePlant.PlantId, includeProperties: "ImagePlants");
            if (plant == null) throw new Exception("Plant không tồn tại");

            // Cập nhật các thuộc tính của plant
            _mapper.Map(updatePlant, plant);
            plant.ModificationDate = DateTime.Now;

            // Tạo một danh sách để lưu các URL ảnh cần xóa
            var existingImageUrls = plant.ImagePlants.Select(ip => ip.ImageUrl).ToList();

            // Nếu có ảnh mới, kiểm tra và thêm các ảnh đó
            if (newImageFiles != null && newImageFiles.Count > 0)
            {
                var newImageUrls = new List<string>();

                foreach (var imageFile in newImageFiles)
                {
                    using var stream = imageFile.OpenReadStream();
                    string fileName = imageFile.FileName;

                    // Upload ảnh mới lên Firebase và lấy URL
                    string imageUrl = await _firebaseStorageService.UploadImageAsync(stream, fileName);

                    // Thêm URL vào danh sách ảnh mới và plant
                    newImageUrls.Add(imageUrl);
                    plant.ImagePlants.Add(new ImagePlant { PlantId = plant.PlantId, ImageUrl = imageUrl });
                }

                // Xác định các ảnh cần xóa (ảnh cũ mà không có trong danh sách ảnh mới)
                var imagesToRemove = plant.ImagePlants.Where(ip => !newImageUrls.Contains(ip.ImageUrl)).ToList();

                foreach (var image in imagesToRemove)
                {
                    await _firebaseStorageService.DeleteImageAsync(image.ImageUrl); // Xóa ảnh khỏi Firebase
                    plant.ImagePlants.Remove(image); // Xóa ảnh khỏi DB
                }
            }

            // Lưu cập nhật vào DB
            _unitOfWork.PlantRepository.Update(plant);
            await _unitOfWork.SaveAsync();
        }
        public IEnumerable<PlantVM> GetPlantsByCategoryId(int categoryId, int pageIndex, int pageSize)
        {
            var plants = _unitOfWork.PlantRepository.Get(
                filter: c => c.CategoryId == categoryId && c.Status != 0, 
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "ImagePlants"
            );

            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }

    }
}
