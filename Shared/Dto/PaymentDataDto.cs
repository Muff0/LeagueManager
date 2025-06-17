namespace Shared.Dto
{
    public class PaymentDataDto
    {
        public DateTime DateTime { get; set; }
        public string BillingName { get; set; } = string.Empty;
        public string BillingEmail { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public double USD { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}