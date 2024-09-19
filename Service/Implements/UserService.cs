using AutoMapper;
using BusinessObjects.Models;
using DTOs.User;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserService : IUserService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public  IEnumerable<User> GetListUsers()
        {
            return  _unitOfWork.UserRepository.Get();
      
       
        }
    }
}
