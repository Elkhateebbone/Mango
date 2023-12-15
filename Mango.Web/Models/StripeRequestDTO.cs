using Mango.Web.Models.DTO;

namespace Mango.Web.Models
{
    public class StripeRequestDTO
    {
        public string? StripeSessionUrl { get; set; }
         public string ApprovalUrl { get; set; }

        public string? StripeSessionId { get; set; }
        public string CancelUrl { get; set; }
        public OrderHeaderDTO orderHeader { get; set; }
    }
}
