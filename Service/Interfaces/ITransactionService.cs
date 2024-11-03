using BusinessObjects.Models;
using DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ITransactionService
    {
        IEnumerable<TransactionVM> GetAllTransactions();
        Transaction GetTransactionById(int id);
        void CreateRechargeTransaction(CreateTransactionDTO createTransaction);
        void CreateWithdrawTransaction(CreateTransactionDTO createTransaction);
        void CreatePaymentTransaction(CreateTransactionDTO createTransaction, int orderId);
        void UpdateTransaction(UpdateTransactionDTO updateTransaction);
    }
}
