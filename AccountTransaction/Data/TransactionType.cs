using System.ComponentModel;

namespace AccountTransaction.Data
{
    public enum TransactionType
    {
        [Description("None")]
        None = 0,
        [Description("Withdrawal")]
        Withdrawal = 1,
        [Description("Deposit")]
        Deposit = 2,
    }
}
