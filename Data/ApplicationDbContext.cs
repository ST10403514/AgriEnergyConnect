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
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectCollaborator> ProjectCollaborators { get; set; }
        public DbSet<FundingOpportunity> FundingOpportunities { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure precision for Price property
            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // Ensure Role is required
            builder.Entity<ApplicationUser>()
                .Property(u => u.Role)
                .IsRequired();

            builder.Entity<Product>()
              .HasOne(p => p.Farmer)
              .WithMany()
              .HasForeignKey(p => p.FarmerId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DiscussionPost>()
                .HasOne(p => p.Farmer)
                .WithMany()
                .HasForeignKey(p => p.FarmerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.Farmer)
                .WithMany()
                .HasForeignKey(c => c.FarmerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.DiscussionPost)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.DiscussionPostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Project>()
                .HasOne(p => p.Farmer)
                .WithMany()
                .HasForeignKey(p => p.FarmerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProjectCollaborator>()
                .HasKey(pc => new { pc.ProjectId, pc.FarmerId });

            builder.Entity<ProjectCollaborator>()
                .HasOne(pc => pc.Project)
                .WithMany(p => p.Collaborators)
                .HasForeignKey(pc => pc.ProjectId);

            builder.Entity<ProjectCollaborator>()
                .HasOne(pc => pc.Farmer)
                .WithMany()
                .HasForeignKey(pc => pc.FarmerId);

            builder.Entity<ProductReview>()
                .HasOne(pr => pr.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(pr => pr.ProductId);

            builder.Entity<ProductReview>()
                .HasOne(pr => pr.Farmer)
                .WithMany()
                .HasForeignKey(pr => pr.FarmerId);
        }
    }
}
