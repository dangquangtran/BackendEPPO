using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IJwtTokenHelper
    {
        ClaimsPrincipal DecodeToken(string token);
        string GetClaimValue(string token, string claimType);
    }
}
