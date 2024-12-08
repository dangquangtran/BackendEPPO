using AutoMapper;
using BusinessObjects.Models;
using DTOs.Contracts;
using DTOs.ImagePlant;
using DTOs.Plant;
using DTOs.User;
using Firebase.Auth;
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
            var plants = _unitOfWork.PlantRepository.Get(filter: c => c.Status != 0, orderBy: query => query.OrderByDescending(c => c.PlantId), pageIndex : pageIndex, pageSize : pageSize, includeProperties: "ImagePlants");
            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }

        public IEnumerable<PlantVM> GetAllPlantsToResgister(int pageIndex, int pageSize)
        {
            var plants = _unitOfWork.PlantRepository.Get(filter: c => c.Status == 2 && c.IsActive == false, pageIndex: pageIndex, pageSize: pageSize, includeProperties: "ImagePlants");
            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }

        public PlantVM GetPlantById(int id)
        {
            var plant = _unitOfWork.PlantRepository.GetByID(id, includeProperties: "ImagePlants");

            return _mapper.Map<PlantVM>(plant); ;
        }

      
        public async Task UpdatePlantByManager(UpdatePlantDTO updatePlant, int plantId, IFormFile mainImageFile, List<IFormFile> newImageFiles)
        {
            // Lấy thông tin plant từ DB
            var plant = _unitOfWork.PlantRepository.GetByID(plantId, includeProperties: "ImagePlants");
            if (plant == null) throw new Exception("Plant không tồn tại");

            // Cập nhật các thuộc tính của plant
            _mapper.Map(updatePlant, plant);
            plant.ModificationDate = DateTime.UtcNow.AddHours(7);

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
                filter: c => c.CategoryId == categoryId,
                orderBy: query => query.OrderByDescending(c => c.PlantId),
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "ImagePlants"
            );

            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }
        public IEnumerable<PlantVM> GetListPlantsByTypeEcommerceId(int pageIndex, int pageSize, int typeEcommerceId)
        {
            var plants = _unitOfWork.PlantRepository.Get(
                filter: c => c.TypeEcommerceId == typeEcommerceId && c.IsActive != false && c.Status == 2, 
                orderBy: query => query.OrderByDescending(c => c.PlantId),
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "ImagePlants,Category"
            );

            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }

        public IEnumerable<PlantVM> GetListPlantsByTypeEcommerceIdManage(int pageIndex, int pageSize, int typeEcommerceId)
        {
            var plants = _unitOfWork.PlantRepository.Get(
                filter: c => c.TypeEcommerceId == typeEcommerceId,
                orderBy: query => query.OrderByDescending(c => c.PlantId),
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "ImagePlants,Category"
            );

            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }

        public IEnumerable<PlantVM> GetListPlantOfOwnerByTypeEcommerceId(int pageIndex, int pageSize, int? typeEcommerceId, string code)
        {
            var plants = _unitOfWork.PlantRepository.Get(
                filter: c => 
                c.Code == code && // check owner Id
                //c.Status == 2 &&
                (typeEcommerceId == null || c.TypeEcommerceId == typeEcommerceId),   // check ecomerce
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: query => query.OrderByDescending(c => c.PlantId),
                includeProperties: "ImagePlants,Category"
            );

            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }

        public IEnumerable<PlantVM> GetListPlantsByTypeEcommerceAndCategory(int pageIndex, int pageSize, int typeEcommerceId, int categoryId)
        {
               var plants = _unitOfWork.PlantRepository.Get(
               filter: c => c.TypeEcommerceId == typeEcommerceId && c.CategoryId == categoryId && c.Status == 2 && c.IsActive == true,
               orderBy: query => query.OrderByDescending(c => c.PlantId),
               pageIndex: pageIndex,
               pageSize: pageSize,
               includeProperties: "ImagePlants"
           );

            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }

        public IEnumerable<PlantVM> SearchPlants(string keyword, int typeEcommerceId, int pageIndex, int pageSize)
        {
            // Tìm kiếm cây theo từ khóa (có thể là tên hoặc một thuộc tính khác)
            var plants = _unitOfWork.PlantRepository.Get(
                filter: c => (c.PlantName.Contains(keyword) || c.Description.Contains(keyword))
                              && c.Status == 2
                              && c.TypeEcommerceId == typeEcommerceId
                              && c.IsActive == true,
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: query => query.OrderByDescending(c => c.PlantId),
                includeProperties: "ImagePlants"
            );

            return _mapper.Map<IEnumerable<PlantVM>>(plants); ;
        }

        public async Task<IEnumerable<PlantVM>> SearchPlantKeyType(int pageIndex, int pageSize, int typeEcommerceId, string keyword)
        {
            // Loại bỏ dấu khỏi từ khóa
            string normalizedKeyword = RemoveVietnameseDiacritics(keyword.ToLower());

            // Lấy danh sách cơ bản từ cơ sở dữ liệu
            var plants = await _unitOfWork.PlantRepository.GetAsync(
                filter: c => c.TypeEcommerceId == typeEcommerceId
                             && c.Status == 2
                             && c.IsActive == true,
                orderBy: query => query.OrderByDescending(c => c.PlantId),
                includeProperties: "ImagePlants"
            );

            // Áp dụng lọc loại bỏ dấu ở phía client
            var filteredPlants = plants
                .Where(c => RemoveVietnameseDiacritics(c.PlantName.ToLower()).Contains(normalizedKeyword)
                         || RemoveVietnameseDiacritics(c.Title.ToLower()).Contains(normalizedKeyword))
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return _mapper.Map<IEnumerable<PlantVM>>(filteredPlants);
        }


        public async Task<IEnumerable<PlantVM>> CheckPlantInCart(List<int> plantId)
        {
            var plantsInCart = await _unitOfWork.PlantRepository.GetAsync(
         filter: p => plantId.Contains(p.PlantId) && p.Status == 1, 
         includeProperties: "ImagePlants"
     );

            // Map the result to PlantVM
            var plantVMs = _mapper.Map<IEnumerable<PlantVM>>(plantsInCart);

            return plantVMs;
        }



        public async Task CreatePlantByOwner(CreatePlantDTOByOwner plant , string userId)
        {
         
            var plantEntity = new Plant
            {
                PlantName=plant.PlantName,
                Title = plant.Title,
                Description = plant.Description,
                Length = plant.Length,
                Width = plant.Width,
                Height = plant.Height,
                Price = plant.Price, // nhập giá mong đợi của owner
                Discounts = plant.Discounts,// chia hoa hồng
                FinalPrice = plant.FinalPrice, // Giá cuối cùng khi đã lên shop eppo
                MainImage = plant.MainImage,
                CategoryId = plant.CategoryId,
                TypeEcommerceId = plant.TypeEcommerceId,
                Status = 1, // đã tạo plant
                IsActive =  false, // chờ manager xét duyệt
                CreationDate = DateTime.UtcNow.AddHours(7),
                ModificationDate = DateTime.UtcNow.AddHours(7),
                Code = userId,
          

            };

            _unitOfWork.PlantRepository.Insert(plantEntity);
            await _unitOfWork.SaveAsync();
        }
        public async Task CreatePlantByOwner(CreatePlantDTOTokenOwner createPlant, IFormFile mainImageFile, List<IFormFile> imageFiles , int userId)
        {
            Plant plant = _mapper.Map<Plant>(createPlant);
            plant.CreationDate = DateTime.UtcNow.AddHours(7);
            plant.ModificationDate = DateTime.UtcNow.AddHours(7);
            plant.Status = 1;
            plant.IsActive = false;
            plant.ModificationBy = userId;
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

        public async Task CreatePlantByOwner(CreatePlantDTO createPlant, IFormFile mainImageFile, List<IFormFile> imageFiles, int userId, string code)
        {
            Plant plant = _mapper.Map<Plant>(createPlant);
            plant.CreationDate = DateTime.UtcNow.AddHours(7);
            plant.Status = 1;
            plant.IsActive = false;
            plant.ModificationBy = userId;
            plant.Code = code;
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

        public async Task UpdatePlantStatus(UpdatePlantStatus updatePlant, int plantId)
        {
            var entity = await Task.FromResult(_unitOfWork.PlantRepository.GetByID(plantId));


            if (entity == null)
            {
                throw new Exception($"Contract with ID {plantId} not found.");
            }
            entity.ModificationDate = DateTime.Now;
            entity.Status = updatePlant.Status; 

            _unitOfWork.PlantRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }
        public async Task CancelContractPlant(int plantId)
        {
            var entity = await Task.FromResult(_unitOfWork.PlantRepository.GetByID(plantId));


            if (entity == null)
            {
                throw new Exception($"Plant with ID {plantId} not found.");
            }
            if (entity.IsActive == false)
            {
                throw new Exception($"Plant with ID {plantId} is already inactive and cannot be cancel.");
            }
            entity.ModificationDate = DateTime.Now;
            entity.Status = 0;
            //entity.IsActive = false;

            _unitOfWork.PlantRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdatePlantIdByManager(UpdatePlantIdDTO updatePlant, int plantId, IFormFile mainImageFile)
        {
            var plantEntity = await Task.FromResult(_unitOfWork.PlantRepository.GetByID(plantId));
            if (plantEntity == null) throw new Exception("Plant không tồn tại");

            if (!string.IsNullOrWhiteSpace(updatePlant.PlantName))
            {
                plantEntity.PlantName = updatePlant.PlantName;
            }
            if (!string.IsNullOrWhiteSpace(updatePlant.Title))
            {
                plantEntity.Title = updatePlant.Title;
            }
            if (!string.IsNullOrWhiteSpace(updatePlant.Description))
            {
                plantEntity.Description = updatePlant.Description;
            }
            if (updatePlant.Length.HasValue)
            {
                plantEntity.Length = updatePlant.Length.Value;
            }
            if (updatePlant.Width.HasValue)
            {
                plantEntity.Width = updatePlant.Width.Value;
            }
            if (updatePlant.Height.HasValue)
            {
                plantEntity.Height = updatePlant.Height.Value;
            }
            if (updatePlant.Price.HasValue)
            {
                plantEntity.Price = updatePlant.Price.Value;
            }
            if (updatePlant.Discounts.HasValue)
            {
                plantEntity.Discounts = updatePlant.Discounts.Value;
            }
            if (updatePlant.FinalPrice.HasValue)
            {
                plantEntity.FinalPrice = updatePlant.FinalPrice.Value;
            }
            if (updatePlant.CategoryId.HasValue)
            {
                plantEntity.CategoryId = updatePlant.CategoryId.Value;
            }
            if (updatePlant.TypeEcommerceId.HasValue)
            {
                plantEntity.TypeEcommerceId = updatePlant.TypeEcommerceId.Value;
            }
            if (updatePlant.Status.HasValue)
            {
                plantEntity.Status = updatePlant.Status.Value;
            }
            if (updatePlant.IsActive.HasValue)
            {
                plantEntity.IsActive = updatePlant.IsActive.Value;
            }
            if (!string.IsNullOrWhiteSpace(updatePlant.Code))
            {
                plantEntity.Code = updatePlant.Code;
            }
            plantEntity.ModificationDate = DateTime.UtcNow.AddHours(7);

            if (mainImageFile != null)
            {
                using var stream = mainImageFile.OpenReadStream();
                string fileName = mainImageFile.FileName;

                string newImageUrl = await _firebaseStorageService.UploadUserImageAsync(stream, fileName);

                if (!string.IsNullOrWhiteSpace(plantEntity.MainImage))
                {
                    //await _firebaseStorageService.DeleteUserImageAsync(userEntity.ImageUrl);
                }
                plantEntity.MainImage = newImageUrl;
            }
            _unitOfWork.PlantRepository.Update(plantEntity);
            await _unitOfWork.SaveAsync();

        }

        public async Task<int> CountShipByPlant(int plantId)
        {
            // Lấy thông tin của cây dựa trên PlantId
            var plant = await _unitOfWork.PlantRepository.GetFirstOrDefaultAsync(
                filter: p => p.PlantId == plantId
            );

            if (plant == null)
            {
                throw new Exception("Plant not found.");
            }

            // Kiểm tra các thuộc tính kích thước
            double length = plant.Length ?? 0;
            double width = plant.Width ?? 0;
            double height = plant.Height ?? 0;

            // Tính thể tích
            double volume = length + (width + height) + (width + height);

            // Nếu không có kích thước hợp lệ, phí vận chuyển là 0
            if (volume <= 0)
            {
                return 0;
            }

            // Giá cơ bản cho mỗi đơn vị thể tích (10.000 VNĐ/m³)
            const double baseRate = 1000;

            // Tính phí vận chuyển
            double shippingCost = (volume * baseRate) + (volume * 0.10) + 50000;

            // Trả về giá trị làm tròn
            return (int)Math.Ceiling(shippingCost);
        }
        public IEnumerable<PlantVM> ViewPlantsToAccept(int pageIndex, int pageSize, string code, int typeEcommerceId)
        {
            var plants = _unitOfWork.PlantRepository.Get(
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: query => query.OrderByDescending(c => c.PlantId),
                filter: x => x.Code == code && x.TypeEcommerceId == typeEcommerceId && x.Status == 2,
                includeProperties: "ImagePlants");
            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }
        public IEnumerable<PlantVM> ViewPlantsWaitAccept(int pageIndex, int pageSize, string code)
        {
            var plants = _unitOfWork.PlantRepository.Get(
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: query => query.OrderByDescending(c => c.PlantId),
                filter: x => x.Code == code && x.Status == 1 && x.IsActive == false,
                includeProperties: "ImagePlants");
            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }
        public IEnumerable<PlantVM> ViewPlantsUnAccept(int pageIndex, int pageSize, string code)
        {
            var plants = _unitOfWork.PlantRepository.Get(
                pageIndex: pageIndex,
                pageSize: pageSize,
                orderBy: query => query.OrderByDescending(c => c.PlantId),
                filter: x => x.Code == code && x.Status == 0 && x.IsActive == false,
                includeProperties: "ImagePlants");
            return _mapper.Map<IEnumerable<PlantVM>>(plants);
        }
        public static string RemoveVietnameseDiacritics(string input)
        {
            string[] vietnameseSigns = new string[]
            {
        "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
            };

            for (int i = 1; i < vietnameseSigns.Length; i++)
            {
                for (int j = 0; j < vietnameseSigns[i].Length; j++)
                {
                    input = input.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
                }
            }
            return input;
        }

    }

}
