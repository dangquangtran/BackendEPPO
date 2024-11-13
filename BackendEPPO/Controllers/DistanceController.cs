using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistanceController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DistanceController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
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
            // Fetch API key from configuration
            string apiKey = _configuration["GoogleMaps:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest(new { StatusCode = 400, Message = "API key is missing or not configured" });
            }

            // URL encode parameters
            origin = Uri.EscapeDataString(origin);
            destination = Uri.EscapeDataString(destination);

            string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&key={apiKey}";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                var status = json["rows"]?[0]?["elements"]?[0]?["status"]?.ToString();
                if (status == "OK")
                {
                    var distanceInMeters = json["rows"][0]["elements"][0]["distance"]["value"].Value<double>();
                    var distanceInKm = distanceInMeters / 1000;
                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = "Distance calculated successfully",
                        Distance = distanceInKm
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Unable to calculate distance: " + status
                    });
                }
            }
            catch (JsonException e)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Error parsing JSON response", Error = e.Message });
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Error connecting to Google Maps API", Error = e.Message });
            }

        }
    }
}
