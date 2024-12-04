using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class TokenService : ITokenService
    {
        public int? ValidateToken(string faceIdToken)
        {
            if (string.IsNullOrEmpty(faceIdToken))
            {
                return null; // Token không hợp lệ
            }

            // Giải mã token để lấy userId
            return DecodeToken(faceIdToken);
        }

        private int? DecodeToken(string token)
        {
            // Logic giải mã token (giả lập)
            // Ví dụ: Giả sử token chứa userId dưới dạng số
            return token == "valid-face-id-token" ? 123 : (int?)null; // Trả về null nếu token không hợp lệ
        }
    }

}


