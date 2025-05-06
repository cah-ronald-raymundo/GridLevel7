using Microsoft.EntityFrameworkCore;
namespace NetAPIGrid.Models
{
    public partial class RenewalsDBContext : DbContext
    {
        public RenewalsDBContext()
        {
        }

        public RenewalsDBContext(DbContextOptions<RenewalsDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<VwUser> VwUsers { get; set; }

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
}
