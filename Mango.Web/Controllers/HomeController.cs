using IdentityModel;
using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{

    public class HomeController : Controller
    {
        private readonly IProductsService _productService;
        private readonly ICartService _cartService;
        public HomeController(IProductsService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }
        [HttpPost]
        [Authorize]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> Details(ProductDTO productDTO)
        {
           
                CartDTO cartDTO = new CartDTO()
                {
                    CartHeader = new CartHeaderDTO
                    {
                        UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                    }
                };
                CartDetailsDTO cartDetails = new CartDetailsDTO()
                {
                    Count = productDTO.Count,
                    ProductId = productDTO.ProductId,

                };
                List<CartDetailsDTO> cartDetailsDTOs = new() { cartDetails };
                cartDTO.CartDetails = cartDetailsDTOs;
                ResponseDTO? response = await _cartService.UpsertCartAsync(cartDTO);
                
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Item Added to the Shopping cart successfully"; 
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            
            return View(productDTO);
        }
        public async Task<IActionResult> Index()
        {
            List<ProductDTO> list = new();
            ResponseDTO? response = await _productService.GetAllProductAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));

            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View(list);
        }

        public async Task<IActionResult> ProductDetails(int productId)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productService.GetProductByIdAsync(productId);
                if (response != null && response.IsSuccess)
                {
                    ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
                    return View(model);
                }
                else
                {
                    TempData["error"] = response.Message;

                }
            }

            return NotFound();
        }
    }
}
