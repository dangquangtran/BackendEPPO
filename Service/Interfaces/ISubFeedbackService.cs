using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ISubFeedbackService
    {
        Task<IEnumerable<SubFeedback>> GetListSubFeedback(int page, int size);
        Task<SubFeedback> GetSubFeedbackByID(int Id);

    }
}
