namespace webapiV2.Services.PagesAuthorizationSettings;

using webapiV2.Models.Pages;

public interface IPagesServices {
    //IEnumerable<UrunResponse> GetAll();
    IEnumerable<PagesResponse> GetAll();
    PagesResponse GetById(int id);
    PagesResponse Create(PageCreateRequest model);
    PagesResponse Update(int id, PageUpdateRequest model);
    void Delete(int id);
}