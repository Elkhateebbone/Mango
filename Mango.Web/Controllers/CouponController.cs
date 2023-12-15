using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly IProductService _couponService;
        public CouponController(IProductService couponService)
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto> list = new();
            ResponseDTO? response = await _couponService.GetAllCouponsAsync();
            if (response != null && response.IsSuccess) 
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));

            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View(list);
        }

		public async Task<IActionResult> CouponCreate()
		{
			return View();
		}
        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto model)
        {
            if(ModelState.IsValid)
            {
                ResponseDTO? response = await _couponService.CreateCouponAsync(model);
                if(response != null && response.IsSuccess)
                {
                    TempData["success"] = "Added Successfully";

                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> CouponDelete(int id)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _couponService.GetCouponByIdAsync(id);
                if (response != null && response.IsSuccess)
                {
                   CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                    return View (model);
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> Delete(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _couponService.DeleteCouponAsync(model.CouponId);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }
            return View(model);
        }
    }

  
}
