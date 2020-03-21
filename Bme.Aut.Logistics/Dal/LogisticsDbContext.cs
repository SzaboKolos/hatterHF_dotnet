using Bme.Aut.Logistics.Model;
using Microsoft.EntityFrameworkCore;

namespace Bme.Aut.Logistics.Dal
{
    public class LogisticsDbContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<TransportPlan> TransportPlans { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Action> Actions { get; set; }
        public DbSet<Compliance> Compliances { get; set; }

        public LogisticsDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                throw new System.Exception("DbContext not configured");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransportPlan>()
                .HasKey(tp => tp.Id);
            modelBuilder.Entity<TransportPlan>()
                .Property(tp => tp.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<TransportPlan>()
                .HasMany<Section>(tp => tp.Sections)
                .WithOne(s => s.TransportPlan)
                .HasForeignKey(s => s.TransportPlanId)
                .HasPrincipalKey(tp => tp.Id);

            modelBuilder.Entity<Section>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<Section>()
                .Property(s => s.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Section>()
                .HasOne<Milestone>(s => s.FromMilestone)
                .WithMany()
                .HasForeignKey(s => s.FromMilestoneId)
                .HasPrincipalKey(m => m.Id);
            modelBuilder.Entity<Section>()
                .HasOne<Milestone>(s => s.ToMilestone)
                .WithMany()
                .HasForeignKey(s => s.ToMilestoneId)
                .HasPrincipalKey(m => m.Id); ;

            modelBuilder.Entity<Milestone>()
                .HasKey(m => m.Id);
            modelBuilder.Entity<Milestone>()
                .Property(m => m.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Milestone>()
                .HasOne<Address>(m => m.Address)
                .WithMany()
                .HasForeignKey(m => m.AddressId)
                .HasPrincipalKey(a => a.Id);

            modelBuilder.Entity<Address>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<Address>()
                .Property(a => a.Id).ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }
    }
}
