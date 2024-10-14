using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IImageFeedbackService
    {
        Task<IEnumerable<ImageFeedback>> GetListImageFeedback(int page, int size);
        Task<ImageFeedback> GetImageFeedbackByID(int Id);

    }
}
