namespace Mango.Web.Models.DTO
{
    public class OrderHeaderDTO
    {
        public int OrderHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? Couponcode { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime OrderTime { get; set; }
        public IEnumerable<OrderDetailsDTO> orderDetails { get; set; }
        public string? Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? StripSessionId { get; set; }
    }
}
