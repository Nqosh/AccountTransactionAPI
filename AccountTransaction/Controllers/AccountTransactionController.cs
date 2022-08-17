using AccountTransaction.Data;
using AccountTransaction.DTOs;
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
        public async Task<IActionResult> DepositWithDrawal([FromQuery] DepositWithdrawalDto transaction)
        {
            if (transaction.TransactionType == TransactionType.Deposit)
            {
                Helpers.TransactionType.TransactionTypeEnum = TransactionType.Deposit;
            }
            else
            {
                Helpers.TransactionType.TransactionTypeEnum = TransactionType.Withdrawal;
            }
            _logger.LogInformation("Entering DepositWithDrawal Method");
            await _accountTransactionService.DepositWithDrawal(transaction.referenceId, transaction.AccountNr, transaction.Amount);
            _logger.LogInformation("Exiting DepositWithDrawal Method");
            return Ok();
        }

        /// <summary>
        /// post a transfer transaction
        /// </summary>
        /// <returns></returns>
        // GET: api/<TransactionController>
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromQuery] TransferTransactionDto transaction)
        {
            await _accountTransactionService.Transfer(transaction.referenceId, transaction.AccountNr, transaction.AccountNrTo, transaction.Amount);
            return Ok();
        }
    }
}
