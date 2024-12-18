﻿using BusinessObjects.Models;
using DTOs.Notification;
using DTOs.Transaction;
using DTOs.Wallet;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Wallet>> GetListTransactionsByWallet(int id)
        {
            return await Task.FromResult(_unitOfWork.WalletRepository
                .Get(filter: w => w.WalletId == id && w.Status != 0, includeProperties: "Transactions"));
        }

        public async Task<IEnumerable<Wallet>> GetListWallet(int page, int size)
        {
            return await _unitOfWork.WalletRepository.GetAsync(pageIndex: page, pageSize: size);
        }
        public async Task<Wallet> GetWalletByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.WalletRepository.GetByID(Id));
        }
        public async Task CreateWallet(CreateWalletDTO wallet)
        {
            var entity = new Wallet
            {
                NumberBalance = wallet.NumberBalance,
                CreationDate = DateTime.UtcNow.AddHours(7),
                ModificationDate = DateTime.UtcNow.AddHours(7),
                Status = 1,
            };
            _unitOfWork.WalletRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }
        public async Task UpdateWallet(UpdateWalletDTO wallet)
        {

            var entity = await Task.FromResult(_unitOfWork.WalletRepository.GetByID(wallet.WalletId));

            if (entity == null)
            {
                throw new Exception($"Wallet with ID {wallet.WalletId} not found.");
            }
            entity.NumberBalance = wallet.NumberBalance;
            entity.ModificationDate = DateTime.UtcNow.AddHours(7);
            entity.Status = wallet.Status;

            _unitOfWork.WalletRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
