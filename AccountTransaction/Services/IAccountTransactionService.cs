using System;
using System.Threading.Tasks;

namespace AccountTransaction.Services
{
    public interface IAccountTransactionService
    {
        Task DepositWithDrawal(Guid referenceId, long accountNr, decimal amount);

        Task Transfer(Guid referenceId, long accountNrFrom, long AccountNrTo, decimal amount);
    }
}
