namespace PayPhone.Models
{
    public class ChatModel
    {
        public Int32 id { get; set; }
        public string? chatName { get; set; }
        public string? sender { get; set; }
        public string? receiver { get; set; }
        public string? latestMessage { get; set; }  
        public DateTime created_on { get; set; } 
        public DateTime updated_at { get; set;}
    }
}
