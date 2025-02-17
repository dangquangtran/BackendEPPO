﻿using BackendEPPO.Extenstion;
using DTOs.TypeEcommerce;
using DTOs.Wallet;
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

        /// <summary>
        /// Get list all Type Ecommerce in database with the page and the size.
        /// </summary>
        /// <returns>Get list all Type Ecommerce in database with the page and the size.</returns>
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

        /// <summary>
        /// Get Type Ecommerce by Type Ecommerce Id with all role.
        /// </summary>
        /// <returns>Get Type Ecommerce by Type Ecommerce Id with all role.</returns>
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

        /// <summary>
        /// Create Type Ecommerce with role manager and staff.
        /// </summary>
        /// <returns>Create Type Ecommerce with role manager and staff.</returns>
        [Authorize(Roles = "admin, manager, staff")]
        [HttpPost(ApiEndPointConstant.TypeEcommerce.CreateTypeEcommerce)]
        public async Task<IActionResult> CreateTypeEcommerce([FromBody] CreateTypeEcommerceDTO typeEcommerce)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.CreateTypeEcommerce(typeEcommerce);

            return Ok(new
            {
                StatusCode = 201,
                Message = "TypeEcommerce created successfully",
                Data = typeEcommerce
            });
        }

        /// <summary>
        /// Update Type Ecommerce with role manager and staff.
        /// </summary>
        /// <returns>Update Type Ecommerce with role manager and staff.</returns>
        [Authorize(Roles = "admin, manager, staff")]
        [HttpPut(ApiEndPointConstant.TypeEcommerce.UpdateTypeEcommerceID)]
        public async Task<IActionResult> UpdateTypeEcommerce(int id, [FromBody] UpdateTypeEcommerceDTO typeEcommerce)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            typeEcommerce.TypeEcommerceId = id;

            try
            {
                await _service.UpdateTypeEcommerce(typeEcommerce);
                var updatedTypeEcommerce = await _service.GetTypeEcommerceByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "TypeEcommerce updated successfully.",
                    Data = updatedTypeEcommerce
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "TypeEcommerce not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
    }
}
