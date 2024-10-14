using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserVoucherService
    {
        Task<IEnumerable<UserVoucher>> GetListUserVoucher(int page, int size);
        Task<UserVoucher> GetUserVoucherByID(int Id);
    }
}
