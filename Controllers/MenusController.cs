namespace webapiV2.Controllers;

using Microsoft.AspNetCore.Mvc;
using webapiV2.Authorization;
using webapiV2.Entities.Menus;
using webapiV2.Models.Menus;
using webapiV2.Services.Menus;

//! Test işlemi için Authorize kapatıldı
//[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class MenusController : BaseController {
    private readonly IMenuServices _menuService;

    public MenusController(IMenuServices menuService) {
        _menuService = menuService;
    }

    // menu listesi hepsi
    [HttpGet("menus")]
    public ActionResult<IEnumerable<MenuResponse>> GetAll() {
        var menus = _menuService.GetAll();
        return Ok(menus);
    }

    // menu id
    [HttpGet("menu")]
    public ActionResult<MenuResponse> GetById(int id) {
        var menu = _menuService.GetById(id);
        return Ok(menu);
    }

    // menu ekleme
    [HttpPost("menu_create")]
    public ActionResult<MenuResponse> Create(MenuCreateRequest model) {
        var menu = _menuService.Create(model);
        return Ok(menu);
    }

    // menu güncelleme
    [HttpPut("menu_update")]
    public ActionResult<MenuResponse> Update(int id, MenuUpdateRequest model) {
        var menu = _menuService.Update(id, model);
        return Ok(menu);
    }

    // menu silme
    [HttpDelete("menu{id:int}")]
    public IActionResult Delete(int id) {
        _menuService.Delete(id);
        return Ok(new { message = "Menü satırı silindi." });
    }

    // üst menu listesi hepsi
    [HttpGet("ust_menu")]
    public ActionResult<IEnumerable<UstMenuResponse>> UstMenuGetAll() {
        var ustMenus = _menuService.UstMenuGetAll();
        return Ok(ustMenus);
    }
}