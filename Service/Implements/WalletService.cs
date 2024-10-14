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
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Wallet>> GetListWallet(int page, int size)
        {
            return await _unitOfWork.WalletRepository.GetAsync(pageIndex: page, pageSize: size);
        }
        public async Task<Wallet> GetWalletByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.WalletRepository.GetByID(Id));
        }
    }
}
