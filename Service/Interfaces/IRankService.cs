using BusinessObjects.Models;
using DTOs.Rank;
using DTOs.User;
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
        Task CreateRankByManager(CreateRankDTO rank);

        IEnumerable<Rank> GetAllRanks();
        Rank GetRankById(int id);
        Task UpdateRank(UpdateRanksDTO rank);


        void CreateRank(CreateRankDTO createRank);
        void UpdateRank(UpdateRankDTO updateRank);
    }
}
