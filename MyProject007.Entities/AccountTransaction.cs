using System;

namespace MyProject007.Entities
{
    public class AccountTransaction
    {
        public long TransactionId { get; set; }
        public int SenderUserId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}