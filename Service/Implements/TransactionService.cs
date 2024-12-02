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
            transaction.CreationDate = DateTime.UtcNow.AddHours(7);
            transaction.Status = 1;
            transaction.IsActive = true;
            transaction.Description = "Nạp tiền vào ví";
            transaction.RechargeDate = DateTime.UtcNow.AddHours(7);
            transaction.WithdrawNumber = null;

            _unitOfWork.TransactionRepository.Insert(transaction);
            var wallet = _unitOfWork.WalletRepository.GetByID(createTransaction.WalletId);
            if (wallet == null)
            {
                throw new Exception("Không tìm thấy ví của người dùng.");
            }

            wallet.NumberBalance += createTransaction.RechargeNumber; 

            _unitOfWork.WalletRepository.Update(wallet);
            _unitOfWork.Save();
        }

        public void CreateWithdrawTransaction(CreateTransactionDTO createTransaction)
        {
           
            Transaction transaction = _mapper.Map<Transaction>(createTransaction);
            transaction.CreationDate = DateTime.UtcNow.AddHours(7);
            transaction.Status = 1;
            transaction.IsActive = true;
            transaction.WithdrawDate = DateTime.UtcNow.AddHours(7);
            transaction.Description = "Rút tiền từ ví";
            //transaction.RechargeNumber = null;

            _unitOfWork.TransactionRepository.Insert(transaction);
            var wallet = _unitOfWork.WalletRepository.GetByID(createTransaction.WalletId);
            if (wallet == null)
            {
                throw new Exception("Không tìm thấy ví của người dùng.");
            }

            if (wallet.NumberBalance < createTransaction.WithdrawNumber)
            {
                throw new Exception("Số dư không đủ để thực hiện giao dịch rút tiền.");
            }

            wallet.NumberBalance -= createTransaction.WithdrawNumber;


            _unitOfWork.WalletRepository.Update(wallet);
            _unitOfWork.Save();
        }

        public void CreatePaymentTransaction(CreateTransactionDTO createTransaction, int orderId)
        {
            
            Transaction transaction = _mapper.Map<Transaction>(createTransaction);
            transaction.CreationDate = DateTime.UtcNow.AddHours(7);
            transaction.Status = 1;
            transaction.IsActive = true;
            transaction.WithdrawDate = DateTime.UtcNow.AddHours(7);
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

        public IEnumerable<TransactionVM> GetAllTransactionsInWallet(int page, int size, int walletId)
        {
            var transactions = _unitOfWork.TransactionRepository.Get(pageIndex: page, pageSize: size, filter: c => c.Status != 0 && c.WalletId == walletId);
            return _mapper.Map<IEnumerable<TransactionVM>>(transactions);
        }

        public async Task<IEnumerable<TransactionVM>> GetListTransactionsByToken(int page, int size, int typeEcommerce)
        {
            var transactions = _unitOfWork.TransactionRepository.Get(pageIndex: page, pageSize: size, filter: c => c.Status != 0 );
            return _mapper.Map<IEnumerable<TransactionVM>>(transactions);
        }
    }
}
