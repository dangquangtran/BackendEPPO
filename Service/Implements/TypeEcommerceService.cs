﻿using BusinessObjects.Models;
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
    }
}
