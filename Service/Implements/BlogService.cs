using BusinessObjects.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BlogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Blog>> GetListBlog(int page, int size)
        {
            return await _unitOfWork.BlogRepository.GetAsync(pageIndex: page, pageSize: size);
        }
        public async Task<Blog> GetBlogByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.BlogRepository.GetByID(Id));
        }
    }
}
