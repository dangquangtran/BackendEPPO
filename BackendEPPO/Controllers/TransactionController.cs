using DTOs.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service.Interfaces;
using ZaloPay.Helper.Crypto;
using ZaloPay.Helper;

namespace BackendEPPO.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly string app_id = "2553";
        private readonly string key1 = "PcY4iZIKFCIdgZvA6ueMcMHHUbRLYjPL";
        private readonly string create_order_url = "https://sb-openapi.zalopay.vn/v2/create";
        private string callbackUrl = "https://sep490ne-001-site1.atempurl.com/api/v1/Transaction/Callback";
        private readonly string redirectUrl = "https://localhost:7097/UserPage/MyOrder/OrderDetail?id=";
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

        //[HttpPost]
        //public IActionResult CreateTransaction([FromBody] CreateTransactionDTO createTransaction)
        //{
        //    _transactionService.CreateTransaction(createTransaction);
        //    return Ok("Đã tạo thành công");
        //}
      
        [Authorize]
        [HttpPost("CreateTransaction")]
        public async Task<IActionResult> CreateTransaction([FromForm] CreateTransactionDTO createTransaction)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);
            Random rnd = new Random();
            //var embed_data = new { redirecturl = redirectUrl + id };
                var embed_data = new
                {
                    WalletId = createTransaction.WalletId,
                    PaymentId = createTransaction.PaymentId,
                    WithdrawNumber = createTransaction.WithdrawNumber,
                    RechargeNumber = createTransaction.RechargeNumber
                };
            var items = new[] { new { } };
            var param = new Dictionary<string, string>();
            var app_trans_id = rnd.Next(1000000); // Generate a random order's ID.

            param.Add("app_id", app_id);
            param.Add("app_user", "user" + userId);
            param.Add("app_time", Utils.GetTimeStamp().ToString());
            param.Add("amount", createTransaction.RechargeNumber.ToString());
            param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
            param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
            param.Add("item", JsonConvert.SerializeObject(items));
            param.Add("description", "Nạp tiền vào ví");
            param.Add("bank_code", "zalopayapp");
            param.Add("callback_url", callbackUrl);

            var data = app_id + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync(create_order_url, param);
            return Ok(result);
        }

        private string key2 = "eG4r0GcoNtRGbO8";

        [HttpPost("Callback")]
        public IActionResult Post([FromBody] dynamic cbdata)
        {
            var result = new Dictionary<string, object>();

            try
            {
                var dataStr = cbdata.GetProperty("data").GetString();
                var reqMac = cbdata.GetProperty("mac").GetString();

                var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key2, dataStr);

                Console.WriteLine("mac = {0}", mac);

                // kiểm tra callback hợp lệ (đến từ ZaloPay server)

                // thanh toán thành công
                // merchant cập nhật trạng thái cho đơn hàng                
                var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                Console.WriteLine("update order's status = success where app_trans_id = {0}", dataJson["app_trans_id"]);

                var embedDataStr = Convert.ToString(dataJson["embed_data"]);
                var createTransaction = JsonConvert.DeserializeObject<CreateTransactionDTO>(embedDataStr);
                _transactionService.CreateRechargeTransaction(createTransaction);
                result["return_code"] = 1;
                result["return_message"] = "success";

            }
            catch (Exception ex)
            {
                result["return_code"] = 0; // ZaloPay server sẽ callback lại (tối đa 3 lần)
                result["return_message"] = ex.Message;
            }

            // thông báo kết quả cho ZaloPay server
            return Ok(result);
        }

        [HttpPut]   
        public IActionResult UpdateTransaction([FromBody] UpdateTransactionDTO updateTransaction)
        {
            _transactionService.UpdateTransaction(updateTransaction);
            return Ok("Đã cập nhật thành công");
        }

        [Authorize]
        [HttpPost("Withdraw")]
        public IActionResult Withdraw([FromBody] CreateTransactionDTO createTransaction)
        {
            try
            {
                // Kiểm tra số tiền rút phải lớn hơn 0
                if (createTransaction.WithdrawNumber <= 0)
                {
                    return BadRequest("Số tiền rút phải lớn hơn 0.");
                }

                _transactionService.CreateWithdrawTransaction(createTransaction);
                return Ok("Giao dịch rút tiền thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
