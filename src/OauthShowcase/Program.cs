using Chemodanchik.Mvc;
using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OauthShowcase;
using OauthShowcase.Data;
using OauthShowcase.Installers;
using OauthShowcase.Mapping;
using OauthShowcase.Options;
using OauthShowcase.Services;

var builder = WebApplication.CreateBuilder(args);

builder.InstallAuthentication();
builder.InstallSubdomainWildcardCorsPolicy();

builder.Services.AddDbContext<ApplicationContext>(
    optionsBuilder => optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);

builder.Services.Configure<AvatarValidationOptions>(
    builder.Configuration.GetSection(AvatarValidationOptions.AvatarValidation)
);

builder.Services.AddScoped<IUserManagement, UserManagement>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IMapper, Mapper>();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(IAssemblyMarker));

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