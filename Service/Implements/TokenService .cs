using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class TokenService : ITokenService
    {
        public string ValidateToken(string faceIdToken)
        {
            if (string.IsNullOrEmpty(faceIdToken))
            {
                return null; // Token không hợp lệ
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(faceIdToken);  // Đọc và giải mã JWT token

                var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value; // Lấy userId từ claim

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return null; // Không tìm thấy userId trong token
                }

                return userIdClaim; // Trả về userId (dạng string hoặc Guid tùy cấu trúc token của bạn)
            }
            catch (Exception)
            {
                return null; // Xử lý lỗi nếu không thể giải mã token
            }
        }

    }

}


