using AccountTransaction.Model;
using Microsoft.EntityFrameworkCore;

namespace AccountTransaction.Data
{
    public class AccountTransactionDbContext : DbContext
    {
        public AccountTransactionDbContext(DbContextOptions<AccountTransactionDbContext> options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
