using Microsoft.EntityFrameworkCore;
using NetAPIGrid.Models;

namespace NetAPIGrid.Context
{
    public partial class GRID_LVL_SEVEN_DBContext:DbContext
    {
        public GRID_LVL_SEVEN_DBContext()
        {
        }

        public GRID_LVL_SEVEN_DBContext(DbContextOptions<GRID_LVL_SEVEN_DBContext> options) : base(options)
        {
        }

        public DbSet<vw_ActiveWindow_Idle_With_Duration> vw_ActiveWindow_Idle_With_Durations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<vw_ActiveWindow_Idle_With_Duration>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_ActiveWindow_Idle_With_Duration");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
