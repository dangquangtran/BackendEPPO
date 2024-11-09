using BusinessObjects.Models;
using DTOs.Category;
using DTOs.Wallet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetListCategory(int page, int size);
        Task<Category> GetCategoryByID(int Id);
        Task CreateCategory(CreateCategoryDTO category);
        Task UpdateCategory(UpdateCategoryDTO category);
        Task DeleteCategory(DeleteCategoryDTO category);
    }
}
