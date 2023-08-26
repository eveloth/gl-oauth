using Microsoft.EntityFrameworkCore;
using OauthShowcase.Data;
using OauthShowcase.Domain;
using OauthShowcase.Errors;

namespace OauthShowcase.Mail;

public class Mailing : IMailing
{
    private readonly ApplicationContext _context;

    public Mailing(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<EmailTemplate?> GetTemplate(int id, CancellationToken ct = default)
    {
        return await _context.EmailTemplates.FindAsync(new object?[] { id }, cancellationToken: ct);
    }

    public async Task<EmailTemplate?> GetTemplate(string name, CancellationToken ct = default)
    {
        return await _context.EmailTemplates.SingleOrDefaultAsync(x => x.Name == name, ct);
    }

    public async Task<List<EmailTemplate>> FindTemplate(
        string searchTerm,
        CancellationToken ct = default
    )
    {
        return await _context.EmailTemplates
            .Where(x => x.Name.Contains(searchTerm))
            .ToListAsync(ct);
    }

    public async Task CreateTemplate(EmailTemplate template, CancellationToken ct = default)
    {
        var existingTemplate = await GetTemplate(template.Name, ct);

        if (existingTemplate is not null)
        {
            throw new ApiException($"Template {template.Name} already exists");
        }

        await _context.EmailTemplates.AddAsync(template, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateTemplate(EmailTemplate template, CancellationToken ct = default)
    {
        var existingTemplate = await GetTemplate(template.Id, ct);

        if (existingTemplate is null)
        {
            throw new ApiException($"Template {template.Id} doesn't exist");
        }

        var templateWithTheSameName = await _context.EmailTemplates.FirstOrDefaultAsync(
            x => x.Name == template.Name && x.Id != template.Id,
            ct
        );

        if (templateWithTheSameName is not null)
        {
            throw new ApiException($"Template {template.Name} already exists");
        }

        existingTemplate.Name = template.Name;
        existingTemplate.Template = template.Template;
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteTemplate(int id, CancellationToken ct = default)
    {
        var existingTemplate = await GetTemplate(id, ct);

        if (existingTemplate is null)
        {
            throw new ApiException($"Template {id} doesn't exist");
        }

        _context.EmailTemplates.Remove(existingTemplate);
        await _context.SaveChangesAsync(ct);
    }
}