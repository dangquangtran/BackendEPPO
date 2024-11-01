﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Plant
{
    public class UpdatePlantDTO
    {
        public int PlantId { get; set; }
        public string PlantName { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Discounts { get; set; }
        public double FinalPrice { get; set; }
        public int? CategoryId { get; set; }
        public int? TypeEcommerceId { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }
        public List<IFormFile>? ImageFiles { get; set; }
    }
}