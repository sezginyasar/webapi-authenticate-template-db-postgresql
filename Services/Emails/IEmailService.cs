namespace webapiV2.Services.Emails;
public interface IEmailService {
    void Send(string to, string subject, string html);
    void Send(string to, string subject, string html, string from);
}