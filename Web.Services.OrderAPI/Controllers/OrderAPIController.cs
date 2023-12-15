using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using Mango.MessageBus;
using Microsoft.EntityFrameworkCore;


namespace Mango.Services.OrderAPI.Controllers
{

    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDTO _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProductService _productService;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;


        public OrderAPIController(AppDbContext db,
            IProductService productService,
            IMapper mapper,IConfiguration configuration,IMessageBus messageBus)
        {
            _db = db;
            _mapper = mapper;
            _productService = productService;
            this._response = new ResponseDTO();
            _configuration = configuration;
            _messageBus = messageBus;
        }
        [Authorize]
        [HttpPost("CreatedOrder")]
        public async Task<ResponseDTO> CreateOrder(CartDTO cartDTO)
        {
            try
            {
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cartDTO.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = SD.Status_Pending;

                // Check if cartDTO.CartDetails is not null before mapping
                if (cartDTO.CartDetails != null && cartDTO.CartDetails.Any())
                {
                    orderHeaderDTO.orderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDTO.CartDetails);

                }
                else
                {
                    // Handle the case where CartDetails is null or empty, depending on your requirements.
                    // For example, you could set a default value or throw an exception.
                    // orderHeaderDTO.orderDetails = new List<OrderDetailsDTO>();
                }

                OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDTO)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDTO.OrderHeaderId = orderCreated.OrderHeaderId;

                _response.Result = orderHeaderDTO;
            }
            // Handle exceptions and return appropriate error response if needed
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51NT5bXKDwnJyVREWiUkWznt9DLBU6I2XfZpVQeiHheNXo26OomfRiSbd1ziKAUbPjHUMxWfobO9eRv6JB7Bfpoxt00xPEYzWNU";

                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovalUrl,
                    CancelUrl = stripeRequestDTO.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in stripeRequestDTO.orderHeader.orderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.99 ->2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,

                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }


                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDTO.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDTO.orderHeader.OrderHeaderId);
                orderHeader.StripSessionId = session.Id;
                _db.SaveChanges();
                _response.Result = stripeRequestDTO;


            }

            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;


        }


        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDTO> ValidateStripeSession([FromBody] int orderHeaderId)
        {

            OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

            var service = new SessionService();
            Session session = service.Get(orderHeader.StripSessionId);


            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);
            if (paymentIntent.Status == "succeeded")
            {
                //then payment was successful 
                orderHeader.PaymentIntentId = paymentIntent.Id;
                orderHeader.Status = SD.Status_Approved;
                _db.SaveChanges();

                RewardsDTO rewardsDTO = new()
                {
                    OrderId = orderHeader.OrderHeaderId,
                    RewardsActivity = Convert.ToInt32(orderHeader.CartTotal),
                    UserId = orderHeader.UserId
                };
                string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                await _messageBus.PublishMessage(rewardsDTO, topicName);
                _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);  
            }
            _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);



         

            return _response;


        }

        [HttpGet("GetOrders")]  
        public ResponseDTO? Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> objList;
                IEnumerable<OrderHeaderDTO> objList1;

                if (User.IsInRole(SD.RoleAdmin))
                {
                    objList = _db.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(u=>u.OrderHeaderId).ToList();
                    objList1 = _mapper.Map<IEnumerable<OrderHeaderDTO>>(objList);
                }
                else
                {
                    objList = _db.OrderHeaders.Include(u => u.OrderDetails).Where(u=>u.UserId==userId).OrderByDescending(u => u.OrderHeaderId).ToList();
                    objList1 = _mapper.Map<IEnumerable<OrderHeaderDTO>>(objList);

                }
                _response.Result = objList1;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDTO? Get(int id )
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.Include(u=>u.OrderDetails).First(u=>u.OrderHeaderId==id);
                _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [Authorize]
        [HttpPost("UpdaetOrderStatus")]
        public async Task<ResponseDTO> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(a=>a.OrderHeaderId==orderId);
                if(orderHeader !=null)
                {
                    if(newStatus == SD.Status_Cancelled)
                    {
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId,
                        };
                            var service = new RefundService();
                        Refund refund = service.Create(options);
                        orderHeader.Status = newStatus;
                    }
                    orderHeader.Status = newStatus;
                    _db.SaveChanges();
                    _db.SaveChanges();
                }
              
            }
            catch (Exception ex)
            {

                _response.IsSuccess=false;
                _response.Message = ex.Message;
            }
            return _response;
        }

    }


   
}

