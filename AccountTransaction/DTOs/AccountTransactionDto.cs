using Newtonsoft.Json.Converters;
using System;
using System.Text.Json.Serialization;

namespace AccountTransaction.DTOs
{
    public class DepositWithdrawalDto
    {
        public Guid referenceId { get; set; }
        public long AccountNr { get; set; }
        public decimal Amount { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Data.TransactionType TransactionType { get; set; }
    }
}
