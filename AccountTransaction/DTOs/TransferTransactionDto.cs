using System;

namespace AccountTransaction.DTOs
{
    public class TransferTransactionDto
    {
        public Guid referenceId { get; set; }
        public long AccountNr { get; set; }
        public long AccountNrTo { get; set; }
        public decimal Amount { get; set; }
    }
}
