namespace webapiV2.Helpers;

using AutoMapper;
using webapiV2.Entities;
using webapiV2.Models.Accounts;

public class AutoMapperProfile : Profile
{
    // model ve varlık nesneleri arasındaki eşlemeler
    public AutoMapperProfile()
    {
        CreateMap<Account, AccountResponse>();

        CreateMap<Account, AuthenticateResponse>();

        CreateMap<RegisterRequest, Account>();

        CreateMap<CreateRequest, Account>();

        CreateMap<UpdateRequest, Account>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    // boş ve boş dize özelliklerini yoksay
                    if (prop == null) return false;
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                    // boş rolü yoksay
                    if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                    return true;
                }
            ));
    }
}