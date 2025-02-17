﻿using BackendEPPO.Extenstion;
using DTOs.Error;
using DTOs.Plant;
using DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public PlantController(IPlantService IService, IUserService userService, IConfiguration configuration)
        {
            _plantService = IService;
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetAllPlants(int pageIndex, int pageSize)
        {
            var plants = _plantService.GetAllPlants(pageIndex, pageSize);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetPlantById(int id)
        {
            

            var plant = _plantService.GetPlantById(id);
            if (plant == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = $"Không tìm thấy cây với ID {id}.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plant
            });
        }

 

        /// <summary>
        /// Get list Plant by Category with the page and the size.
        /// </summary>
        /// <returns> Get list Plant by Category with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetPlantByCategory)]
        public IActionResult GetPlantsByCategoryId(int pageIndex, int pageSize, int categoryId)
        {
            var plants = _plantService.GetPlantsByCategoryId(pageIndex, pageSize, categoryId);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào trong danh mục.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }


        /// <summary>
        /// Get list Plant by Type Ecommerce with the page and the size.
        /// </summary>
        /// <returns> Get list Plant by Type Ecommerce with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetListPlantsByTypeEcommerceId)]
        public IActionResult GetListPlantsByTypeEcommerceId(int pageIndex, int pageSize, int typeEcommerceId)
        {
            var plants = _plantService.GetListPlantsByTypeEcommerceId(pageIndex, pageSize, typeEcommerceId);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo loại thương mại điện tử.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }


        /// <summary>
        /// Get list Plant by Type Ecommerce with the page and the size.
        /// </summary>
        /// <returns> Get list Plant by Type Ecommerce with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetListPlantsByTypeEcommerceIdManager)]
        public IActionResult GetListPlantsByTypeEcommerceIdManage(int pageIndex, int pageSize, int typeEcommerceId)
        {
            var plants = _plantService.GetListPlantsByTypeEcommerceIdManage(pageIndex, pageSize, typeEcommerceId);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo loại thương mại điện tử.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }

        /// <summary>
        /// Get list Plant by Type Ecommerce And Category with the page and the size.
        /// </summary>
        /// <returns> Get list Plant by Type Ecommerce And Category with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetListPlantsByTypeEcommerceAndCategory)]
        public IActionResult GetListPlantsByTypeEcommerceAndCategory(int pageIndex, int pageSize, int typeEcommerceId, int categoryId)
        {
            var plants = _plantService.GetListPlantsByTypeEcommerceAndCategory(pageIndex, pageSize, typeEcommerceId, categoryId);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo loại thương mại điện tử và danh mục.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }

        /// <summary>
        /// Search list Plant by key word and Type Ecommerce with the page and the size.
        /// </summary>
        /// <returns>Search list Plant by key word and Type Ecommerce with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.SearchPlantByKeyWord)]
        public async Task<IActionResult> SearchPlantKeyType(int pageIndex, int pageSize, int typeEcommerceId, string keyWord)
        {
            var plants = await _plantService.SearchPlantKeyType(pageIndex, pageSize, typeEcommerceId, keyWord);

            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo từ khóa."
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        
        }

        /// <summary>
        ///  Check Plant in to the cart is new value.
        /// </summary>
        /// <returns>Check Plant in to the cart is new value.</returns>
        [HttpGet(ApiEndPointConstant.Plants.CheckPlantInCart)]
        public async Task<IActionResult> CheckPlantInCart(List<int> plantId)
        {
            var plants = await _plantService.CheckPlantInCart(plantId);

            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo từ khóa."
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }


        [HttpGet("search")]
        public IActionResult SearchPlants(string keyword, int typeEcommerceId, int pageIndex, int pageSize)
        {
            var plants = _plantService.SearchPlants(keyword, typeEcommerceId, pageIndex, pageSize);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo từ khóa.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }

        /// <summary>
        /// Function for mobile: Create plant for owner by role owner.
        /// </summary>
        /// <returns> Function for mobile: Create plant for owner by role owner</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost(ApiEndPointConstant.Plants.CreatePlantByOwner_Endpoint)]
        public async Task<IActionResult> CreatePlantByOwner([FromBody] CreatePlantDTOByOwner plant)
        {

            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _plantService.CreatePlantByOwner(plant, userIdClaim);

            return Ok(new
            {
                StatusCode = 201,
                Message = Error.REQUESR_SUCCESFULL,
                Data = plant
            });
        }

        /// <summary>
        /// Function for mobile: Create plant for owner by role owner.
        /// </summary>
        /// <returns> Function for mobile: Create plant for owner by role owner</returns>
        //[Authorize(Roles = "admin, manager, staff, owner, customer")]
        //[HttpPost(ApiEndPointConstant.Plants.CreatePlantByOwnerToken_Endpoint)]
        //public async Task<IActionResult> CreatePlantDTOTokenOwner([FromForm] CreatePlantDTOTokenOwner createPlant)
        //{
        //    var userIdClaim = User.FindFirst("userId")?.Value;
        //    int userId = int.Parse(userIdClaim);

        //    await _plantService.CreatePlantByOwner(createPlant, createPlant.MainImageFile, createPlant.ImageFiles, userId);
        //    return Ok(new
        //    {
        //        StatusCode = 201,
        //        Message = "Đã tạo cây thành công.",
        //        Data = createPlant
        //    });
        //}

        /// <summary>
        /// Function for mobile: Create plant for owner by role owner.
        /// </summary>
        /// <returns> Function for mobile: Create plant for owner by role owner</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost(ApiEndPointConstant.Plants.CreatePlantByOwnerToken_Endpoint)]
        public async Task<IActionResult> CreatePlant([FromForm] CreatePlantDTO createPlant)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);
            await _plantService.CreatePlantByOwner(createPlant, createPlant.MainImageFile, createPlant.ImageFiles , userId, userIdClaim);
            return Ok(new
            {
                StatusCode = 201,
                Message = "Đã tạo cây thành công.",
                Data = createPlant
            });
        }
        /// <summary>
        /// Function for mobile: Get All Plants To Register.
        /// </summary>
        /// <returns> Function for web: Update plant for manager by role manager.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetListPlantsRegister_Endpoint)]
        public IActionResult GetAllPlantsToResgister(int pageIndex, int pageSize)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            var plants = _plantService.GetAllPlantsToResgister(pageIndex, pageSize, userIdClaim);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }
        /// <summary>
        /// Function for web: Update plant for manager by role manager.
        /// </summary>
        /// <returns> Function for web: Update plant for manager by role manager.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.Plants.UpdatePlantByManager)]
        public async Task<IActionResult> UpdatePlantByManager([FromForm] UpdatePlantDTO updatePlant, int plantId)
        {
            await _plantService.UpdatePlantByManager(updatePlant, plantId,updatePlant.MainImageFile, updatePlant.ImageFiles);
            return Ok(new
            {
                StatusCode = 200,
                Message = "Đã cập nhật cây thành công.",
                Data = updatePlant
            });
        }

        /// <summary>
        /// Function of owner: Get list Plant by Type Ecommerce with the page and the size.
        /// </summary>
        /// <returns> Get list Plant by Type Ecommerce with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetListPlantsOwnerByTypeEcommerceId)]
        public IActionResult GetListPlantOfOwnerByTypeEcommerceId(int pageIndex, int pageSize, int? typeEcommerceId)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            var plants = _plantService.GetListPlantOfOwnerByTypeEcommerceId(pageIndex, pageSize, typeEcommerceId , userIdClaim);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo loại thương mại điện tử.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }
        /// <summary>
        /// Function for web: Update status plant.
        /// </summary>
        /// <returns> Function for web: Update plant for manager by role manager.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.Plants.UpdateStatusPlant)]
        public async Task<IActionResult> UpdateStauts([FromBody] UpdatePlantStatus plant, int plantId)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _plantService.UpdatePlantStatus(plant, plantId);

            return Ok(new
            {
                StatusCode = 201,
                Message = Error.REQUESR_SUCCESFULL,
                Data = plant
            });
        }

        /// <summary>
        /// Function for web: Update plant Id for manager by role manager.
        /// </summary>
        /// <returns> Function for web: Update plant for manager by role manager.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.Plants.UpdatePlantIdByManager)]
        public async Task<IActionResult> UpdatePlantIdByManager([FromForm] UpdatePlantIdDTO updatePlant, int plantId)
        {
            await _plantService.UpdatePlantIdByManager(updatePlant, plantId, updatePlant.MainImageFile);
            return Ok(new
            {
                StatusCode = 200,
                Message = "Đã cập nhật cây thành công.",
                Data = updatePlant
            });
        }

        /// <summary>
        /// Function for mobile: View All Plants To accept for manager.
        /// </summary>
        /// <returns> Function for mobile: View All Plants To accept for manager.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Plants.ViewPlantsToAccept)]
        public IActionResult ViewPlantsToAccept(int pageIndex, int pageSize, int typeEcommerceId)
        {
            var code = User.FindFirst("userId")?.Value;
      
            var plants = _plantService.ViewPlantsToAccept(pageIndex, pageSize, code, typeEcommerceId);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }

        /// <summary>
        /// Function for mobile: View All Plants Wait to accept for manager.
        /// </summary>
        /// <returns> Function for mobile: View All Plants To accept for manager.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Plants.ViewPlantsWaitAccept)]
        public IActionResult ViewPlantsWaitAccept(int pageIndex, int pageSize)
        {
            var code = User.FindFirst("userId")?.Value;

            var plants = _plantService.ViewPlantsWaitAccept(pageIndex, pageSize, code);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }

        /// <summary>
        /// Function for mobile: View All Plants Un to accept for manager.
        /// </summary>
        /// <returns> Function for mobile: View All Plants To accept for manager.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Plants.ViewPlantsUnAccept)]
        public IActionResult ViewPlantsUnAccept(int pageIndex, int pageSize)
        {
            var code = User.FindFirst("userId")?.Value;

            var plants = _plantService.ViewPlantsUnAccept(pageIndex, pageSize, code);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }

        /// <summary>
        /// Function for mobile: Cancel contract plant.
        /// </summary>
        /// <returns> Cancel contract plant.</returns>
        [Authorize(Roles = "admin, manager, staff, owner")]
        [HttpPut(ApiEndPointConstant.Plants.CancelContractPlant)]
        public async Task<IActionResult> CancelContractPlant(int plantId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            try
            {
                var result = await _plantService.CancelContractPlant(plantId);

                if (!result)
                {
                    return Conflict(new
                    {
                        StatusCode = 409,
                        Message = $"Plant with ID {plantId} is already inactive."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Plant contract canceled successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error: {ex.Message}");

                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        /// <summary>
        /// Search list Plant by plantID and Type Ecommerce with the page and the size.
        /// </summary>
        /// <returns>Search list Plant by key word and Type Ecommerce with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.SearchPlantID)]
        public async Task<IActionResult> SearchPlantIDKey(int pageIndex, int pageSize, int typeEcommerceId, string keyWord)
        {
            var plants = await _plantService.SearchPlantIDKey(pageIndex, pageSize, typeEcommerceId, keyWord);

            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo từ khóa."
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });

        }

        /// <summary>
        /// Function for mobile: Get All Plants sale of the owner for customer buy or rental.
        /// </summary>
        /// <returns>Function for mobile: Get All Plants of the owner for customer buy or rental.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetAllPlantsSaleOfOwner)]
        public IActionResult GetAllPlantsSaleOfOwner(int pageIndex, int pageSize, string code)
        {
            try
            {
                if (!int.TryParse(code, out int userId))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = $"Mã code không hợp lệ: {code}",
                        Data = (object)null
                    });
                }
                var owner = _userService.GetUsersByID(userId);
                if (owner == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"Không tìm thấy người dùng với UserId: {userId}",
                        Data = (object)null
                    });
                }
                var plants = _plantService.GetAllPlantsSaleOfOwner(pageIndex, pageSize, code);
                if (plants == null || !plants.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy cây nào.",
                        Owner = new
                        {
                            FullName = owner.Result.FullName,
                            PhoneNumber = owner.Result.PhoneNumber,
                            ImageUrl = owner.Result.ImageUrl
                        },
                        Data = (object)null
                    });
                }
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Yêu cầu thành công.",
                    Owner = new
                    {
                        FullName = owner.Result.FullName,
                        PhoneNumber = owner.Result.PhoneNumber,
                        ImageUrl = owner.Result.ImageUrl
                    },
                    Data = plants
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi khi xử lý yêu cầu.",
                    Details = ex.Message
                });
            }
        }


        /// <summary>
        /// Function for mobile: Get All Plants Rental of the owner for customer buy or rental.
        /// </summary>
        /// <returns>Function for mobile: Get All Plants of the owner for customer buy or rental.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetAllPlantsRentalOfOwner)]
        public IActionResult GetAllPlantsRentalOfOwner(int pageIndex, int pageSize, string code)
        {
            try
            {
                if (!int.TryParse(code, out int userId))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = $"Mã code không hợp lệ: {code}",
                        Data = (object)null
                    });
                }
                var owner = _userService.GetUsersByID(userId);
                if (owner == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"Không tìm thấy người dùng với UserId: {userId}",
                        Data = (object)null
                    });
                }
                var plants = _plantService.GetAllPlantsRentalOfOwner(pageIndex, pageSize, code);
                if (plants == null || !plants.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy cây nào.",
                        Owner = new
                        {
                            FullName = owner.Result.FullName,
                            PhoneNumber = owner.Result.PhoneNumber,
                            ImageUrl = owner.Result.ImageUrl
                        },
                        Data = (object)null
                    });
                }
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Yêu cầu thành công.",
                    Owner = new
                    {
                        FullName = owner.Result.FullName,
                        PhoneNumber = owner.Result.PhoneNumber,
                        ImageUrl = owner.Result.ImageUrl
                    },
                    Data = plants
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi khi xử lý yêu cầu.",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Function for mobile: Get Deposit Rental by plant Id.
        /// </summary>
        /// <returns>Function for mobile: Get All Plants of the owner for customer buy or rental.</returns>
        [HttpGet(ApiEndPointConstant.Plants.CalculateDepositRental)]
        [Authorize(Roles = "admin, manager, customer")]
        public IActionResult CalculateDepositRental(int plantId)
        {
            try
            {
                double result = _plantService.CalculateDepositRental(plantId).Result;

                if (result == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.BAD_REQUEST,
                        Data = 0
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = Error.NO_DATA_FOUND + ex.Message,
                    Data = (object)null
                });
            }
        }

    }
}
