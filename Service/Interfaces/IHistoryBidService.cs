using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IHistoryBidService
    {
        void CreateHistoryBid(HistoryBid bid);
    }
}
