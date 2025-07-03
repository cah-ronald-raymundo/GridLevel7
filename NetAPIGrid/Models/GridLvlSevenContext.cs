using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NetAPIGrid.Models;

public partial class GridLvlSevenContext : DbContext
{
    public GridLvlSevenContext()
    {
    }

    public GridLvlSevenContext(DbContextOptions<GridLvlSevenContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblProductiveList> TblProductiveLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=GRID_LVL_SEVEN_PROD");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblProductiveList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblProductive");

            entity.ToTable("tblProductiveList");

            entity.Property(e => e.Details).IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
