namespace webapiV2.Services.Menus;

using webapiV2.Models.Menus;

public interface IMenuServices {
    IEnumerable<MenuResponse> GetAll();
    MenuResponse GetById(int id);
    MenuResponse Create(MenuCreateRequest model);
    MenuResponse Update(int id, MenuUpdateRequest model);
    void Delete(int id);
    IEnumerable<UstMenuResponse> UstMenuGetAll();
}