﻿using BusinessObjects.Models;
using DTOs.ContractDetails;
using DTOs.Feedback;
using DTOs.Plant;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class FeedbackService: IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Feedback>> GetListFeedback(int page, int size)
        {
            return await _unitOfWork.FeedbackRepository.GetAsync(filter: c => c.Status != 0, pageIndex: page, pageSize: size);
        }
        public async Task<Feedback> GetFeedbackByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.FeedbackRepository.GetByID(Id));
        }
        public async Task UpdateFeedback(UpdateFeedbackDTO feedback)
        {
            var entity = await Task.FromResult(_unitOfWork.FeedbackRepository.GetByID(feedback.FeedbackId));

            if (entity == null)
            {
                throw new Exception($"Feedback with ID {feedback.FeedbackId} not found.");
            }
            feedback.Title = feedback.Title;
            feedback.Description = feedback.Description;
            feedback.CreationDate = feedback.CreationDate;
            feedback.PlantId = feedback.PlantId;
            feedback.Rating = feedback.Rating;
            feedback.UserId = feedback.UserId;
            feedback.ModificationDate = feedback.ModificationDate;
            feedback.ModificationByUserId = feedback.ModificationByUserId;
            feedback.Status = feedback.Status;

            _unitOfWork.FeedbackRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task CreateFeedback(CreateFeedbackDTO feedback)
        {
            var entity = new Feedback
            {
                Title = feedback.Title,
                Description = feedback.Description,
                CreationDate = DateTime.Now,
                PlantId = feedback.PlantId,
                Rating = feedback.Rating,
                UserId = feedback.UserId,
                ModificationDate = DateTime.Now,
                ModificationByUserId = feedback.ModificationByUserId,
                Status = 1,
            };
            _unitOfWork.FeedbackRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteFeedback(DeleteFeedbackDTO feedback)
        {
            var entity = await Task.FromResult(_unitOfWork.FeedbackRepository.GetByID(feedback.FeedbackId));

            if (entity == null)
            {
                throw new Exception($"Feedback with ID {feedback.FeedbackId} not found.");
            }

            entity.Status = 0;

            _unitOfWork.FeedbackRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

    }
}
