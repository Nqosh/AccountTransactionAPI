using AccountTransaction.Data;
using AccountTransaction.DTOs;
using AccountTransaction.Model;
using AccountTransaction.Services;
using AutoMapper;
using Customer_API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AccountTransaction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountTransactionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IAccountTransactionService _accountTransactionService;

        public AccountTransactionController(IMapper mapper, IAccountTransactionService accountTransactionService, ILogger logger)
        {
            _mapper = mapper;
            _logger = logger;
            _accountTransactionService = accountTransactionService;
        }

        /// <summary>
        /// post a deposit or withdrawal transaction.
        /// </summary>
        /// <returns></returns>
        // GET: api/<TransactionController>
        [HttpPost("depositwithdrawal")]
        public async Task<IActionResult> DepositWithDrawal(int userId, [FromQuery] DepositWithdrawalDto transaction)
        {

            if (Helpers.Claims.ClaimsArr == null)
                return Unauthorized();
            if (userId != int.Parse(Helpers.Claims.ClaimsArr[0].Value))
                return Unauthorized();

            if (transaction.TransactionType == TransactionType.Deposit)
            {
                Helpers.TransactionType.Type = TransactionType.Deposit;
            }
            else
            {
                Helpers.TransactionType.Type = TransactionType.Withdrawal;
            }

            _logger.LogInformation("Check if Account exists Method");
            var account = await _accountTransactionService.GetAccount(transaction.AccountNr);

            if (account != null)
            {
                _logger.LogInformation("Entering DepositWithDrawal Method");
                if (!await _accountTransactionService.DepositWithDrawal(transaction.referenceId, transaction.AccountNr, transaction.Amount, account))
                {
                    return BadRequest("Transaction could not be Implemented");
                }
                _logger.LogInformation("Exiting DepositWithDrawal Method");
            }
            else
            {
                return BadRequest("Account does not exist, Please create one");
            }
            return Ok("Transaction Successfull!!!");
        }

        /// <summary>
        /// Create an account.
        /// </summary>
        /// <returns></returns>
        // Post: api/<TransactionController>
        [HttpPost("createAccount")]
        public async Task<IActionResult> AddAccount(int userId, CreateAccountDto account)
        {

            if (Helpers.Claims.ClaimsArr == null)
                return Unauthorized();
            if (userId != int.Parse(Helpers.Claims.ClaimsArr[0].Value))
                return Unauthorized();

            var acc = await _accountTransactionService.GetAccount(account.AccountNr);

            if (acc == null)
            {
                if (!await _accountTransactionService.CreateAccount(account))
                {
                    return BadRequest("Account could not be created");
                }
                return Ok("Account Created Successfully");
            }
            else
            {
                return BadRequest("Account already Exists");
            } 
        }

        /// <summary>
        /// post a transfer transaction
        /// </summary>
        /// <returns></returns>
        // GET: api/<TransactionController>
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(int userId, [FromQuery] TransferTransactionDto transaction)
        {
            if (Helpers.Claims.ClaimsArr == null)
                return Unauthorized();
            if (userId != int.Parse(Helpers.Claims.ClaimsArr[0].Value))
                return Unauthorized();

            bool isTransfered = await _accountTransactionService.Transfer(transaction.referenceId, transaction.AccountNr, transaction.AccountNrTo, transaction.Amount);
            if (!isTransfered)
            {
                return BadRequest("Transfer failed");
            }
            return Ok("Transfer Successfull!!!"); ;
        }

        /// <summary>
        /// Get account transactions
        /// </summary>
        /// <returns></returns>
        // GET: api/<TransactionController>
        [HttpGet("transactions")]
        public async Task<IActionResult> GetAccountTransactions(int userId, long accountNr)
        {

            if (Helpers.Claims.ClaimsArr == null)
                return Unauthorized();
            if (userId != int.Parse(Helpers.Claims.ClaimsArr[0].Value))
                return Unauthorized();
            var account = await _accountTransactionService.GetAccount(accountNr);
            if (account != null)
            {
                var transactionList = await _accountTransactionService.GetAccountTransactions(accountNr);
                return Ok(transactionList);
            }
            else
            {
                return BadRequest("Account does not exit, Please create one");
            }
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        /// <returns></returns>
        // GET: api/<TransactionController>
        [HttpGet("accountbalance")]
        public async Task<IActionResult> GetAccountBalance(int userId, long accountNr)
        {

            if (Helpers.Claims.ClaimsArr == null)
                return Unauthorized();
            if (userId != int.Parse(Helpers.Claims.ClaimsArr[0].Value))
                return Unauthorized();
            var balance = await _accountTransactionService.GetAccountBalance(accountNr);
            if (balance != 0)
                return Ok(balance);
            else
                return BadRequest("Transaction had an issue try again later");
        }
    }
}
