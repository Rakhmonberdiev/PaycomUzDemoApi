namespace PaycomUzDemoApi.Models
{
    public class Order
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public bool IsPaid { get; set; }
        public int Amount { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PerformedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public int? Reason { get; set; }
        public int State { get; set; }
    }
}
