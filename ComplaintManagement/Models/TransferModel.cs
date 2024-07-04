namespace PayPhone.Models
{
    public class TransferModel
    {
        public string? SenderEmail { get; set; }
        public string? ReceiverEmail { get; set; }
        public string? ReceiverName { get; set; }
        public string? SenderName { get; set; }
        public string? ReceiverPassword { get; set; }
        public decimal Amount { get; set; }
    }
}
