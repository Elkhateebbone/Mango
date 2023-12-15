using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductsService _productService;

        public ProductController(IProductsService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
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

		public async Task<IActionResult> ProductCreate()
		{
			return View();
		}
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDTO model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Log or inspect the error message
                    var errorMessage = error.ErrorMessage;
                }
            }

            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productService.CreateProductAsync(model);
                if(response != null && response.IsSuccess)
                {
                    TempData["success"] = "Added Successfully";

                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> ProductDelete(int id)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productService.GetProductByIdAsync(id);
                if (response != null && response.IsSuccess)
                {
                   ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
                    return View (model);
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> Delete(ProductDTO model)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productService.DeleteProductAsync(model.ProductId);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }
        
        public async Task<IActionResult> ProductEdit(int productId)
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
        [HttpPost]
        public async Task<IActionResult> Edit(ProductDTO model)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productService.UpdateProductAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product Updated Successfully";

                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;

                }
            }
            
            return View(model);
        }
     
    }
   
}



  
