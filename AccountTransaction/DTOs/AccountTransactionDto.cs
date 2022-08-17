using System;

namespace AccountTransaction.DTOs
{
    public class DepositWithdrawalDto
    {
        public Guid referenceId { get; set; }
        public long AccountNr { get; set; }
        public decimal Amount { get; set; }
        public Data.TransactionType TransactionType { get; set; }
    }
}
