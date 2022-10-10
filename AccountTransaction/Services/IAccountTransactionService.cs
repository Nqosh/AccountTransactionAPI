using AccountTransaction.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountTransaction.Services
{
    public interface IAccountTransactionService
    {
        Task<bool> DepositWithDrawal(Guid referenceId, long accountNr, decimal amount);
        Task<bool> Transfer(Guid referenceId, long accountNrFrom, long AccountNrTo, decimal amount);
        Task<bool> CreateAccount(Account account);
        Task<List<Transaction>> GetAccountTransactions(long accountNr);
    }
}
