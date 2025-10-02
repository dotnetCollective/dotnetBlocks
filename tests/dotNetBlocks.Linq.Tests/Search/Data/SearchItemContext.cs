using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;


public class SearchItemContext : DbContext
{
    public SearchItemContext(DbContextOptions options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<SearchItem> SearchItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var builder = modelBuilder.Entity<SearchItem>();
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Field1);
        builder.Property(c => c.Field2);

        var builder2 = modelBuilder.Entity<SearchChildItem>();
        builder2.HasKey(c => c.Id);
        builder2.Property(c => c.ChildField1);
        builder2.Property(c => c.ChildField2);

        builder
            .HasOne(p => p.Child)
            .WithOne(p => p.Parent)
            .HasForeignKey<SearchChildItem>(p => p.ParentId);

    }
}
