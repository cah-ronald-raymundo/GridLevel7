using Microsoft.EntityFrameworkCore;

namespace NetAPIGrid.Models
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<VwUserTeam> VwUserTeams { get; set; }
        public virtual DbSet<TblProductiveList> TblProductiveLists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VwUserTeam>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("vw_UserTeam");

                entity.Property(e => e.Cluster)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Eid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EID");
                entity.Property(e => e.Segment)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.WorkType)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

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

    }
