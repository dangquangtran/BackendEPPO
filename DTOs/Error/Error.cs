using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Error
{
    public static class Error
    {
        public static readonly string NO_DATA_FOUND = "Không tìm thấy dữ liệu.";

        public static readonly string REQUESR_SUCCESFULL = "Yêu cầu đã thành công.";

        public static readonly string ERROR_500 = "Lỗi 500: Lỗi máy chủ";

        public static readonly string CREATE_ACCOUNT_SUCCESSFUL = "Tạo tài khoản thành công.";
        public static readonly string CREATE_ACCOUNT_FAIL = "Tài khoản mật khẩu đã tồn tại.";

        public static readonly string UPDATE_ACCOUNT_SUCCESSFUL = "Tài khoản cập nhật thành công.";
    }
}
