using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NetAPIGrid.Models;

public partial class RpaRenewalsContext : DbContext
{
    public RpaRenewalsContext()
    {
    }

    public RpaRenewalsContext(DbContextOptions<RpaRenewalsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<VwUser> VwUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=WPEC5009aaasq01;Database=RPA_RENEWALS;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VwUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_Users");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDt).HasColumnType("datetime");
            entity.Property(e => e.Eid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDt).HasColumnType("datetime");
            entity.Property(e => e.NameOfUser)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.O365Uid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("O365_UID");
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("User_ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
