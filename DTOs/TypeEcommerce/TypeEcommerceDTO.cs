using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.TypeEcommerce
{
    public class TypeEcommerceDTO
    {
        public int TypeEcommerceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
    }
    public class CreateTypeEcommerceDTO
    {
        public int TypeEcommerceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
    }
    public class UpdateTypeEcommerceDTO
    {
        public int TypeEcommerceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
    }
}
