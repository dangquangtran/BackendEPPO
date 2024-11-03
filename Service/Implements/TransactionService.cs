using AutoMapper;
using BusinessObjects.Models;
using DTOs.Transaction;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class TransactionService : ITransactionService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IEnumerable<TransactionVM> GetAllTransactions()
        {
            var transactions = _unitOfWork.TransactionRepository.Get(filter: c => c.Status != 0);
            return _mapper.Map<IEnumerable<TransactionVM>>(transactions);
        }

        public Transaction GetTransactionById(int id)
        {
            return _unitOfWork.TransactionRepository.GetByID(id);
        }

        public void CreateRechargeTransaction(CreateTransactionDTO createTransaction)
        {
            
            Transaction transaction = _mapper.Map<Transaction>(createTransaction);
            transaction.CreationDate = DateTime.Now;
            transaction.Status = 1;
            transaction.IsActive = true;
            transaction.Description = "Nạp tiền vào ví";
            transaction.RechargeDate = DateTime.Now;

            _unitOfWork.TransactionRepository.Insert(transaction);
            _unitOfWork.Save();
        }

        public void CreateWithdrawTransaction(CreateTransactionDTO createTransaction)
        {
           
            Transaction transaction = _mapper.Map<Transaction>(createTransaction);
            transaction.CreationDate = DateTime.Now;
            transaction.Status = 1;
            transaction.IsActive = true;
            transaction.WithdrawDate = DateTime.Now;
            transaction.Description = "Rút tiền từ ví";

            _unitOfWork.TransactionRepository.Insert(transaction);
            _unitOfWork.Save();
        }

        public void CreatePaymentTransaction(CreateTransactionDTO createTransaction, int orderId)
        {
            
            Transaction transaction = _mapper.Map<Transaction>(createTransaction);
            transaction.CreationDate = DateTime.Now;
            transaction.Status = 1;
            transaction.IsActive = true;
            transaction.WithdrawDate = DateTime.Now;
            transaction.Description = "Thanh toán đơn hàng "+orderId;

            _unitOfWork.TransactionRepository.Insert(transaction);
            _unitOfWork.Save();
        }
        public void UpdateTransaction(UpdateTransactionDTO updateTransaction)
        {
            Transaction transaction = _mapper.Map<Transaction>(updateTransaction);
            _unitOfWork.TransactionRepository.Update(transaction);
            _unitOfWork.Save();
        }
    }
}
