using Azure;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        protected ResponseDTO _response;


        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._response = new ResponseDTO();

        }

        
            [HttpPost("OrderReadyForPickup")]
            public async Task<IActionResult> OrderReadyForPickup(int orderId)
            {
            var response = await _orderService.UpdateOrderStatus(orderId,SD.Status_ReadyForPickUp);
            if(response != null && response.IsSuccess) 
            {
                TempData["success"] = "Status Updated Successfully ";
                return RedirectToAction(nameof(orderDetails), new {orderId});
            }
            return View();

            }
        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Completed);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status Updated Successfully ";
                return RedirectToAction(nameof(orderDetails), new { orderId });
            }
            return View();

        }
        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status Updated Successfully ";
                return RedirectToAction(nameof(orderDetails), new { orderId });
            }
            return View();

        }


        [HttpGet]
        public async Task<IActionResult> orderDetails(int orderId)
        {
            OrderHeaderDTO orderHeaderDTO = new OrderHeaderDTO();
           string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            var response = await _orderService.GetOrder( orderId);
            if(response != null && response.IsSuccess)
            {

                orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));

            }
            if (!User.IsInRole(SD.RoleAdmin) && userId !=orderHeaderDTO.UserId)
            {
                return NotFound();
            }
            return View(orderHeaderDTO);

        }


        [HttpGet]
        public  JsonResult GetAll(string status) {

            try
            {
                IEnumerable<OrderHeaderDTO> list;


                string userId = "";

                if (!User.IsInRole(SD.RoleAdmin))
                {
                    userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;

                }
                var response = _orderService.GetAllOrder(userId).Result;
                _logger.LogInformation($"Response IsSuccess: {response?.IsSuccess}, Message: {response?.Message}");

                if (response != null && response.IsSuccess)
                {


                    list = JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(Convert.ToString(response.Result));


                    if (status == "approved")
                    {
                        list = list.Where(u=>u.Status==SD.Status_Approved).ToList();
                    }
                    else if(status == "ReadyForPickup")
                    {
                        list = list.Where(u => u.Status == SD.Status_ReadyForPickUp).ToList();

                    }else if(status == "Cancelled")
                    {
                        list = list.Where(u => u.Status == SD.Status_Cancelled).ToList();
                    }


                }
                else
                {
                    _logger.LogError($"Error retrieving orders: {response.Message}");

                    list = new List<OrderHeaderDTO>();
                }
                return Json(new { data =  list});

            }


            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = $"Error in GetAllOrder: {ex.Message}";
                // Log the exception
                _logger.LogError(ex, _response.Message);
                _response.Message = $"Error in GetAllOrder: {ex.Message}";

                // Log the exception at various levels
                _logger.LogError(ex, _response.Message); // Log at the error level
                _logger.LogInformation(ex, _response.Message); // Log at the info level
                _logger.LogWarning(ex, _response.Message); 
                        var  list = new List<OrderHeaderDTO>();

                return Json(new { data = list });

            }

        }
        public IActionResult OrderIndex(string status = "")
        {
            return View();
            
        }
    }
}
