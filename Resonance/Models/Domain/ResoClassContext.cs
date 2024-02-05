﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ResoClassAPI.Models.Domain;

public partial class ResoClassContext : DbContext
{
    public ResoClassContext()
    {
    }

    public ResoClassContext(DbContextOptions<ResoClassContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ResoUser> ResoUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=SqlConnectionString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResoUser>(entity =>
        {
            entity.ToTable("ResoUser");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeviceId).HasMaxLength(128);
            entity.Property(e => e.Email).HasMaxLength(128);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Latitude)
                .HasMaxLength(50)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasMaxLength(50)
                .HasColumnName("longitude");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(128);
            entity.Property(e => e.PhoneNumber).HasMaxLength(10);
            entity.Property(e => e.RegistrationId).HasMaxLength(256);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
