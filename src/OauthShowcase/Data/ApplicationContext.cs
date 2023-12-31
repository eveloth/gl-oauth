﻿using Microsoft.EntityFrameworkCore;

namespace OauthShowcase.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IAssemblyMarker).Assembly);
    }

    public required DbSet<User> Users { get; set; }
    public required DbSet<ExternalData> ExternalData { get; set; }
}