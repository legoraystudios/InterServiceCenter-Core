using System;
using System.Collections.Generic;
using InterServiceCenter_Core.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace InterServiceCenter_Core.Contexts;

public partial class InterServiceCenterContext : DbContext
{
    public InterServiceCenterContext()
    {
    }

    public InterServiceCenterContext(DbContextOptions<InterServiceCenterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<IscAccount> IscAccounts { get; set; }

    public virtual DbSet<IscDevicesession> IscDevicesessions { get; set; }

    public virtual DbSet<IscPost> IscPosts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<IscAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("isc-accounts");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.HashedPassword).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.ProfilePhotoFile).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(255);
        });

        modelBuilder.Entity<IscDevicesession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("isc-devicesessions");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.DeviceId).HasMaxLength(255);
            entity.Property(e => e.DeviceName).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.ExpireIn).HasColumnType("datetime");
            entity.Property(e => e.IpAddress).HasMaxLength(255);
        });

        modelBuilder.Entity<IscPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("isc-posts");

            entity.HasIndex(e => e.PublishedBy, "PublishedBy");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.FrontImage).HasColumnType("blob");
            entity.Property(e => e.PublishedAt).HasColumnType("datetime");
            entity.Property(e => e.PublishedBy).HasColumnType("int(11)");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.PublishedByNavigation).WithMany(p => p.IscPosts)
                .HasForeignKey(d => d.PublishedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("isc-posts_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
