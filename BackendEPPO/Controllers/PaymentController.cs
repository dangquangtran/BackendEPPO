using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ZaloPay.Helper.Crypto;
using ZaloPay.Helper;

namespace BackendEPPO.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly string app_id = "2553";
        private readonly string key1 = "PcY4iZIKFCIdgZvA6ueMcMHHUbRLYjPL";
        private readonly string create_order_url = "https://sb-openapi.zalopay.vn/v2/create";
        private readonly string redirectUrl = "https://localhost:7170/api/Product/GetAllProducts";
        private readonly string refund_url = "https://sb-openapi.zalopay.vn/v2/refund";
        public PaymentController()
        {
        }


        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder()
        {

            Random rnd = new Random();
            var embed_data = new { redirecturl = redirectUrl };
            var items = new[] { new { } };
            var param = new Dictionary<string, string>();
            var app_trans_id = rnd.Next(1000000); // Generate a random order's ID.

            param.Add("app_id", app_id);
            param.Add("app_user", "user123");
            param.Add("app_time", Utils.GetTimeStamp().ToString());
            param.Add("amount", "50000");
            param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
            param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
            param.Add("item", JsonConvert.SerializeObject(items));
            param.Add("description", "Lazada - Thanh toán đơn hàng #" + app_trans_id);
            param.Add("bank_code", "zalopayapp");
            //param.Add("callback_url", "https://0b6a-2402-800-be09-c3d2-b125-ecd8-6f5b-b158.ngrok-free.app/odata/Callback/" + order.OrderID);

            var data = app_id + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync(create_order_url, param);
            //var order = _mapper.Map<Order>(orderDTO);
            return Ok(result);
        }

        private string key2 = "eG4r0GcoNtRGbO8";

        [HttpPost("Callback/{id}")]
        public IActionResult Post([FromBody] dynamic cbdata, [FromRoute] int id)
        {
            var result = new Dictionary<string, object>();

            try
            {
                var dataStr = Convert.ToString(cbdata["data"]);
                var reqMac = Convert.ToString(cbdata["mac"]);

                var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key2, dataStr);

                Console.WriteLine("mac = {0}", mac);

                // kiểm tra callback hợp lệ (đến từ ZaloPay server)

                // thanh toán thành công
                // merchant cập nhật trạng thái cho đơn hàng
                var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                Console.WriteLine("update order's status = success where app_trans_id = {0}", dataJson["app_trans_id"]);
                //_orderService.UpdatePaymentSuccess(id);

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

        [HttpPost("Refund")]
        public async Task<IActionResult> Refund()
        {

            var timestamp = Utils.GetTimeStamp().ToString();
            var rand = new Random();
            var uid = timestamp + "" + rand.Next(111, 999).ToString();

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("app_id", app_id);
            param.Add("m_refund_id", DateTime.Now.ToString("yyMMdd") + "_" + app_id + "_" + uid);
            param.Add("zp_trans_id", "1234567");
            param.Add("amount", "1000");
            param.Add("timestamp", timestamp);
            param.Add("description", "demo");

            var data = app_id + "|" + param["zp_trans_id"] + "|" + param["amount"] + "|" + param["description"] + "|" + param["timestamp"];
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

            var result = await HttpHelper.PostFormAsync(refund_url, param);

            foreach (var entry in result)
            {
                Console.WriteLine("{0} = {1}", entry.Key, entry.Value);
            }
            return Ok(result);
        }
    }
}

