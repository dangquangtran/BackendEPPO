using AutoMapper;
using BusinessObjects.Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RankService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RankService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IEnumerable<Rank> GetAllRanks()
        {
            return _unitOfWork.RankRepository.Get();
        }
    }
}
