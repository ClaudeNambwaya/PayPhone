namespace PayPhone.Models
{
    public class MessageModel
    {
        public int Id { get; set; }

        public int Sender { get; set; }

        public string? Content { get; set; }

        public int Receiver { get; set; }

        public int Chat { get; set; }

        // To represent JSON in a C# object
        public List<int> ReadBy { get; set; } = new List<int>();

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
