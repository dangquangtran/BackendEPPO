using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Error
{
    public class ValidationError
    {
        public static readonly string BAD_REQUEST = "Không tìm thấy dữ liệu.";

        public static readonly string INVALID_DATE = "Ngày không hợp lệ.";

        public static readonly string FUTURE_DATE_NOT_ALLOWED = "Ngày không được ở tương lai.";

        public string FieldName { get; set; }
        public string ErrorMessage { get; set; }
        public object? InvalidValue { get; set; }

        public ValidationError(string fieldName, string errorMessage, object? invalidValue = null)
        {
            FieldName = fieldName;
            ErrorMessage = errorMessage;
            InvalidValue = invalidValue;
        }
    }
}
