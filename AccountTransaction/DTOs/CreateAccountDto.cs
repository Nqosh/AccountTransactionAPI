using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer_API.DTOs
{
    public class CreateAccountDto
    {
        public Guid referenceId { get; set; }
        public long AccountNr { get; set; }
        public decimal Balance { get; set; }
    }
}
