using AutoMapper;
using BankSystem_MVC_.Dto;
using BankSystem_MVC_.Models;

namespace BankSystem_MVC_.Mapper
{
    public class MapperConfiguration:Profile
    {
        public MapperConfiguration()
        {
            CreateMap<Deposite, DepositeDto>().ReverseMap();
            CreateMap<Withdraw, WithdrawDto>().ReverseMap();
            CreateMap<Transfer, TransferDto>().ReverseMap();

            CreateMap<Account, LogginDto>().ReverseMap();
        }
    }
}
