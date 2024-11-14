using AutoMapper;
using BusinessObjects.Models;
using DTOs.ImagePlant;
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
            // Bao gồm các thuộc tính cần thiết
            var plant = _unitOfWork.PlantRepository.GetByID(id, includeProperties: "ImagePlants,ContractDetails.Contract");

            // Map dữ liệu và lấy ngày hợp đồng
            var plantVM = _mapper.Map<PlantVM>(plant);
            if (plant != null)
            {
                plantVM.RentalStartDate = plant.ContractDetails
                    .Select(cd => cd.Contract)
                    .OrderByDescending(contract => contract.CreationContractDate)
                    .FirstOrDefault()?.CreationContractDate;

                plantVM.RentalEndDate = plant.ContractDetails
                    .Select(cd => cd.Contract)
                    .OrderByDescending(contract => contract.EndContractDate)
                    .FirstOrDefault()?.EndContractDate;
            }

            return plantVM;
        }

        public async Task CreatePlant(CreatePlantDTO createPlant, IFormFile mainImageFile, List<IFormFile> imageFiles)
        {
            Plant plant = _mapper.Map<Plant>(createPlant);
            plant.CreationDate = DateTime.Now;
            plant.Status = 1;
            plant.IsActive = true;
            if (mainImageFile != null)
            {
                using var mainImageStream = mainImageFile.OpenReadStream();
                string mainImageFileName = mainImageFile.FileName;

                string mainImageUrl = await _firebaseStorageService.UploadPlantImageAsync(mainImageStream, mainImageFileName);

                plant.MainImage = mainImageUrl;
            }
            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var imageFile in imageFiles)
                {
                    using var stream = imageFile.OpenReadStream();
                    string fileName = imageFile.FileName;

                    // Upload từng hình ảnh lên Firebase và lấy URL
                    string imageUrl = await _firebaseStorageService.UploadPlantImageAsync(stream, fileName);

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
        public async Task UpdatePlant(UpdatePlantDTO updatePlant, IFormFile mainImageFile, List<IFormFile> newImageFiles)
        {
            // Lấy thông tin plant từ DB
            var plant = _unitOfWork.PlantRepository.GetByID(updatePlant.PlantId, includeProperties: "ImagePlants");
            if (plant == null) throw new Exception("Plant không tồn tại");

            // Cập nhật các thuộc tính của plant
            _mapper.Map(updatePlant, plant);
            plant.ModificationDate = DateTime.Now;

            // Cập nhật ảnh chính nếu có
            if (mainImageFile != null)
            {
                using var mainImageStream = mainImageFile.OpenReadStream();
                string mainImageFileName = mainImageFile.FileName;

                // Upload ảnh chính mới lên Firebase và lấy URL
                string mainImageUrl = await _firebaseStorageService.UploadPlantImageAsync(mainImageStream, mainImageFileName);

                // Gán URL mới vào MainImage và xử lý việc xóa ảnh chính cũ nếu cần
                if (!string.IsNullOrEmpty(plant.MainImage))
                {
                    //await _firebaseStorageService.DeletePlantImageAsync(plant.MainImage); // Xóa ảnh chính cũ khỏi Firebase nếu cần
                }
                plant.MainImage = mainImageUrl;
            }

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
                    string imageUrl = await _firebaseStorageService.UploadPlantImageAsync(stream, fileName);

                    // Thêm URL vào danh sách ảnh mới và plant
                    newImageUrls.Add(imageUrl);
                    plant.ImagePlants.Add(new ImagePlant { PlantId = plant.PlantId, ImageUrl = imageUrl });
                }

                // Xác định các ảnh cần xóa (ảnh cũ mà không có trong danh sách ảnh mới)
                var imagesToRemove = plant.ImagePlants.Where(ip => !newImageUrls.Contains(ip.ImageUrl)).ToList();

                foreach (var image in imagesToRemove)
                {
                    //await _firebaseStorageService.DeletePlantImageAsync(image.ImageUrl); // Xóa ảnh khỏi Firebase
                    plant.ImagePlants.Remove(image); // Xóa ảnh khỏi DB 
                }
            }

            // Lưu cập nhật vào DB
            _unitOfWork.PlantRepository.Update(plant);
            await _unitOfWork.SaveAsync();
        }
        public IEnumerable<PlantVM> GetPlantsByCategoryId(int pageIndex, int pageSize, int categoryId)
        {
            var plants = _unitOfWork.PlantRepository.Get(
                filter: c => c.CategoryId == categoryId && c.Status != 0, 
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "ImagePlants"
            );

            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }
        public IEnumerable<PlantVM> GetListPlantsByTypeEcommerceId(int pageIndex, int pageSize, int typeEcommerceId)
        {
            var plants = _unitOfWork.PlantRepository.Get(
                filter: c => c.TypeEcommerceId == typeEcommerceId && c.Status != 0,
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "ImagePlants,ContractDetails.Contract"
            );

            var plantVMs = plants.Select(plant => new PlantVM
            {
                PlantId = plant.PlantId,
                PlantName = plant.PlantName,
                Title = plant.Title,
                Description = plant.Description,
                Length = plant.Length,
                Width = plant.Width,
                Height = plant.Height,
                Price = plant.Price,
                Discounts = plant.Discounts,
                FinalPrice = plant.FinalPrice,
                MainImage = plant.MainImage,
                CategoryId = plant.CategoryId,
                TypeEcommerceId = plant.TypeEcommerceId,
                Status = plant.Status,
                IsActive = plant.IsActive,
                CreationDate = plant.CreationDate,
                ModificationDate = plant.ModificationDate,
                ModificationBy = plant.ModificationBy,
                ImagePlants = _mapper.Map<ICollection<ImagePlantVM>>(plant.ImagePlants),

                // Lấy ngày thuê và ngày hết hạn từ Contract liên quan
                RentalStartDate = plant.ContractDetails
             .Select(cd => cd.Contract)
             .OrderByDescending(contract => contract.CreationContractDate)
             .FirstOrDefault()?.CreationContractDate,
                RentalEndDate = plant.ContractDetails
             .Select(cd => cd.Contract)
             .OrderByDescending(contract => contract.EndContractDate)
             .FirstOrDefault()?.EndContractDate
            });

            return plantVMs;
        }


        public IEnumerable<PlantVM> GetListPlantsByTypeEcommerceAndCategory(int pageIndex, int pageSize, int typeEcommerceId, int categoryId)
        {
               var plants = _unitOfWork.PlantRepository.Get(
               filter: c => c.TypeEcommerceId == typeEcommerceId && c.CategoryId == categoryId && c.Status != 0,
               pageIndex: pageIndex,
               pageSize: pageSize,
               includeProperties: "ImagePlants,ContractDetails.Contract"
           );

            var plantVMs = plants.Select(plant => new PlantVM
            {
                PlantId = plant.PlantId,
                PlantName = plant.PlantName,
                Title = plant.Title,
                Description = plant.Description,
                Length = plant.Length,
                Width = plant.Width,
                Height = plant.Height,
                Price = plant.Price,
                Discounts = plant.Discounts,
                FinalPrice = plant.FinalPrice,
                MainImage = plant.MainImage,
                CategoryId = plant.CategoryId,
                TypeEcommerceId = plant.TypeEcommerceId,
                Status = plant.Status,
                IsActive = plant.IsActive,
                CreationDate = plant.CreationDate,
                ModificationDate = plant.ModificationDate,
                ModificationBy = plant.ModificationBy,
                ImagePlants = _mapper.Map<ICollection<ImagePlantVM>>(plant.ImagePlants),

                // Lấy ngày thuê và ngày hết hạn từ Contract liên quan
                RentalStartDate = plant.ContractDetails
              .Select(cd => cd.Contract)
              .OrderByDescending(contract => contract.CreationContractDate)
              .FirstOrDefault()?.CreationContractDate,
                RentalEndDate = plant.ContractDetails
              .Select(cd => cd.Contract)
              .OrderByDescending(contract => contract.EndContractDate)
              .FirstOrDefault()?.EndContractDate
            });

            return plantVMs;
        }

        public IEnumerable<PlantVM> SearchPlants(string keyword,int typeEcommerceId, int pageIndex, int pageSize)
        {
            // Tìm kiếm cây theo từ khóa (có thể là tên hoặc một thuộc tính khác)
            var plants = _unitOfWork.PlantRepository.Get(
                filter: c => (c.PlantName.Contains(keyword) || c.Description.Contains(keyword)) && c.Status != 0 && c.TypeEcommerceId == typeEcommerceId,
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "ImagePlants,ContractDetails.Contract"
            );

            var plantVMs = plants.Select(plant => new PlantVM
            {
                PlantId = plant.PlantId,
                PlantName = plant.PlantName,
                Title = plant.Title,
                Description = plant.Description,
                Length = plant.Length,
                Width = plant.Width,
                Height = plant.Height,
                Price = plant.Price,
                Discounts = plant.Discounts,
                FinalPrice = plant.FinalPrice,
                MainImage = plant.MainImage,
                CategoryId = plant.CategoryId,
                TypeEcommerceId = plant.TypeEcommerceId,
                Status = plant.Status,
                IsActive = plant.IsActive,
                CreationDate = plant.CreationDate,
                ModificationDate = plant.ModificationDate,
                ModificationBy = plant.ModificationBy,
                ImagePlants = _mapper.Map<ICollection<ImagePlantVM>>(plant.ImagePlants),

                // Lấy ngày thuê và ngày hết hạn từ Contract liên quan
                RentalStartDate = plant.ContractDetails
             .Select(cd => cd.Contract)
             .OrderByDescending(contract => contract.CreationContractDate)
             .FirstOrDefault()?.CreationContractDate,
                            RentalEndDate = plant.ContractDetails
             .Select(cd => cd.Contract)
             .OrderByDescending(contract => contract.EndContractDate)
             .FirstOrDefault()?.EndContractDate
                        });

            return plantVMs;
        }
    }
}
