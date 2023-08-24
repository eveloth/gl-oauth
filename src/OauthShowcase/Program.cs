using Chemodanchik.Mvc;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using OauthShowcase.Data;
using OauthShowcase.Installers;
using OauthShowcase.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.InstallAuthentication();
builder.InstallSubdomainWildcardCorsPolicy();

builder.Services.AddDbContext<ApplicationContext>(
    optionsBuilder => optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);

builder.ConfigureOptions();
builder.InstallServices();

builder.Services.AddControllers(options => options.UseSlugCaseRoutes());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.ConfigureDomainToResponseMapping();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    await context.Database.MigrateAsync();
}

app.UseForwardedHeaders(
    new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    }
);

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();