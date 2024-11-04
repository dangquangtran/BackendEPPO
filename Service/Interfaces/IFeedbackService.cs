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

        Task CreateFeedback(CreateFeedbackDTO feedback, List<IFormFile> imageFiles);
        Task UpdateFeedback(UpdateFeedbackDTO feedback);
    }
}
