using Mango.service.EmailAPI.Models;

namespace Mango.service.EmailAPI
{
    public class CartDTO
    {
        public CartHeaderDTO? CartHeader { get; set; }
        public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }
    }

}
