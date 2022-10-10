using AccountTransaction.Data;
using Newtonsoft.Json.Converters;
using System;
using System.Text.Json.Serialization;

namespace AccountTransaction.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UniqueId { get; set; }
        public Guid referenceId { get; set; }
        public long AccountNr { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Payload { get; set; }
    }
}
