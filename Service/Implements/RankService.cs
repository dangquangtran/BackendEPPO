using AutoMapper;
using BusinessObjects.Models;
using DTOs.Rank;
using DTOs.User;
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
        public async Task<IEnumerable<Rank>> GetListRanks(int page, int size)
        {
            return await _unitOfWork.RankRepository.GetAsync(pageIndex: page, pageSize: size);
        }
        public async Task<Rank> GetRankByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.RankRepository.GetByID(Id));
        }

        public IEnumerable<Rank> GetAllRanks()
        {
            return _unitOfWork.RankRepository.Get();
        }
        public async Task CreateRankByManager(CreateRankDTO rank)
        {
            var rankEntity = new Rank
            {
               Title = rank.Title,
               Description = rank.Description,
               CreationDate = DateTime.Now,
               UpdateDate = DateTime.Now,
            };

            _unitOfWork.RankRepository.Insert(rankEntity);
            await _unitOfWork.SaveAsync();
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
        public async Task UpdateRank(UpdateRanksDTO rank)
        {
         
            var rankEntity = await Task.FromResult(_unitOfWork.RankRepository.GetByID(rank.RankId));

            if (rankEntity == null)
            {
                throw new Exception($"Rank with ID {rank.RankId} not found.");
            }

         
            rankEntity.Title = rank.Title;
            rankEntity.Description = rank.Description;
            rankEntity.UpdateDate = DateTime.Now;

 
            _unitOfWork.RankRepository.Update(rankEntity);
            await _unitOfWork.SaveAsync();
        }

    }
}
