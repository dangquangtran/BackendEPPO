using BusinessObjects.Models;
using DTOs.TypeEcommerce;
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
    public class TypeEcommerceService: ITypeEcommerceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TypeEcommerceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TypeEcommerce>> GetListTypeEcommerce(int page, int size)
        {
            return await _unitOfWork.TypeEcommerceRepository.GetAsync(pageIndex: page, pageSize: size);
        }
        public async Task<TypeEcommerce> GetTypeEcommerceByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.TypeEcommerceRepository.GetByID(Id));
        }
        public async Task CreateTypeEcommerce(CreateTypeEcommerceDTO typeEcommerce)
        {
            var entity = new TypeEcommerce
            {
                Title = typeEcommerce.Title,
                Description = typeEcommerce.Description,
                Status = 1,
            };
            _unitOfWork.TypeEcommerceRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }
        public async Task UpdateTypeEcommerce(UpdateTypeEcommerceDTO typeEcommerce)
        {

            var entity = await Task.FromResult(_unitOfWork.TypeEcommerceRepository.GetByID(typeEcommerce.TypeEcommerceId));

            if (entity == null)
            {
                throw new Exception($"TypeEcommerce with ID {typeEcommerce.TypeEcommerceId} not found.");
            }
            typeEcommerce.Title = typeEcommerce.Title;
            typeEcommerce.Description = typeEcommerce.Description;
            typeEcommerce.Status = typeEcommerce.Status;
            _unitOfWork.TypeEcommerceRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
