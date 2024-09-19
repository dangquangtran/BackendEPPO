using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Rank
{
    public class UpdateRank
    {
        public int RankId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
