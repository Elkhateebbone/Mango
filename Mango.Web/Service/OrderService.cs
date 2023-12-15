using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO> CreateOrder(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                Url = SD.OrderAPI + "/api/order/CreatedOrder"
            });
        }

        public async Task<ResponseDTO> CreateStripeSession(StripeRequestDTO stripeRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = stripeRequestDTO,
                Url = SD.OrderAPI + "/api/order/CreateStripeSession"
            });
        }

         public async Task<ResponseDTO> GetAllOrder(string? userId)
            {
                return await _baseService.SendAsync(new RequestDTO()
                {
                    ApiType = SD.ApiType.GET,
                
                    Url = SD.OrderAPI + "/api/order/GetOrders?userId=" + userId
                });
            }

        public async Task<ResponseDTO> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPI + "/api/order/GetOrder/" + orderId
            });
        }

        public async Task<ResponseDTO> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = newStatus,
                Url = SD.OrderAPI + "/api/order/UpdaetOrderStatus?orderId=" + orderId
            });
        }

        public async Task<ResponseDTO> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = orderHeaderId,
                Url = SD.OrderAPI + "/api/order/ValidateStripeSession"
            });
        }
    }                                                                                                               
}
