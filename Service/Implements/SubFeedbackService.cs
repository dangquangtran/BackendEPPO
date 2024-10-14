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
    public class SubFeedbackService: ISubFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubFeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SubFeedback>> GetListSubFeedback(int page, int size)
        {
            return await _unitOfWork.SubFeedbackRepository.GetAsync(pageIndex: page, pageSize: size);
        }
        public async Task<SubFeedback> GetSubFeedbackByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.SubFeedbackRepository.GetByID(Id));
        }
    }
}
