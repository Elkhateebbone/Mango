namespace Mango.Services.CouponAPI.Models.DTO
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public int DiscountAmount { get; set; }

        public int MinAmount { get; set; }
    }
}
