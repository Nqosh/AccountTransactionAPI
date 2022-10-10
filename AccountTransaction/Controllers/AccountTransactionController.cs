using AccountTransaction.Data;
using AccountTransaction.DTOs;
using AccountTransaction.Model;
using AccountTransaction.Services;
using AutoMapper;
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
            _logger.LogInformation("Entering DepositWithDrawal Method");

            if (!await _accountTransactionService.DepositWithDrawal(transaction.referenceId, transaction.AccountNr, transaction.Amount))
            {
                return BadRequest("Transaction could not be Implemented");
            }
            _logger.LogInformation("Exiting DepositWithDrawal Method");
            return Ok("Transaction Succesful!!!");
        }

        /// <summary>
        /// Create an account.
        /// </summary>
        /// <returns></returns>
        // Post: api/<TransactionController>
        [HttpPost("createAccount")]
        public async Task<IActionResult> AddAccount(int userId, Account account)
        {

            if (Helpers.Claims.ClaimsArr == null)
                return Unauthorized();
            if (userId != int.Parse(Helpers.Claims.ClaimsArr[0].Value))
                return Unauthorized();

            if (!await _accountTransactionService.CreateAccount(account))
            {
                return BadRequest("Account could not be created");
            }
            return Ok("Account Created Successfully");
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
            return Ok("Transfer Succesfull!!!"); ;
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

            var transactionList = await _accountTransactionService.GetAccountTransactions(accountNr);
            return Ok(transactionList);

        }
    }
}
