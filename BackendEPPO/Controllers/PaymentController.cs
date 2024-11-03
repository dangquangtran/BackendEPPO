using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ZaloPay.Helper.Crypto;
using ZaloPay.Helper;
using Service.Interfaces;
using Mysqlx.Crud;
using DTOs.Order;
using Service.Implements;
using Microsoft.AspNetCore.Authorization;
using DTOs.Transaction;
using BusinessObjects.Models;

namespace BackendEPPO.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly string app_id = "2553";
        private readonly string key1 = "PcY4iZIKFCIdgZvA6ueMcMHHUbRLYjPL";
        private readonly string create_order_url = "https://sb-openapi.zalopay.vn/v2/create";
        private string callbackUrl = "https://sep490ne-001-site1.atempurl.com/api/v1/Payment/Callback/";
        private readonly string redirectUrl = "https://localhost:7097/UserPage/MyOrder/OrderDetail?id=";
        private readonly IOrderService _orderService;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;

        public PaymentController(IOrderService orderService , ITransactionService transactionService, IUserService userService)
        {
            _orderService = orderService;
            _transactionService = transactionService;
            _userService = userService;
        }


        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
            {
                return NotFound("Order không tồn tại.");
            }

            Random rnd = new Random();
            //var embed_data = new { redirecturl = redirectUrl + id };
            var embed_data = new {  };
            var items = new[] { new { } };
            var param = new Dictionary<string, string>();
            var app_trans_id = rnd.Next(1000000); // Generate a random order's ID.

            param.Add("app_id", app_id);
            param.Add("app_user", "user" + order.UserId);
            param.Add("app_time", Utils.GetTimeStamp().ToString());
            param.Add("amount", order.FinalPrice.ToString());
            param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
            param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
            param.Add("item", JsonConvert.SerializeObject(items));
            param.Add("description", "Thanh toán đơn hàng #" + orderId + app_trans_id);
            param.Add("bank_code", "zalopayapp");
            param.Add("callback_url", callbackUrl + orderId);

            var data = app_id + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync(create_order_url, param);
            return Ok(result);
        }

        private string key2 = "eG4r0GcoNtRGbO8";

        [HttpPost("Callback/{id}")]
        public IActionResult Post([FromBody] dynamic cbdata, [FromRoute] int id)
        {
            var order = _orderService.GetOrderById(id);
            int userId = order.UserId;
            var user = _userService.GetUserByID(userId);
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

                _orderService.UpdatePaymentStatus(id, "Đã thanh toán");
                if (order != null)
                {
                    var transactionDto1 = new CreateTransactionDTO
                    {
                        WalletId = user.WalletId,
                        PaymentId = 2,
                        RechargeNumber = order.FinalPrice
                    };

                    _transactionService.CreateRechargeTransaction(transactionDto1);

                    var transactionDto2 = new CreateTransactionDTO
                    {
                        WalletId = user.WalletId,
                        PaymentId = 2,
                        WithdrawNumber = order.FinalPrice
                    };

                    _transactionService.CreatePaymentTransaction(transactionDto2, id);
                }
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
       
    }
}

