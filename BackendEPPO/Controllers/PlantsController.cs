﻿using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantsController : ControllerBase
    {
        private readonly IPlantService _plantsService;

        public PlantsController(IPlantService IService)
        {
            _plantsService = IService;
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Plants.GetListPlants_Endpoint)]
        public async Task<IActionResult> GetListPlants(int page, int size)
        {
            var _plant = await _plantsService.GetListPlants(page, size);

            if (_plant == null || !_plant.Any())
            {
                return NotFound("No contract found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _plant
            });
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Plants.GetPlantByID)]
        public async Task<IActionResult> GetPlantByID(int id)
        {
            var plant = await _plantsService.GetPlantByID(id); 

            if (plant == null)
            {
                return NotFound($"Plant with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = plant
            });
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Plants.GetPlantByCategory)]
        public async Task<IActionResult> GetListPlantsByCategory(int Id)
        {
            var _plant = await _plantsService.GetListPlantByCategory(Id);

            if (_plant == null || !_plant.Any())
            {
                return NotFound("No contract found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _plant
            });
        }
    }
}
