using DTOs.Transaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public IActionResult GetAllTransactions()
        {
            return Ok(_transactionService.GetAllTransactions());
        }

        [HttpGet("{id}")]
        public IActionResult GetTransactionById(int id)
        {
            return Ok(_transactionService.GetTransactionById(id));
        }

        [HttpPost]
        public IActionResult CreateTransaction([FromBody] CreateTransactionDTO createTransaction)
        {
            _transactionService.CreateTransaction(createTransaction);
            return Ok("Đã tạo thành công");
        }

        [HttpPut]
        public IActionResult UpdateTransaction([FromBody] UpdateTransactionDTO updateTransaction)
        {
            _transactionService.UpdateTransaction(updateTransaction);
            return Ok("Đã cập nhật thành công");
        }

    }
}
