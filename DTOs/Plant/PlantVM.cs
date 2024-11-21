using BusinessObjects.Models;
using DTOs.ImagePlant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Plant
{
    public class PlantVM
    {
        public int PlantId { get; set; }
        public string PlantName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double Price { get; set; }
        public double Discounts { get; set; }
        public double FinalPrice { get; set; }
        public string MainImage { get; set; }
        public int? CategoryId { get; set; }
        public int? TypeEcommerceId { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public virtual ICollection<ImagePlantVM> ImagePlants { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public string? Code { get; set; }
    }

    public class CheckCart
    {
        public int PlantId { get; set; }
    }

    public class PlantOwnerDTO
    {
        public int PlantId { get; set; }
        public string PlantName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double Price { get; set; }
        public double Discounts { get; set; }
        public double FinalPrice { get; set; }
        public string MainImage { get; set; }
        public int? CategoryId { get; set; }
        public int? TypeEcommerceId { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationBy { get; set; }
        public string? Code { get; set; }
        public virtual ICollection<ImagePlantVM> ImagePlants { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }

    }
}
