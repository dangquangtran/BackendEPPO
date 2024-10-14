using BusinessObjects.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class ImageFeedbackService: IImageFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImageFeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ImageFeedback>> GetListImageFeedback(int page, int size)
        {
            return await _unitOfWork.ImageFeedbackRepository.GetAsync(pageIndex: page, pageSize: size);
        }
        public async Task<ImageFeedback> GetImageFeedbackByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.ImageFeedbackRepository.GetByID(Id));
        }
    }
}
