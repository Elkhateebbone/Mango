using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {

        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault().Value;
            ResponseDTO? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Cart Deleted Successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Applycoupon(CartDTO cartDTO)
        {
            ResponseDTO? response = await _cartService.ApplyCouponAsync(cartDTO);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Cart Updated Successfully";
                return RedirectToAction(nameof(CartIndex));

            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EmailCartRequest(CartDTO cartDTO)
        {

            // Log ModelState errors for debugging
            CartDTO cart = await LoadcartDtoBasedOnLoggedInUser();
            cart.CartHeader.Email= User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault().Value;

            ResponseDTO? response = await _cartService.CartEmail(cart);
                if (response != null && response.IsSuccess)
                {

                    TempData["Success"] = "Email will processed and sent shortly.";

                   
                    return RedirectToAction(nameof(CartIndex));


                
            }
            return View();

        }

        public async Task<IActionResult> RemoveCoupon(CartDTO cartDTO)
        {
            cartDTO.CartHeader.Couponcode = "";

            ResponseDTO? response = await _cartService.ApplyCouponAsync(cartDTO);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Cart Removed Successfully";
                return RedirectToAction(nameof(CartIndex));

            }
            return View();
        }


        [Authorize]
        public async  Task<IActionResult> CartIndex()
        {
            return View(await LoadcartDtoBasedOnLoggedInUser());
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadcartDtoBasedOnLoggedInUser());
        }
        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDTO cartDTO)
        {
            CartDTO cart = await LoadcartDtoBasedOnLoggedInUser();
            cart.CartHeader.Phone = cartDTO.CartHeader.Phone;
            cart.CartHeader.Email = cartDTO.CartHeader.Email;
            cart.CartHeader.Name = cartDTO.CartHeader.Name;
            var response = await _orderService.CreateOrder(cart);
            OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));

           if (response !=null && response.IsSuccess)
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                //get stripe 
                StripeRequestDTO stripeRequestDTO = new()
                {
                    ApprovalUrl = domain + "cart/confirmation?orderId="+orderHeaderDTO.OrderHeaderId,
                    CancelUrl = domain + "cart/checkout",
                    orderHeader = orderHeaderDTO,
                };
                var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDTO);
                StripeRequestDTO stripeRequestResult= JsonConvert.DeserializeObject<StripeRequestDTO>(Convert.ToString(stripeResponse.Result));
                Response.Headers.Add("Location", stripeRequestResult?.StripeSessionUrl);
                return new StatusCodeResult(303);


            }
            return View(cart);
        }
        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDTO response = await _orderService.ValidateStripeSession(orderId);
            if(response !=null && response.IsSuccess)
            {

                OrderHeaderDTO orderHeaderDTO  = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
                if(orderHeaderDTO.Status == SD.Status_Approved)
                {
                    return View(orderId);
                }

                //redirect to some error page based Status
                return RedirectToAction(nameof(CartIndex));
                
            }

            return View(orderId);
        }
        public async Task<CartDTO> LoadcartDtoBasedOnLoggedInUser()
            {
            var userId = User.Claims.Where(u=>u.Type==JwtRegisteredClaimNames.Sub)?.FirstOrDefault().Value;
            ResponseDTO? response = await _cartService.GetCartByUserIdAsync(userId);

            if (response !=null && response.IsSuccess)
            {


                CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));

                return cartDTO;

            }
            return new CartDTO();
        }
    }
}
