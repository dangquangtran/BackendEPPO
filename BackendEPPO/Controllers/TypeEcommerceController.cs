using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeEcommerceController : ControllerBase
    {
        private readonly ITypeEcommerceService _service;

        public TypeEcommerceController(ITypeEcommerceService IService)
        {
            _service = IService;
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.TypeEcommerce.GetListTypeEcommerce_Endpoint)]
        public async Task<IActionResult> GetListTypeEcommerce(int page, int size)
        {
            var _typeEcommerce = await _service.GetListTypeEcommerce(page, size);

            if (_typeEcommerce == null || !_typeEcommerce.Any())
            {
                return NotFound("No Type Ecommerce found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _typeEcommerce
            });
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.TypeEcommerce.GetTypeEcommerceByID)]
        public async Task<IActionResult> GetTypeEcommerceByID(int id)
        {
            var _typeEcommerce = await _service.GetTypeEcommerceByID(id);

            if (_typeEcommerce == null)
            {
                return NotFound($"Type Ecommerce with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _typeEcommerce
            });
        }
    }
}
