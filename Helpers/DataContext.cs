namespace webapiV2.Helpers;

using Microsoft.EntityFrameworkCore;
using webapiV2.Entities;

public class DataContext : DbContext {
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration) {
        Configuration = configuration;
    }

    // connect to  database
    protected override void OnConfiguring(DbContextOptionsBuilder options) 
        => options.UseNpgsql(Configuration.GetConnectionString("PostgreSql"));
    
    public DbSet<Account> Accounts { get; set; }
}