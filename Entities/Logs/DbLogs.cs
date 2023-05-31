using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapiV2.Entities.Logs;

[Table("db_log")]
public class DbLogs {
    [Column("id")] 
    public int Id { get; set; }
    
    [Column("username"), StringLength(100)]
    public string? Username { get; set; }

    [Column("zaman")]
    public DateTime Zaman { get; set; }

    [Column("islem"), StringLength(30)]
    public string? Islem { get; set; }

    [Column("tablo"), StringLength(30)]
    public string? Tablo { get; set; }

    [Column("nesne_id")]
    public Guid? NesneId { get; set; }

    [Column("nesne", TypeName = "jsonb")]
    public object? Nesne { get; set; }

    [Column("ip_adres"), StringLength(30)]
    public string? IpAdres { get; set; }

    [Column("browser"), StringLength(1000)]
    public string? Browser { get; set; }

    [Column("message"), StringLength(1000)]
    public string? Message { get; set; }

    [NotMapped]
    public string? AdiSoyadi { get; set; }
}