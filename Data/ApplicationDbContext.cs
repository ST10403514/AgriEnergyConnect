using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnect.Models;

namespace AgriEnergyConnect.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<DiscussionPost> DiscussionPosts { get; set; }
        public DbSet<ProjectProposal> ProjectProposals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure precision for Price property
            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // Seed sample data
            builder.Entity<Farmer>().HasData(
                new Farmer { Id = 1, Name = "John Doe", Email = "john@example.com", Address = "123 Farm Rd" },
                new Farmer { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Address = "456 Agri St" }
            );

            builder.Entity<Product>().HasData(
                new Product { Id = 1, FarmerId = 1, Name = "Solar Irrigation System", Category = "Solar", Description = "Efficient solar-powered irrigation", Price = 1500.00m },
                new Product { Id = 2, FarmerId = 1, Name = "Farm Wind Turbine", Category = "Wind", Description = "Small-scale wind energy solution", Price = 5000.00m },
                new Product { Id = 3, FarmerId = 2, Name = "Biogas Generator", Category = "Biogas", Description = "Converts waste to energy", Price = 3000.00m }
            );

            builder.Entity<DiscussionPost>().HasData(
                new DiscussionPost { Id = 1, FarmerId = 1, Title = "Organic Farming Tips", Content = "Sharing my experience with compost", CreatedDate = new DateTime(2025, 5, 1) },
                new DiscussionPost { Id = 2, FarmerId = 2, Title = "Water Conservation", Content = "Drip irrigation success stories", CreatedDate = new DateTime(2025, 5, 2) }
            );

            builder.Entity<ProjectProposal>().HasData(
                new ProjectProposal { Id = 1, FarmerId = 1, Title = "Solar Farm Expansion", Description = "Seeking partners for solar panel installation", CreatedDate = new DateTime(2025, 5, 1) }
            );
        }
    }
}