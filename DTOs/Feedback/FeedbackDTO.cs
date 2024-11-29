using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Feedback
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }
        public string? Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? PlantId { get; set; }
        public int? Rating { get; set; }
        public int? UserId { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationByUserId { get; set; }
        public int? Status { get; set; }
    }
    public class CreateFeedbackDTO
    {
        //public string Title { get; set; }
        public string Description { get; set; }
        //public DateTime? CreationDate { get; set; }
        public int? PlantId { get; set; }
        public int? Rating { get; set; }
        //public int? UserId { get; set; }
        //public DateTime? ModificationDate { get; set; }
        //public int? ModificationByUserId { get; set; }
        //public int? Status { get; set; }
        public List<IFormFile>? ImageFiles { get; set; }
    }
    public class UpdateFeedbackDTO
    {
        public int FeedbackId { get; set; }
        //public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsFeedback { get; set; }
        public int? PlantId { get; set; }
        public int? Rating { get; set; }
        public int? UserId { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModificationByUserId { get; set; }
        public int? Status { get; set; }
    }
    public class DeleteFeedbackDTO
    {
        public int FeedbackId { get; set; }
    }
}
