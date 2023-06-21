namespace webapiV2.Helpers;

using AutoMapper;
using webapiV2.Entities;
using webapiV2.Entities.PageAuthorization;
using webapiV2.Entities.Menus;
using webapiV2.Models.Accounts;
using webapiV2.Models.Pages;
using webapiV2.Models.Menus;

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

        CreateMap<Pages, PagesResponse>();

        CreateMap<PageCreateRequest, Pages>();

        CreateMap<PageUpdateRequest, Pages>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) => {
                    // boş ve boş dize özelliklerini yoksay
                    if (prop == null) return false;
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                    return true;
                }
            ));

        CreateMap<Menu, MenuResponse>();

        CreateMap<MenuCreateRequest, Menu>();

        CreateMap<MenuUpdateRequest, Menu>()
            // .ForAllMembers(x => x.Condition(
            //     (src, dest, prop) => {
            //         // boş ve boş dize özelliklerini yoksay
            //         if (prop == null) return false;
            //         if (prop.GetType() == typeof(string) && string.IsNullOrWhiteSpace((string)prop)) return false;
            //         return true;
            //     }
            // ))
            ;

        CreateMap<Menu, UstMenuResponse>();
    }
}