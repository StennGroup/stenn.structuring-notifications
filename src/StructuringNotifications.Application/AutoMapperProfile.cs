using AutoMapper;
using OperationsNotifications.Contracts.Dto;
using MoneyDto = OperationsNotifications.Contracts.Dto.MoneyDto;
using OperationsLinkDto = Operations.Contracts.Dto.DocumentLinkDto;
using OperationsMoneyDto = Operations.Contracts.Dto.MoneyDto;

namespace StructuringNotifications.Application
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<OperationsLinkDto, LinkDto>();
            CreateMap<OperationsMoneyDto, MoneyDto>()
                .ForMember(x => x.CurrencyCode, x => x.MapFrom(y => y.Currency.Iso3LetterCode));
        }
    }
}