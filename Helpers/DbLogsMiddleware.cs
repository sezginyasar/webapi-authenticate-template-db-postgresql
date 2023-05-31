namespace webapiV2.Helpers;

using webapiV2.Authorization;
using webapiV2.Entities.Logs;

public class DbLogsMiddleware {
    private readonly DataContext _context;
    private IJwtUtils _jwtUtils;
    public DbLogsMiddleware(DataContext context, IJwtUtils jwtUtils) {
        _context = context;
        _jwtUtils = jwtUtils;
    }
    public Task Log(DbLogs entry, bool persist = true) {

        //Budama işlemleri
        if (!string.IsNullOrEmpty(entry.Message)) {
            if (entry.Message.Length > 990) entry.Message = entry.Message.Substring(0, 990) + " [...]";
        }

        if (!string.IsNullOrEmpty(entry.Browser)) {
            if (entry.Browser.Length > 990) entry.Browser = entry.Browser.Substring(0, 990) + " [...]";
        }

        entry.Zaman = DateTime.Now;

        _context.DbLogs.Add(entry);

        if (persist) {
            return _context.SaveChangesAsync(); //Hata olması durumunda?
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Veritabanına işlem logunu kaydeder.
    /// </summary>
    /// <param name="username">İşlemi yapan kullanıcı adı</param>
    /// <param name="httpContext">Komutun çalıştığın HttpContext</param>
    /// <param name="islem">Yapılan işlem, '''Sabitler sınıfında bulunur'''.</param>
    /// <param name="id">İşlem yapılan nesne id. Null olabilir.</param>
    /// <param name="msg">(Opsiyonel) Ek bir mesaj vermek için kullanılabilir.</param>
    /// <param name="nesne">Üzerinde işlem yapılan nesnenin en son hali</param>
    /// <param name="persist">Veri tabanına yazılma durumu. True ise hemen yazılır, fakat önceki değişiklikler de yazılır. false ise dbcontext'e eklenir
    /// fakat ayrıca SaveChanges çağrımı gereklidir. Fakat bu durum kayıt işleminde transaction kullanımında faydalı olur.
    /// </param>
    /// <returns>Task</returns>
    public async Task Log(string username, HttpContext httpContext, string islem, Guid? id, string msg = null, object nesne = null, bool persist = true) {
        var entry = new DbLogs() {
            Islem = islem,
            Browser = httpContext.Items["browser"].ToString(),
            IpAdres = httpContext.Items["clientIp"].ToString(),
            Username = username,
            Nesne = nesne,
            NesneId = id,
            Message = msg
        };

        await Log(entry, persist);
    }
}