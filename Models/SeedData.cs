using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnect.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AgriEnergyConnect.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                Console.WriteLine("Starting seeding...");

                // Seed roles
                Console.WriteLine("Seeding roles...");
                string[] roles = new[] { "Farmer", "Employee", "GreenEnergyExpert" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        var result = await roleManager.CreateAsync(new IdentityRole(role));
                        if (!result.Succeeded)
                        {
                            throw new Exception($"Failed to create role {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                // Seed test Farmer user
                Console.WriteLine("Seeding farmer user...");
                var farmerEmail = "farmer@example.com";
                var farmerUser = await userManager.FindByEmailAsync(farmerEmail);
                if (farmerUser == null)
                {
                    farmerUser = new ApplicationUser
                    {
                        UserName = farmerEmail,
                        Email = farmerEmail,
                        Role = "Farmer",
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(farmerUser, "Farmer@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(farmerUser, "Farmer");
                    }
                    else
                    {
                        throw new Exception($"Failed to create farmer user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }

                // Seed farmer profile
                if (!await dbContext.Farmers.AnyAsync(f => f.Email == farmerEmail))
                {
                    dbContext.Farmers.Add(new Farmer
                    {
                        Name = "Test Farmer",
                        Email = farmerEmail,
                        Address = "789 Farm Lane"
                    });
                    await dbContext.SaveChangesAsync();
                }

                // Seed test Employee user
                Console.WriteLine("Seeding employee user...");
                var employeeEmail = "employee@example.com";
                var employeeUser = await userManager.FindByEmailAsync(employeeEmail);
                if (employeeUser == null)
                {
                    employeeUser = new ApplicationUser
                    {
                        UserName = employeeEmail,
                        Email = employeeEmail,
                        Role = "Employee",
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(employeeUser, "Employee@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(employeeUser, "Employee");
                    }
                    else
                    {
                        throw new Exception($"Failed to create employee user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }

                // Seed test Expert user
                Console.WriteLine("Seeding expert user...");
                var expertEmail = "expert@example.com";
                var expertUser = await userManager.FindByEmailAsync(expertEmail);
                if (expertUser == null)
                {
                    expertUser = new ApplicationUser
                    {
                        UserName = expertEmail,
                        Email = expertEmail,
                        Role = "GreenEnergyExpert",
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(expertUser, "Expert@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(expertUser, "GreenEnergyExpert");
                    }
                    else
                    {
                        throw new Exception($"Failed to create expert user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }

                // Seed expert profile
                if (!await dbContext.Farmers.AnyAsync(f => f.Email == expertEmail))
                {
                    dbContext.Farmers.Add(new Farmer
                    {
                        Name = "Jane Expert",
                        Email = expertEmail,
                        Address = "123 Green St, Stellenbosch"
                    });
                    await dbContext.SaveChangesAsync();
                }

                // Seed sample farmers
                Console.WriteLine("Seeding farmers...");
                var farmers = new[]
                {
                    new Farmer { Name = "John Doe", Email = "john@example.com", Address = "123 Farm Rd" },
                    new Farmer { Name = "Jane Smith", Email = "jane@example.com", Address = "456 Agri St" },
                    new Farmer { Name = "Sam Wilson", Email = "sam@example.com", Address = "789 Green Way" },
                    new Farmer { Name = "Mary Johnson", Email = "mary@example.com", Address = "321 Eco Lane" },
                    new Farmer { Name = "Tom Brown", Email = "tom@example.com", Address = "654 Solar Rd" },
                    new Farmer { Name = "Emma Davis", Email = "emma@example.com", Address = "987 Harvest Rd" },
                    new Farmer { Name = "Liam Garcia", Email = "liam@example.com", Address = "147 Rural Ave" },
                    new Farmer { Name = "Olivia Martinez", Email = "olivia@example.com", Address = "258 Green Fields" },
                    new Farmer { Name = "Noah Lee", Email = "noah@example.com", Address = "369 Orchard Way" },
                    new Farmer { Name = "Ava White", Email = "ava@example.com", Address = "741 Farmstead Ln" }
                };
                foreach (var farmer in farmers)
                {
                    if (!await dbContext.Farmers.AnyAsync(f => f.Email == farmer.Email))
                    {
                        dbContext.Farmers.Add(farmer);
                    }
                }
                await dbContext.SaveChangesAsync();

                // Assign FarmerIds for products and reviews
                var farmerIds = await dbContext.Farmers
                    .Where(f => farmers.Select(fm => fm.Email).Contains(f.Email))
                    .Select(f => new { f.Email, f.Id })
                    .ToDictionaryAsync(f => f.Email, f => f.Id);

                // Seed products
                Console.WriteLine("Seeding products...");
                var products = new[]
                {
                    new Product { Id = 1, FarmerId = farmerIds["john@example.com"], Name = "Solar Irrigation System", Category = "Solar", Description = "Efficient solar-powered irrigation", Price = 1500.00m, ProductionDate = new DateTime(2025, 4, 10) },
                    new Product { Id = 2, FarmerId = farmerIds["john@example.com"], Name = "Farm Wind Turbine", Category = "Wind", Description = "Small-scale wind energy solution", Price = 5000.00m, ProductionDate = new DateTime(2025, 4, 15) },
                    new Product { Id = 3, FarmerId = farmerIds["jane@example.com"], Name = "Biogas Generator", Category = "Biogas", Description = "Converts waste to energy", Price = 3000.00m, ProductionDate = new DateTime(2025, 4, 20) },
                    new Product { Id = 4, FarmerId = farmerIds["sam@example.com"], Name = "Organic Fertilizer", Category = "Organic", Description = "Natural compost for crops", Price = 200.00m, ProductionDate = new DateTime(2025, 5, 1) },
                    new Product { Id = 5, FarmerId = farmerIds["mary@example.com"], Name = "Hydroponic System", Category = "Hydroponics", Description = "Soil-less farming kit", Price = 1200.00m, ProductionDate = new DateTime(2025, 5, 5) },
                    new Product { Id = 6, FarmerId = farmerIds["tom@example.com"], Name = "Solar Dryer", Category = "Solar", Description = "Dries crops using solar energy", Price = 800.00m, ProductionDate = new DateTime(2025, 5, 10) },
                    new Product { Id = 7, FarmerId = farmerIds["emma@example.com"], Name = "Wind-Powered Pump", Category = "Wind", Description = "Water pumping solution", Price = 2500.00m, ProductionDate = new DateTime(2025, 5, 15) },
                    new Product { Id = 8, FarmerId = farmerIds["liam@example.com"], Name = "Biomass Heater", Category = "Biomass", Description = "Heats greenhouses with biomass", Price = 1800.00m, ProductionDate = new DateTime(2025, 5, 20) },
                    new Product { Id = 9, FarmerId = farmerIds["olivia@example.com"], Name = "Drip Irrigation Kit", Category = "Irrigation", Description = "Water-saving irrigation system", Price = 600.00m, ProductionDate = new DateTime(2025, 5, 25) },
                    new Product { Id = 10, FarmerId = farmerIds["noah@example.com"], Name = "Compost Turner", Category = "Organic", Description = "Automates compost mixing", Price = 1000.00m, ProductionDate = new DateTime(2025, 5, 30) }
                };
                foreach (var product in products)
                {
                    if (!await dbContext.Products.AnyAsync(p => p.Id == product.Id))
                    {
                        dbContext.Products.Add(product);
                    }
                }
                await dbContext.SaveChangesAsync();

                // Seed product reviews
                Console.WriteLine("Seeding product reviews...");
                var reviews = new[]
                {
                    new ProductReview { ProductId = 1, FarmerId = farmerIds["jane@example.com"], Rating = 4, Comment = "Great for small farms, reliable performance.", CreatedDate = new DateTime(2025, 5, 14) },
                    new ProductReview { ProductId = 2, FarmerId = farmerIds["sam@example.com"], Rating = 5, Comment = "Excellent wind turbine, easy to install.", CreatedDate = new DateTime(2025, 5, 14) },
                    new ProductReview { ProductId = 3, FarmerId = farmerIds["mary@example.com"], Rating = 3, Comment = "Good biogas system, but setup was complex.", CreatedDate = new DateTime(2025, 5, 14) },
                    new ProductReview { ProductId = 4, FarmerId = farmerIds["tom@example.com"], Rating = 4, Comment = "Really improved my crop yields!", CreatedDate = new DateTime(2025, 5, 14) },
                    new ProductReview { ProductId = 5, FarmerId = farmerIds["emma@example.com"], Rating = 5, Comment = "Hydroponics made easy with this kit.", CreatedDate = new DateTime(2025, 5, 14) },
                    new ProductReview { ProductId = 6, FarmerId = farmerIds["liam@example.com"], Rating = 4, Comment = "Solar dryer works well, saves time.", CreatedDate = new DateTime(2025, 5, 14) },
                    new ProductReview { ProductId = 7, FarmerId = farmerIds["olivia@example.com"], Rating = 3, Comment = "Pump is effective, but maintenance is high.", CreatedDate = new DateTime(2025, 5, 14) },
                    new ProductReview { ProductId = 8, FarmerId = farmerIds["noah@example.com"], Rating = 4, Comment = "Keeps my greenhouse warm efficiently.", CreatedDate = new DateTime(2025, 5, 14) },
                    new ProductReview { ProductId = 9, FarmerId = farmerIds["ava@example.com"], Rating = 5, Comment = "Drip irrigation is a game-changer!", CreatedDate = new DateTime(2025, 5, 14) },
                    new ProductReview { ProductId = 10, FarmerId = farmerIds["john@example.com"], Rating = 4, Comment = "Compost turner saves a lot of effort.", CreatedDate = new DateTime(2025, 5, 14) }
                };
                foreach (var review in reviews)
                {
                    if (!await dbContext.ProductReviews.AnyAsync(r => r.ProductId == review.ProductId && r.FarmerId == review.FarmerId))
                    {
                        dbContext.ProductReviews.Add(review);
                    }
                }
                await dbContext.SaveChangesAsync();

                // Seed discussion posts
                Console.WriteLine("Seeding discussion posts...");
                var posts = new[]
                {
                    new DiscussionPost { Id = 1, FarmerId = farmerIds["john@example.com"], Title = "Organic Farming Tips", Content = "Share your best practices for organic farming!", CreatedAt = DateTime.UtcNow.AddDays(-10) },
                    new DiscussionPost { Id = 2, FarmerId = farmerIds["jane@example.com"], Title = "Water Conservation", Content = "How do you manage water usage on your farm?", CreatedAt = DateTime.UtcNow.AddDays(-9) },
                    new DiscussionPost { Id = 3, FarmerId = farmerIds["sam@example.com"], Title = "Solar Energy Benefits", Content = "Discuss the advantages of solar power for farms.", CreatedAt = DateTime.UtcNow.AddDays(-8) },
                    new DiscussionPost { Id = 4, FarmerId = farmerIds["mary@example.com"], Title = "Soil Health Strategies", Content = "What methods do you use to improve soil fertility?", CreatedAt = DateTime.UtcNow.AddDays(-7) },
                    new DiscussionPost { Id = 5, FarmerId = farmerIds["tom@example.com"], Title = "Wind Turbine Maintenance", Content = "Tips for maintaining farm wind turbines?", CreatedAt = DateTime.UtcNow.AddDays(-6) },
                    new DiscussionPost { Id = 6, FarmerId = farmerIds["emma@example.com"], Title = "Crop Rotation Benefits", Content = "How has crop rotation helped your yields?", CreatedAt = DateTime.UtcNow.AddDays(-5) },
                    new DiscussionPost { Id = 7, FarmerId = farmerIds["liam@example.com"], Title = "Biogas for Small Farms", Content = "Is biogas viable for small-scale farms?", CreatedAt = DateTime.UtcNow.AddDays(-4) },
                    new DiscussionPost { Id = 8, FarmerId = farmerIds["olivia@example.com"], Title = "Sustainable Packaging", Content = "What eco-friendly packaging do you use?", CreatedAt = DateTime.UtcNow.AddDays(-3) },
                    new DiscussionPost { Id = 9, FarmerId = farmerIds["noah@example.com"], Title = "Hydroponics Challenges", Content = "What challenges have you faced with hydroponics?", CreatedAt = DateTime.UtcNow.AddDays(-2) },
                    new DiscussionPost { Id = 10, FarmerId = farmerIds["ava@example.com"], Title = "Regenerative Agriculture", Content = "Share your regenerative farming practices.", CreatedAt = DateTime.UtcNow.AddDays(-1) },
                    new DiscussionPost { Id = 11, FarmerId = farmerIds["john@example.com"], Title = "Pest Control Methods", Content = "What natural pest control do you recommend?", CreatedAt = DateTime.UtcNow.AddHours(-23) },
                    new DiscussionPost { Id = 12, FarmerId = farmerIds["jane@example.com"], Title = "Composting Techniques", Content = "How do you optimize your compost?", CreatedAt = DateTime.UtcNow.AddHours(-20) },
                    new DiscussionPost { Id = 13, FarmerId = farmerIds["sam@example.com"], Title = "Solar Panel Costs", Content = "Are solar panels worth the investment?", CreatedAt = DateTime.UtcNow.AddHours(-18) },
                    new DiscussionPost { Id = 14, FarmerId = farmerIds["mary@example.com"], Title = "Cover Crops", Content = "Which cover crops work best for you?", CreatedAt = DateTime.UtcNow.AddHours(-15) },
                    new DiscussionPost { Id = 15, FarmerId = farmerIds["tom@example.com"], Title = "Wind Energy Grants", Content = "Any grants for wind energy projects?", CreatedAt = DateTime.UtcNow.AddHours(-12) },
                    new DiscussionPost { Id = 16, FarmerId = farmerIds["emma@example.com"], Title = "Organic Certification", Content = "How hard is it to get certified organic?", CreatedAt = DateTime.UtcNow.AddHours(-10) },
                    new DiscussionPost { Id = 17, FarmerId = farmerIds["liam@example.com"], Title = "Biomass Energy", Content = "Anyone using biomass for energy?", CreatedAt = DateTime.UtcNow.AddHours(-8) },
                    new DiscussionPost { Id = 18, FarmerId = farmerIds["olivia@example.com"], Title = "Drip Irrigation Setup", Content = "Tips for setting up drip irrigation?", CreatedAt = DateTime.UtcNow.AddHours(-6) },
                    new DiscussionPost { Id = 19, FarmerId = farmerIds["noah@example.com"], Title = "Soil Testing", Content = "How often do you test your soil?", CreatedAt = DateTime.UtcNow.AddHours(-4) },
                    new DiscussionPost { Id = 20, FarmerId = farmerIds["ava@example.com"], Title = "Farm Tech Innovations", Content = "What new tech are you using on your farm?", CreatedAt = DateTime.UtcNow.AddHours(-2) }
                };
                foreach (var post in posts)
                {
                    if (!await dbContext.DiscussionPosts.AnyAsync(p => p.Id == post.Id))
                    {
                        dbContext.DiscussionPosts.Add(post);
                    }
                }
                await dbContext.SaveChangesAsync();

                // Seed comments
                Console.WriteLine("Seeding comments...");
                var comments = new[]
                {
                    new Comment { Id = 1, DiscussionPostId = 1, FarmerId = farmerIds["jane@example.com"], Content = "Great tips! I use crop rotation to maintain soil health.", CreatedAt = DateTime.UtcNow.AddDays(-9.5) },
                    new Comment { Id = 2, DiscussionPostId = 2, FarmerId = farmerIds["sam@example.com"], Content = "Drip irrigation has saved us a lot of water!", CreatedAt = DateTime.UtcNow.AddDays(-8.5) },
                    new Comment { Id = 3, DiscussionPostId = 3, FarmerId = farmerIds["mary@example.com"], Content = "Solar panels cut our energy costs by 40%.", CreatedAt = DateTime.UtcNow.AddDays(-7.5) },
                    new Comment { Id = 4, DiscussionPostId = 1, FarmerId = farmerIds["tom@example.com"], Content = "I also add compost regularly.", CreatedAt = DateTime.UtcNow.AddDays(-7) },
                    new Comment { Id = 5, DiscussionPostId = 2, FarmerId = farmerIds["emma@example.com"], Content = "Rainwater harvesting is key for us.", CreatedAt = DateTime.UtcNow.AddDays(-6.5) },
                    new Comment { Id = 6, DiscussionPostId = 3, FarmerId = farmerIds["liam@example.com"], Content = "Agreed, solar is a game-changer!", CreatedAt = DateTime.UtcNow.AddDays(-6) },
                    new Comment { Id = 7, DiscussionPostId = 4, FarmerId = farmerIds["olivia@example.com"], Content = "Cover crops have improved my soil.", CreatedAt = DateTime.UtcNow.AddDays(-5.5) },
                    new Comment { Id = 8, DiscussionPostId = 5, FarmerId = farmerIds["noah@example.com"], Content = "Regular maintenance is crucial.", CreatedAt = DateTime.UtcNow.AddDays(-5) }
                    // Rest of comments unchanged
                };
                foreach (var comment in comments)
                {
                    if (!await dbContext.Comments.AnyAsync(c => c.Id == comment.Id))
                    {
                        dbContext.Comments.Add(comment);
                    }
                }
                await dbContext.SaveChangesAsync();

                // Seed funding opportunities
                Console.WriteLine("Seeding funding opportunities...");
                var fundingOpportunities = new[]
                {
                    new FundingOpportunity
                    {
                        Title = "Green Farming Grant",
                        Description = "Up to R180,000 for solar installations on farms.",
                        Amount = 180000,
                        Source = "Government",
                        ApplicationUrl = "https://example.com/grants"
                    },
                    new FundingOpportunity
                    {
                        Title = "Biogas Subsidy",
                        Description = "50% cost coverage for farm biogas systems, up to R90,000.",
                        Amount = 90000,
                        Source = "NGO",
                        ApplicationUrl = "https://example.com/subsidies"
                    }
                };
                foreach (var funding in fundingOpportunities)
                {
                    if (!await dbContext.FundingOpportunities.AnyAsync(f => f.Title == funding.Title))
                    {
                        dbContext.FundingOpportunities.Add(funding);
                    }
                }
                await dbContext.SaveChangesAsync();

                // Seed projects
                Console.WriteLine("Seeding projects...");
                var projects = new[]
                {
                    new Project
                    {
                        FarmerId = farmerIds["john@example.com"],
                        Title = "Solar Irrigation System",
                        Description = "Implement solar-powered irrigation for 50 acres.",
                        Category = "Solar",
                        FundingGoal = 270000,
                        Status = "Open",
                        CreatedDate = DateTime.UtcNow
                    },
                    new Project
                    {
                        FarmerId = farmerIds["john@example.com"],
                        Title = "Biogas Pilot",
                        Description = "Convert farm waste to biogas for energy.",
                        Category = "Biogas",
                        FundingGoal = 144000,
                        Status = "Open",
                        CreatedDate = DateTime.UtcNow
                    }
                };
                foreach (var project in projects)
                {
                    if (!await dbContext.Projects.AnyAsync(p => p.Title == project.Title))
                    {
                        dbContext.Projects.Add(project);
                    }
                }
                await dbContext.SaveChangesAsync();

                Console.WriteLine("Seeding finished successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}