using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Service;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDTO _response;
        private IMapper _mapper;
        private readonly IMessageBus _bus;
        private readonly IConfiguration _configuraton;
        private readonly AppDbContext _db;
        private readonly ICouponService _couponService;
        private readonly IProductService _productService;
        public CartAPIController(AppDbContext db,IConfiguration configuration, IMapper mapper,IProductService productService, ICouponService couponService,IMessageBus bus)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDTO();
            _productService = productService;
            _couponService = couponService;
            _configuraton = configuration;
            _bus = bus;
        }
        [HttpPost("EmailCartRequest")]
        public async Task<ResponseDTO> EmailCartRequest([FromBody] CartDTO cartDTO)
        {
            try
            {
                await _bus.PublishMessage(cartDTO, _configuraton.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                _response.Result = true;


            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString(); 
            }
            return _response;
        }
        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDTO> ApplyCoupon([FromBody] CartDTO cartDTO)
        {
            try
            {
                var cartFromDb = await _db.cartHeaders.FirstAsync(u => u.UserId == cartDTO.CartHeader.UserId);
                cartFromDb.Couponcode = cartDTO.CartHeader.Couponcode;
                _db.cartHeaders.Update(cartFromDb); await _db.SaveChangesAsync();
                 _response.Result = true;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }
        [HttpPost("RemoveCoupon")]
        public async Task<ResponseDTO> RemoveCoupon([FromBody] CartDTO cartDTO)
        {
            try
            {
                var cartFromDb = await _db.cartHeaders.FirstAsync(u => u.UserId == cartDTO.CartHeader.UserId);
                cartFromDb.Couponcode = "";
                _db.cartHeaders.Update(cartFromDb); await _db.SaveChangesAsync();
                _response.Result = true;

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }


        [HttpGet("Getcart/{userId}")]
        public async Task<ResponseDTO> Getcart(string userId)
        {
            try
            {
                IEnumerable<ProductDTO> productDTOs = await _productService.GetProducts();

                CartDTO cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(_db.cartHeaders.First(u => u.UserId == userId)),
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>>(_db.cartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId).ToList());

                    foreach(var item  in cart.CartDetails)
                       {

                          item.Product = productDTOs.FirstOrDefault(u => u.ProductId == item.ProductId);
                          item.CartHeader.CartTotal = item.Count * item.Product.Price;
                    cart.CartHeader.CartTotal += item.CartHeader.CartTotal;
                       }
                    if(!string.IsNullOrEmpty(cart.CartHeader.Couponcode))
                       {
                    CouponDto coupon = await _couponService.GetCoupon(cart.CartHeader.Couponcode);
                    if(coupon != null && cart.CartHeader.CartTotal>coupon.MinAmount) 
                    {

                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount   = coupon.DiscountAmount;
                    }

                  }
                _response.Result = cart;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = true;


                _response.Message = ex.Message;


            }
            return _response;
        }



            [HttpPost("CartUppsertt")]
        public async Task<ResponseDTO> Upsert(CartDTO cartDTO)
        {
            try
            {
                var cartHeaderFromDb = _db.cartHeaders.FirstOrDefault(a => a.UserId == cartDTO.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //Created header
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.CartHeader);
                    _db.cartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;

                    _db.cartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if header not null check if details has same product
                    var cartDetailsFromDb = await _db.cartDetails.AsNoTracking().FirstOrDefaultAsync(a => a.ProductId == cartDTO.CartDetails.First().ProductId && a.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                       
                        cartDTO.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.cartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //update count 

                        cartDTO.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDTO.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        cartDTO.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _db.cartDetails.Update(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    _response.Result = cartDTO;
                }

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDTO> RemoveCart([FromBody] int cartDetailsId)
        {

            try
            {
                CartDetails cartDetails = _db.cartDetails.
                    First(u => u.CartDetailsId == cartDetailsId);
                int totalCountofCartItem = _db.cartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.cartDetails.Remove(cartDetails);
                if (totalCountofCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.cartHeaders.
                        FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _db.cartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                _response.Result = true;


            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
} 
    
