using AccountTransaction.Data;
using AccountTransaction.Model;
using Customer_API.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountTransaction.Services
{
    public class AccountTransactionService : IAccountTransactionService
    {
        private readonly AccountTransactionDbContext _context;
        private readonly ILogger _logger;
        public AccountTransactionService(AccountTransactionDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<bool> DepositWithDrawal(Guid referenceId, long accountNr, decimal amount, Account account)
        {
            _logger.LogInformation("Checking duplicate transaction");

            bool isDuplicate = CheckDuplicateTransaction(referenceId, amount);

            if (!isDuplicate)
            {
                _logger.LogInformation("Getting account from database");

      

                switch (Helpers.TransactionType.Type)
                {
                    case TransactionType.Deposit:

                        _logger.LogInformation("Entering the deposit transaction process");
                        
                            if (amount > 0)
                            {
                                account.Balance += amount;

                                _logger.LogInformation("Adding transaction for an additional deposit transation");

                                // add the Transaction to Transaction Table for Audit
                                if (AddTransaction(referenceId, accountNr, 0, account.Balance, amount, TransactionType.Deposit))
                                {
                                    if (TrySave())
                                    {
                                        return true;
                                    }
                                }
                                return false;
                            }
                            else
                            {
                                throw new ApplicationException("deposit an amount more than zero!!");
                            }
                        
                    case TransactionType.Withdrawal:

                        _logger.LogInformation("Entering the Withdrawal transaction process");
                    
                            if (account.Balance < amount)
                            {
                                throw new ApplicationException("insufficient funds");
                            }
                            else if (amount <= 0)
                            {
                                throw new ApplicationException("invalid withdrawal amount");
                            }
                            else if (account.Balance < 0)
                            {
                                throw new ArgumentOutOfRangeException("The initial balance must be greater or equals to 0");
                            }
                            else if (account.Balance - amount < 0)
                            {
                                throw new ApplicationException(string.Format("Withdrawing {0} would put your account in overdraft.", amount));
                            }

                            account.Balance -= amount;

                            _logger.LogInformation("Adding Withdrawal transaction");
                            // add the Transaction to Transaction Table for Audit
                            if (AddTransaction(referenceId, accountNr, 0, account.Balance, amount, TransactionType.Withdrawal))
                            {
                                if (TrySave())
                                {
                                    return true;
                                }
                            }
                            return false;
                    default:
                        break;
                }
            }
            else
            {
                throw new Exception("Transaction is a Duplicate");
            }
            if (!TrySave())
            {
                return false;
            }
            Helpers.TransactionType.Type = TransactionType.None;
            return true;
        }
        public async Task<bool> Transfer(Guid referenceId, long accountNrFrom, long AccountNrTo, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ApplicationException("transfer amount must be positive");
            }
            else if (amount == 0)
            {
                throw new ApplicationException("invalid transfer amount");
            }

            _logger.LogInformation("Entering the CheckDuplicateTransaction method");

            bool isDuplicate = CheckDuplicateTransaction(referenceId, amount);

            if (!isDuplicate)
            {
                _logger.LogInformation("Get accounts for both accounts to transfer");
                Account fromAccount = await GetAccount(accountNrFrom);
                Account toAccount = await GetAccount(AccountNrTo);

                if (fromAccount != null && toAccount != null)
                {
                    if (fromAccount.Balance < amount)
                    {
                        throw new ApplicationException("insufficient funds");
                    }
                    fromAccount.Balance -= amount;
                    toAccount.Balance += amount;
                }
                else
                {
                    throw new ApplicationException("One of the accounts does not exist");
                }

                _logger.LogInformation("Adding Transfer transaction");

                if (AddTransaction(referenceId, toAccount.AccountNr, 0, toAccount.Balance, amount,TransactionType.Deposit))
                    if (AddTransaction(referenceId, fromAccount.AccountNr, 0, fromAccount.Balance, amount, TransactionType.Withdrawal))
                    {
                        if (TrySave())
                        {
                            Helpers.TransactionType.Type = TransactionType.None;
                            return true;
                        }
                    }
                return false;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> CreateAccount(CreateAccountDto account)
        {
            if (account != null)
            {
                var accountCounter = GetAccountCount();
                accountCounter++;
                _context.Accounts.Add(new Account()
                {
                    Id = accountCounter,
                    referenceId = account.referenceId,
                    AccountNr = account.AccountNr,
                    Balance = account.Balance
                });
            }
            if (!TrySave())
            {
                return false;
            }
            return true;
        }
        public async Task<List<Transaction>> GetAccountTransactions(long accountNr)
        {
            var accountList = _context.Transactions.Where(x => x.AccountNr == accountNr).ToList();
            return accountList;
        }
        public bool TrySave()
        {
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool CheckDuplicateTransaction(Guid referenceId, decimal amount)
        {
            var transaction = _context.Transactions.Where(x => x.referenceId == referenceId && x.Amount == amount).FirstOrDefault();

            if (transaction != null)
            {
                return true;
            }
            return false;
        }

        public async Task<Account> GetAccount(long accountNr)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.AccountNr == accountNr);
            if (account != null)
                return account;
            else return null;
        }
     
        private int GetAccountCount()
        {
            int count = _context.Accounts.Count();
            return count;
        }
        private bool AddTransaction(Guid referenceId, long accountNr, long AccountNrTo, decimal balance, decimal amount, TransactionType transactionType)
        {
            try
            {
                var counter = GetTransactionCount();
                counter++;
                _context.Transactions.Add(new Transaction()
                {
                    UniqueId = counter,
                    referenceId = referenceId,
                    AccountNr = accountNr,
                    Balance = balance,
                    Amount = transactionType == TransactionType.Withdrawal ? -amount : amount
                });
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private int GetTransactionCount()
        {
            int count = _context.Transactions.Count();
            return count;
        }
        public async Task<decimal> GetAccountBalance(long accountNr)
        {
            var account = _context.Accounts.Where(x => x.AccountNr == accountNr).FirstOrDefault();
            return account.Balance;
        }
    }
}
