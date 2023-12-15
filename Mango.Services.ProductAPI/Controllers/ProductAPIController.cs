using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using static System.Net.WebRequestMethods;

namespace Mango.Services.ProductAPI.Controllers
{

    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDTO _response;
        private readonly IMapper _mapper;
        public ProductAPIController(AppDbContext db,IMapper mapper)
        {
            _db = db;
            _response = new ResponseDTO();
            _mapper = mapper;
        }
        [HttpGet]
        public ResponseDTO Get()
        {
            try { 
            IEnumerable<Product> objList = _db.products.ToList();
            _response.Result =_mapper.Map<IEnumerable<ProductDTO>>(objList);
            }
            catch (Exception ex) { 
            _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpGet]
        [Route("{id:int}")]
        public ResponseDTO Get(int id)
        {
            try { 
            Product obj = _db.products.First(a=>a.ProductId == id);

                _response.Result = _mapper.Map<ProductDTO>(obj);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message; 
            }
            return _response; 
        }

        [HttpPost]
        [Authorize(Roles="ADMIN")]
        
        public ResponseDTO Post([FromBody] ProductDTO productDTO )
        {
            try
            {
                Product obj = _mapper.Map<Product>(productDTO);
                 _db.products.Add(obj);
                _db.SaveChanges();

                if (productDTO.Image != null)
                {
                    string fileName = obj.ProductId +Path.GetExtension(productDTO.Image.FileName);
                    string filePath = @"wwwroot\ProductImages";
                        var  filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(),filePath);
                    using (var filestream  = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        productDTO.Image.CopyTo(filestream);
                        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                        obj.ImageUrl = baseUrl+ "/ProductImages/" + filePath;
                        obj.ImageLocalPath = filePath;

                    }

                }
                else
                {
                    obj.ImageUrl = "https://placehold.co/600X700";
                }
                _db.products.Update(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<Product>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        [Authorize(Roles ="ADMIN")]
        public ResponseDTO Put(ProductDTO couponDto)
        {
            try
            {
                Product obj = _mapper.Map<Product>(couponDto);
                    _db.products.Update(obj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<ProductDTO>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles="ADMIN")]
        public ResponseDTO Delete(int id)
        {
            try
            {
                Product obj = _db.products.Find(id);

                _db.products.Remove(obj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<ProductDTO>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

    }
}
