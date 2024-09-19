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
        IEnumerable<Rank> GetAllRanks();
        Rank GetRankById(int id);
        void CreateRank(CreateRankDTO createRank);
        void UpdateRank(UpdateRankDTO updateRank);
        void DeleteRank(int id);
    }
}
