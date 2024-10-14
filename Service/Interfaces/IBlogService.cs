using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IBlogService
    {
        Task<IEnumerable<Blog>> GetListBlog(int page, int size);
        Task<Blog> GetBlogByID(int Id);
    }
}
