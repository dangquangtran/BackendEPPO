using BusinessObjects.Models;
using DTOs.Rank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IRankService
    {
        Task<IEnumerable<Rank>> GetListRanks(int page, int size);
        Task<Rank> GetRankByID(int Id);


        IEnumerable<Rank> GetAllRanks();
        Rank GetRankById(int id);
        void CreateRank(CreateRankDTO createRank);
        void UpdateRank(UpdateRankDTO updateRank);
    }
}
