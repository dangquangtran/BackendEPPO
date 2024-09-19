using AutoMapper;
using BusinessObjects.Models;
using DTOs.Rank;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IUserService
    {
        IEnumerable<User> GetListUsers();
       
    }
}
