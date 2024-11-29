using BusinessObjects.Models;
using DTOs.ContractDetails;
using DTOs.Feedback;
using DTOs.Plant;
using Microsoft.AspNetCore.Http;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class FeedbackService: IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FirebaseStorageService _firebaseStorageService;

        public FeedbackService(IUnitOfWork unitOfWork, FirebaseStorageService firebaseStorageService)
        {
            _unitOfWork = unitOfWork;
            _firebaseStorageService = firebaseStorageService;
        }

        public async Task<IEnumerable<Feedback>> GetListFeedback(int page, int size)
        {

           // return await _unitOfWork.FeedbackRepository.GetAsync(filter: c => c.Status != 0, pageIndex: page, pageSize: size);

            return await _unitOfWork.FeedbackRepository.GetAsync(filter: c => c.Status != 0, orderBy: query => query.OrderByDescending(c => c.FeedbackId), pageIndex: page, pageSize: size, includeProperties: "Plant,User,ImageFeedbacks");

        }

        public async Task<IEnumerable<Feedback>> GetListFeedbackByPlant(int page, int size, int plantId)
        {
            return await _unitOfWork.FeedbackRepository.GetAsync(filter: c => c.PlantId == plantId && c.Status != 0, orderBy: query => query.OrderByDescending(c => c.FeedbackId), pageIndex: page, pageSize: size, includeProperties: "Plant,User,ImageFeedbacks");
        }

        public async Task<Feedback> GetFeedbackByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.FeedbackRepository.GetByID(Id, includeProperties: "Plant,User,ImageFeedbacks"));
        }
        public async Task UpdateFeedback(UpdateFeedbackDTO feedback)
        {
            var entity = await Task.FromResult(_unitOfWork.FeedbackRepository.GetByID(feedback.FeedbackId));

            if (entity == null)
            {
                throw new Exception($"Feedback with ID {feedback.FeedbackId} not found.");
            }
            //entity.Title = feedback.Title;
            entity.Description = feedback.Description;
            entity.CreationDate = feedback.CreationDate;
            entity.PlantId = feedback.PlantId;
            entity.Rating = feedback.Rating;
            entity.UserId = feedback.UserId;
            entity.ModificationDate = feedback.ModificationDate;
            entity.ModificationByUserId = feedback.ModificationByUserId;
            entity.Status = feedback.Status;

            _unitOfWork.FeedbackRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task CreateFeedback(CreateFeedbackDTO feedback,int userId, List<IFormFile> imageFiles)
        {

            var existingFeedback = await _unitOfWork.FeedbackRepository.GetFirstOrDefaultAsync(
      f => f.PlantId == feedback.PlantId && f.UserId == userId && f.Status == 1
  );
            if (existingFeedback != null)
            {
                throw new InvalidOperationException("Bạn đã gửi feedback cho cây này rồi.");
            }

            var entity = new Feedback
            {
                PlantId = feedback.PlantId,
                //Title = feedback.Title,
                Description = feedback.Description,

                CreationDate = DateTime.Now,
                Rating = feedback.Rating,
                UserId = userId,
                ModificationDate = DateTime.Now,

          
  

                Status = 1,
            };
            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var imageFile in imageFiles)
                {
                    using var stream = imageFile.OpenReadStream();
                    string fileName = imageFile.FileName;

                    // Upload từng hình ảnh lên Firebase và lấy URL
                    string imageUrl = await _firebaseStorageService.UploadFeedbackImageAsync(stream, fileName);

                    ImageFeedback imageFeedback = new ImageFeedback
                    {
                        FeedbackId = entity.FeedbackId, 
                        ImageUrl = imageUrl
                    };

                    entity.ImageFeedbacks.Add(imageFeedback);
                }
            }
            _unitOfWork.FeedbackRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteFeedback(DeleteFeedbackDTO feedback)
        {
            var entity = await Task.FromResult(_unitOfWork.FeedbackRepository.GetByID(feedback.FeedbackId));

            if (entity == null)
            {
                throw new Exception($"Feedback with ID {feedback.FeedbackId} not found.");
            }

            entity.Status = 0;

            _unitOfWork.FeedbackRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<Feedback>> GetFeedbackByDeliveredPlants(int page, int size , int userId, int TypeEcommerceId)
        {
                    var feedbacks = await _unitOfWork.FeedbackRepository.GetAsync(
                 filter: f =>  f.Plant.TypeEcommerceId == TypeEcommerceId
                 && f.Plant.OrderDetails.Any(od => od.Order.Status == 4 // Đã giao thành công
                                                            && od.Order.UserId == userId), // Thanh toán hoàn tất

                 pageIndex: page,
                 pageSize: size,
                 includeProperties: "Plant,User,ImageFeedbacks"
             );


            return feedbacks;
        }
        public async Task<IEnumerable<Order>> GetDeliveredOrdersForFeedback(int userId, int page, int size)
        {
            var orders = await _unitOfWork.OrderRepository.GetAsync(
                filter: o => o.Status == 4 // Đã giao hàng thành công
                          && o.UserId == userId, // Lọc theo người dùng
                orderBy: query => query.OrderByDescending(o => o.CreationDate), // Sắp xếp mới nhất
                pageIndex: page,
                pageSize: size,
                includeProperties: "OrderDetails,OrderDetails.Plant" // Bao gồm các chi tiết cần thiết
            );

            return orders;
        }

        public async Task<IEnumerable<Plant>> GetDeliveredPlantsForFeedback(int userId, int page, int size)
        {
            // Lấy các OrderDetails có liên quan từ các đơn hàng đã giao hàng thành công
            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(
                filter: od => od.Order.Status == 4 // Đã giao hàng thành công
                              && od.Order.UserId == userId, // Lọc theo người dùng
                orderBy: query => query.OrderByDescending(od => od.Order.CreationDate), // Sắp xếp mới nhất
                pageIndex: page,
                pageSize: size,
                includeProperties: "Plant,Order" // Bao gồm thông tin Plant và Order
            );

            // Lấy danh sách các Plant từ OrderDetails
            var plants = orderDetails.Select(od => od.Plant).Distinct(); // Loại bỏ trùng lặp

            return plants;
        }


    }
}
