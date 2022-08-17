using System;

namespace AccountTransaction.Model
{
    public class Account
    {
        public int Id { get; set; }
        public Guid referenceId { get; set; }
        public long AccountNr { get; set; }
        public decimal Balance { get; set; }
    }
}
