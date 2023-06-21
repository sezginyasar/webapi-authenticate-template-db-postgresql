namespace webapiV2.Services.Menus;

using System;
using System.Collections.Generic;
using AutoMapper;
using webapiV2.Helpers;
using webapiV2.Models.Menus;
using Dapper;
using Npgsql;
using webapiV2.Entities.Menus;

public class MenuServices : IMenuServices {
    private readonly DataContext _context;
    protected readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<MenuServices> _logger;

    public MenuServices(DataContext context, IConfiguration configuration, IMapper mapper, ILogger<MenuServices> logger) {
        _context = context;
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }

    public IEnumerable<MenuResponse> GetAll() {
        var result = _context.Menu.Where(x => x.Status != 0);
        return _mapper.Map<IList<MenuResponse>>(result);
    }

    public MenuResponse GetById(int id) {
        var result = getMenu(id);
        return _mapper.Map<MenuResponse>(result);
    }

    public MenuResponse Create(MenuCreateRequest model) {
        var result = _mapper.Map<Menu>(model);

        _context.Menu.Add(result);
        _context.SaveChanges();

        return _mapper.Map<MenuResponse>(result);
    }

    public MenuResponse Update(int id, MenuUpdateRequest model) {
        var result = getMenu(id);

        _mapper.Map(model, result);

        _context.Menu.Update(result);
        _context.SaveChanges();

        return _mapper.Map<MenuResponse>(result);
    }

    public void Delete(int id) {
        var result = getMenu(id);

        result.Status = 0;

        _context.Update(result);
        _context.SaveChanges();
    }

    //? HELPER METHODS
    private Menu getMenu(int id) {
        var result = _context.Menu.Find(id);
        if (result == null) throw new KeyNotFoundException("Menü bulunamadı");
        return result;
    }

    public IEnumerable<UstMenuResponse> UstMenuGetAll() {
        var result = _context.Menu.Where(x => x.Status != 0 && x.ParentId == 0);
        return _mapper.Map<IList<UstMenuResponse>>(result);
    }
}