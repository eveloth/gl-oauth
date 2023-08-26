using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OauthShowcase.Domain;
using OauthShowcase.Mail;

namespace OauthShowcase.Data;

public class Seeder
{
    private string confirmRegistrationTemplateDefault;

    private readonly ApplicationContext _context;
    private readonly ILogger<Seeder> _logger;

    public Seeder(ApplicationContext context, ILogger<Seeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeDatabase()
    {
        var doTemplatesExist = await _context.EmailTemplates.AnyAsync();

        if (doTemplatesExist)
        {
            _logger.LogInformation("Templates exist, skipping seeding");
            return;
        }

        var templateHtml = await File.ReadAllTextAsync("confirm_registration_default.html");
        var template = new EmailTemplate(
            MailingDefaults.ConfirmRegistrationTemplateName,
            templateHtml
        );

        await _context.EmailTemplates.AddAsync(template);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeding completed");
    }
}