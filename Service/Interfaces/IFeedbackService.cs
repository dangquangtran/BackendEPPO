using BusinessObjects.Models;
using DTOs.Category;
using DTOs.Feedback;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IFeedbackService
    {

        Task<IEnumerable<Feedback>> GetListFeedback(int page, int size);
        Task<Feedback> GetFeedbackByID(int Id);

        Task CreateFeedback(CreateFeedbackDTO feedback,int userId, List<IFormFile> imageFiles);
        Task UpdateFeedback(UpdateFeedbackDTO feedback);
        Task DeleteFeedback(DeleteFeedbackDTO feedback);

        Task<IEnumerable<Feedback>> GetListFeedbackByPlant(int page, int size, int plantId);

        Task<IEnumerable<Feedback>> GetFeedbackByDeliveredPlants(int page, int size, int userId, int TypeEcommerceId);
        Task<IEnumerable<Order>> GetDeliveredOrdersForFeedback(int userId, int page, int size);
        Task<IEnumerable<Plant>> GetDeliveredPlantsForFeedback(int userId, int page, int size);
    }
}
