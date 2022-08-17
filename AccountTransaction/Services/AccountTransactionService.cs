using AccountTransaction.Data;
using AccountTransaction.Model;
using Microsoft.Extensions.Logging;
using System;
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
        public Task DepositWithDrawal(Guid referenceId, long accountNr, decimal amount)
        {
            _logger.LogInformation("Checking duplicate transaction");

            bool isDuplicate = CheckDuplicateTransaction(referenceId, amount);

            if (!isDuplicate)
            {
                _logger.LogInformation("Getting account from database");

                Account account = GetAccount(accountNr);

                switch (Helpers.TransactionType.TransactionTypeEnum)
                {
                    case TransactionType.Deposit:

                        _logger.LogInformation("Entering the deposit transaction process");

                        if (account == null)
                        {
                            _logger.LogInformation("Checking account counter");

                            var accountCounter = GetAccountCount();
                            accountCounter++;
                            _context.Accounts.Add(new Account()
                            {
                                Id = accountCounter,
                                referenceId = referenceId,
                                AccountNr = accountNr,
                                Balance = amount
                            });

                            _logger.LogInformation("Adding transaction for a deposit transation");
                            // add the Transaction to Transaction Table for Audit
                            AddTransaction(referenceId, accountNr, 0, amount);
                        }
                        if (account != null)
                        {
                            if (amount > 0)
                            {
                                account.Balance += amount;

                                _logger.LogInformation("Adding transaction for an additional deposit transation");

                                // add the Transaction to Transaction Table for Audit
                                AddTransaction(referenceId, accountNr, 0, amount);
                            }
                            else
                            {
                                throw new ApplicationException("deposit an amount more than zero!!");
                            }
                        }
                        break;
                    case TransactionType.Withdrawal:

                        _logger.LogInformation("Entering the Withdrawal transaction process");

                        if (account != null)
                        {
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
                            AddTransaction(referenceId, accountNr, 0, amount);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                return Task.FromException(new Exception("Transaction is a Duplicate"));
            }
            if (!TrySave())
            {
                return Task.FromException(new Exception("Transaction Failed"));
            }
            Helpers.TransactionType.TransactionTypeEnum = TransactionType.None;
            return Task.CompletedTask;
        }
        public Task Transfer(Guid referenceId, long accountNrFrom, long AccountNrTo, decimal amount)
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
                Account fromAccount = GetAccount(accountNrFrom);
                Account toAccount = GetAccount(AccountNrTo);

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

                AddTransaction(referenceId, fromAccount.AccountNr, toAccount.AccountNr, amount);
            }
            else
            {
                return Task.FromException(new Exception("Transaction is a duplicate!!"));
            }
            if (!TrySave())
            {
                return Task.FromException(new Exception("Transaction Failed"));
            }
            AccountTransaction.Helpers.TransactionType.TransactionTypeEnum = TransactionType.None;
            return Task.CompletedTask;
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
        private Account GetAccount(long accountNr)
        {
            Account account = _context.Accounts.FirstOrDefault(x => x.AccountNr == accountNr);
            return account;
        }
        private int GetAccountCount()
        {
            int count = _context.Accounts.Count();
            return count;
        }
        private void AddTransaction(Guid referenceId, long accountNr, long AccountNrTo, decimal amount)
        {
            var counter = GetTransactionCount();
            counter++;
            _context.Transactions.Add(new Transaction()
            {
                UniqueId = counter,
                referenceId = referenceId,
                AccountNr = accountNr,
                Amount = amount
            });
        }
        private int GetTransactionCount()
        {
            int count = _context.Transactions.Count();
            return count;
        }
    }
}
