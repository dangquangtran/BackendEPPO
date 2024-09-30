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
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Category>> GetListCategory()
        {
            return await _unitOfWork.CategoriesRepository.GetAsync();
        }
        public async Task<Category> GetCategoryByID(int id)
        {
            return await Task.FromResult(_unitOfWork.CategoriesRepository.GetByID(id));
        }
 
    }
}
