namespace webapiV2.Services.PagesAuthorizationSettings;

using System;
using System.Collections.Generic;
using AutoMapper;
using webapiV2.Helpers;
using webapiV2.Models.Pages;
using Dapper;
using Npgsql;
using webapiV2.Entities.PageAuthorization;

public class PagesServices : IPagesServices {
    private readonly DataContext _context;
    protected readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<PagesServices> _logger;

    public PagesServices(DataContext context, IConfiguration configuration, IMapper mapper, ILogger<PagesServices> logger) {
        _context = context;
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }

    public IEnumerable<PagesResponse> GetAll() {
        // string sql = @"SELECT * FROM pages;";

        // using var connect = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql"));
        // var result = connect.Query<PagesResponse>(sql);

        // return _mapper.Map<IEnumerable<PagesResponse>>(result);
        var pages = _context.Pages.Where(x=>x.Status==0);
        return _mapper.Map<IList<PagesResponse>>(pages);
    }

    public PagesResponse GetById(int id) {
        var page = getPages(id);
        return _mapper.Map<PagesResponse>(page);
    }

    public PagesResponse Create(PageCreateRequest model) {
        var page = _mapper.Map<Pages>(model);

        _context.Pages.Add(page);
        _context.SaveChanges();

        return _mapper.Map<PagesResponse>(page);
    }

    public PagesResponse Update(int id, PageUpdateRequest model) {
        var page = getPages(id);

        _mapper.Map(model, page);

        _context.Pages.Update(page);
        _context.SaveChanges();

        return _mapper.Map<PagesResponse>(page);
    }

    public void Delete(int id) {
        var page = getPages(id);

        page.Status = 0;

        _context.Update(page);
        _context.SaveChanges();
    }

    //? HELPER METHODS
    private Pages getPages(int id) {
        var page = _context.Pages.Find(id);
        if (page == null) throw new KeyNotFoundException("Sayfa bulunamadı");
        return page;    }
}