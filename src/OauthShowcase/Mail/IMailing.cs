using OauthShowcase.Domain;

namespace OauthShowcase.Mail;

public interface IMailing
{
    Task<EmailTemplate?> GetTemplate(int id, CancellationToken ct = default!);
    Task<EmailTemplate?> GetTemplate(string name, CancellationToken ct = default!);
    Task<List<EmailTemplate>> FindTemplate(
        string searchTerm,
        CancellationToken ct = default!
    );
    Task CreateTemplate(EmailTemplate template, CancellationToken ct = default!);
    Task UpdateTemplate(EmailTemplate template, CancellationToken ct = default!);
    Task DeleteTemplate(int id, CancellationToken ct = default!);
}