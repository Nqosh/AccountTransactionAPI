using AccountTransaction.Data;
using AccountTransaction.Model;
using Customer_API.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountTransaction.Services
{
    public interface IAccountTransactionService
    {
        Task<bool> DepositWithDrawal(Guid referenceId, long accountNr, decimal amount, Account account);
        Task<bool> Transfer(Guid referenceId, long accountNrFrom, long AccountNrTo, decimal amount);
        Task<bool> CreateAccount(CreateAccountDto account);
        Task<List<Transaction>> GetAccountTransactions(long accountNr);
        Task<decimal> GetAccountBalance(long accountNr);
        Task<Account> GetAccount(long accountNr);
    }
}
