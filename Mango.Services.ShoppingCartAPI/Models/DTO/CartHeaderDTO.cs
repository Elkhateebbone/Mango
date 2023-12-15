using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models.DTO
{
    public class CartHeaderDTO
    {

        [Key]
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? Couponcode { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; }

        public string? Name { get; set; }
 
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
