namespace PayPhone.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }

        public TrnxType TrnxType { get; set; }

        public string? Purpose { get; set; }

        public decimal Amount { get; set; }

        public string? Username { get; set; }

        public string? Reference { get; set; }

        public decimal BalanceBefore { get; set; }

        public decimal BalanceAfter { get; set; }

        public string? FullNameTransactionEntity { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public enum TrnxType
    {
        Credit,
        Debit
    }
}

