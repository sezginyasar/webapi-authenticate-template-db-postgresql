namespace webapiV2.Helpers;

using Microsoft.EntityFrameworkCore;
using webapiV2.Entities;
using webapiV2.Entities.PageAuthorization;
using webapiV2.Entities.Logs;

public class DataContext : DbContext {
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration) {
        Configuration = configuration;
    }

    // connect to  database
    protected override void OnConfiguring(DbContextOptionsBuilder options) 
        => options.UseNpgsql(Configuration.GetConnectionString("PostgreSql"));
    
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Pages> Pages { get; set; }
    
    public DbSet<DbLogs> DbLogs { get; set; }
}