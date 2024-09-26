using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Service.Interfaces
{
    public interface IAccessoryService
    {
        Task<IEnumerable<Accessory>> GetListAccessory();
    }
}
