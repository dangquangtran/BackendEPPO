using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistanceController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DistanceController> _logger;

        public DistanceController(HttpClient httpClient, IConfiguration configuration, ILogger<DistanceController> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiKey = _configuration["GoogleMaps:ApiKey"];

            // Set timeout for HttpClient
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Calculates the distance between two addresses.
        /// </summary>
        /// <param name="origin">Origin address (e.g., "Quận 9, Thành phố Hồ Chí Minh")</param>
        /// <param name="destination">Destination address (e.g., "Quận 8, Thành phố Hồ Chí Minh")</param>
        /// <returns>Distance in kilometers</returns>
        [HttpGet("calculate")]
        public async Task<IActionResult> CalculateDistance(string origin, string destination)
        {
            // Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
            {
                _logger.LogWarning("Invalid input: Origin or Destination is missing.");
                return BadRequest(new { StatusCode = 400, Message = "Origin and Destination must be provided." });
            }

            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogError("API key is missing in configuration.");
                return BadRequest(new { StatusCode = 400, Message = "API key is missing or not configured." });
            }

            // Escape các ký tự đặc biệt trong chuỗi địa chỉ
            origin = Uri.EscapeDataString(origin);
            destination = Uri.EscapeDataString(destination);

            // Tạo URL cho API Google Maps Distance Matrix
            string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&key=AIzaSyBPVN9T3lA9gG0GErwN6N6HqA7IaCbG8Mo";

            try
            {
                _logger.LogInformation("Sending request to Google Maps API: {Url}", url);
                var response = await _httpClient.GetStringAsync(url);

                // In ra phản hồi để kiểm tra cấu trúc JSON
                _logger.LogInformation("Response from Google Maps API: {Response}", response);

                var json = JObject.Parse(response);

                // Kiểm tra xem có phần tử 'rows' không và chiều dài của nó
                var rows = json["rows"];
                if (rows != null && rows.HasValues)
                {
                    var elements = rows[0]["elements"];
                    if (elements != null && elements.HasValues)
                    {
                        var status = elements[0]["status"]?.ToString();
                        if (status == "OK")
                        {
                            var distanceInMeters = elements[0]["distance"]?["value"]?.Value<double>() ?? 0;
                            var distanceInKm = distanceInMeters / 1000;

                            _logger.LogInformation("Distance calculated successfully: {Distance} km", distanceInKm);
                            return Ok(new
                            {
                                StatusCode = 200,
                                Message = "Distance calculated successfully.",
                                Distance = distanceInKm
                            });
                        }
                        else
                        {
                            var errorReason = elements[0]["status"]?.ToString() ?? "Unknown error";
                            _logger.LogWarning("Failed to calculate distance. Status: {Status}", errorReason);
                            return BadRequest(new
                            {
                                StatusCode = 400,
                                Message = "Failed to calculate distance.",
                                Error = errorReason
                            });
                        }
                    }
                    else
                    {
                        var errorReason = "No elements found in response.";
                        _logger.LogWarning("Failed to calculate distance. Error: {ErrorReason}", errorReason);
                        return BadRequest(new
                        {
                            StatusCode = 400,
                            Message = "No distance elements found.",
                            Error = errorReason
                        });
                    }
                }
                else
                {
                    var errorReason = "No rows found in response.";
                    _logger.LogWarning("Failed to calculate distance. Error: {ErrorReason}", errorReason);
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "No rows found in the response.",
                        Error = errorReason
                    });
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Error connecting to Google Maps API: {Error}", e.Message);
                return StatusCode(500, new { StatusCode = 500, Message = "Error connecting to Google Maps API.", Error = e.Message });
            }
            catch (JsonException e)
            {
                _logger.LogError("Error parsing JSON response: {Error}", e.Message);
                return StatusCode(500, new { StatusCode = 500, Message = "Error parsing JSON response.", Error = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError("Unexpected error: {Error}", e.Message);
                return StatusCode(500, new { StatusCode = 500, Message = "An unexpected error occurred.", Error = e.Message });
            }


        }
    }
}
