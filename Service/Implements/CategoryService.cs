using BusinessObjects.Models;
using DTOs.Category;
using DTOs.Wallet;
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

        public async Task<IEnumerable<Category>> GetListCategory(int page, int size)
        {
            return await _unitOfWork.CategoriesRepository.GetAsync(pageIndex: page, pageSize: size);
        }
        public async Task<Category> GetCategoryByID(int id)
        {
            return await Task.FromResult(_unitOfWork.CategoriesRepository.GetByID(id));
        }
        public async Task CreateCategory(CreateCategoryDTO category)
        {
            var entity = new Category
            {    
                Title = category.Title,
                Description = category.Description,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                ModificationById = category.ModificationById,
                Status = 1,
            };
            _unitOfWork.CategoriesRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }
        public async Task UpdateCategory(UpdateCategoryDTO category)
        {

            var entity = await Task.FromResult(_unitOfWork.CategoriesRepository.GetByID(category.CategoryId));

            if (entity == null)
            {
                throw new Exception($"Category with ID {category.CategoryId} not found.");
            }
            category.Title = category.Title;
            category.ModificationDate = DateTime.Now;
            category.Status = category.Status;
            _unitOfWork.CategoriesRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
