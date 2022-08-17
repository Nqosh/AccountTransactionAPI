using AutoMapper;

namespace AccountTransaction.Core
{
    public class AccountTransactionMapping : Profile
    {
        public AccountTransactionMapping()
        {
            CreateMap<Model.Transaction, DTOs.DepositWithdrawalDto>().ReverseMap();
            CreateMap<Model.Transaction, DTOs.DepositWithdrawalDto>().ReverseMap();
        }
    }
}
