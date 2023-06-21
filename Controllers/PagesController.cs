namespace webapiV2.Controllers;

using Microsoft.AspNetCore.Mvc;
using webapiV2.Authorization;
using webapiV2.Entities.PageAuthorization;
using webapiV2.Models.Pages;
using webapiV2.Services.PagesAuthorizationSettings;

//! Test işlemi için Authorize kapatıldı
//[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class PagesController : BaseController {
    private readonly IPagesServices _pagesService;

    public PagesController(IPagesServices pagesService) {
        _pagesService = pagesService;
    }

    // sayfa listesi hepsi
    [HttpGet("pages")]
    public ActionResult<IEnumerable<PagesResponse>> GetAll() {
        var pages = _pagesService.GetAll();
        return Ok(pages);
    }

    // Sayfa id
    [HttpGet("page")]
    public ActionResult<PagesResponse> GetById(int id) {
        var page = _pagesService.GetById(id);
        return Ok(page);
    }

    // Sayfa ekleme
    [HttpPost("page_create")]
    public ActionResult<PagesResponse> Create(PageCreateRequest model) {
        var page = _pagesService.Create(model);
        return Ok(page);
    }

    // Sayfa güncelleme
    [HttpPut("page_update")]
    public ActionResult<PagesResponse> Update(int id, PageUpdateRequest model) {
        var page = _pagesService.Update(id, model);
        return Ok(page);
    }

    // sayfa silme
    [HttpDelete("page{id:int}")]
    public IActionResult Delete(int id) {
        _pagesService.Delete(id);
        return Ok(new { message = "Sayfa silindi." });
    }
}