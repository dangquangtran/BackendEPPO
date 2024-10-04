using AutoMapper;
using BusinessObjects.Models;
using DTOs.Rank;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class RankService : IRankService
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

        public Rank GetRankById(int id)
        {
            return _unitOfWork.RankRepository.GetByID(id);
        }

        public void CreateRank(CreateRankDTO createRank)
        {
            Rank rank = _mapper.Map<Rank>(createRank);
            rank.CreationDate = DateTime.Now;
            _unitOfWork.RankRepository.Insert(rank);
            _unitOfWork.Save();
        }
        public void UpdateRank(UpdateRankDTO updateRank)
        {
            Rank rank = _mapper.Map<Rank>(updateRank);
            rank.UpdateDate = DateTime.Now;
            _unitOfWork.RankRepository.Update(rank);
            _unitOfWork.Save();
        }
    }
}
