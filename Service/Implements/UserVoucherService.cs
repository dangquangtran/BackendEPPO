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
    public class UserVoucherService: IUserVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserVoucherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserVoucher>> GetListUserVoucher(int page, int size)
        {
            return await _unitOfWork.UserVoucherRepository.GetAsync(pageIndex: page, pageSize: size);
        }
        public async Task<UserVoucher> GetUserVoucherByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.UserVoucherRepository.GetByID(Id));
        }
    }
}
