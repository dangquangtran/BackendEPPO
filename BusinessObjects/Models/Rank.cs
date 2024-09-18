using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Rank
    {
        public Rank()
        {
            Users = new HashSet<User>();
        }

        public int RankId { get; set; }
        public string Title { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
