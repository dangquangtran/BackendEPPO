using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Rank
{
    public class CreateRankDTO
    {
        public string Title { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Description { get; set; }

    }
    public class UpdateRanksDTO
    {
        public int RankId { get; set; }
        public string Title { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Description { get; set; }

    }
}
