using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class HistoryBidService : IHistoryBidService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HistoryBidService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void CreateHistoryBid(HistoryBid bid)
        {
             _unitOfWork.HistoryBidRepository.Insert(bid);
             _unitOfWork.Save();
        }
    }
}
