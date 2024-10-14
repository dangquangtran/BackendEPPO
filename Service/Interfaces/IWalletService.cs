using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
namespace Service.Interfaces
{
    public interface IWalletService
    {
        Task<IEnumerable<Wallet>> GetListWallet(int page, int size);
        Task<Wallet> GetWalletByID(int Id);

    }
    
}
