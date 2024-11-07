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

    public virtual DbSet<IscStatusbarcolor> IscStatusbarcolors { get; set; }

    public virtual DbSet<IscStatusbaricon> IscStatusbaricons { get; set; }

    public virtual DbSet<IscStatusbarmessage> IscStatusbarmessages { get; set; }

    public virtual DbSet<IscStatusbarproperty> IscStatusbarproperties { get; set; }
    
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
            entity.Property(e => e.FrontBannerFile).HasMaxLength(255);
            entity.Property(e => e.PublishedAt).HasColumnType("datetime");
            entity.Property(e => e.PublishedBy).HasColumnType("int(11)");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.PublishedByNavigation).WithMany(p => p.IscPosts)
                .HasForeignKey(d => d.PublishedBy)
                .HasConstraintName("isc-posts_ibfk_1");
        });

        modelBuilder.Entity<IscStatusbarcolor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("isc-statusbarcolor");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.ColorName).HasMaxLength(255);
        });

        modelBuilder.Entity<IscStatusbaricon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("isc-statusbaricons");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.IconName).HasMaxLength(255);
        });

        modelBuilder.Entity<IscStatusbarmessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("isc-statusbarmessages");

            entity.HasIndex(e => e.CreatedBy, "CreatedBy");

            entity.HasIndex(e => e.Icon, "Icon");

            entity.HasIndex(e => e.ModifiedBy, "ModifiedBy");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasColumnType("int(11)");
            entity.Property(e => e.ExpiresIn).HasColumnType("datetime");
            entity.Property(e => e.Icon).HasColumnType("int(255)");
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasColumnType("int(11)");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.IscStatusbarmessageCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("isc-statusbarmessages_ibfk_2");

            entity.HasOne(d => d.IconNavigation).WithMany(p => p.IscStatusbarmessages)
                .HasForeignKey(d => d.Icon)
                .HasConstraintName("isc-statusbarmessages_ibfk_1");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.IscStatusbarmessageModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("isc-statusbarmessages_ibfk_3");
        });

        modelBuilder.Entity<IscStatusbarproperty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("isc-statusbarproperties");

            entity.HasIndex(e => e.StatusBarColor, "StatusBarColor");

            entity.HasIndex(e => e.StatusBarIcon, "StatusBarIcon");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.StatusBarColor).HasColumnType("int(255)");
            entity.Property(e => e.StatusBarIcon).HasColumnType("int(11)");

            entity.HasOne(d => d.StatusBarColorNavigation).WithMany(p => p.IscStatusbarproperties)
                .HasForeignKey(d => d.StatusBarColor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("isc-statusbarproperties_ibfk_2");

            entity.HasOne(d => d.StatusBarIconNavigation).WithMany(p => p.IscStatusbarproperties)
                .HasForeignKey(d => d.StatusBarIcon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("isc-statusbarproperties_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
