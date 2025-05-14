using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Log database path
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Using SQLite database at: {DatabasePath}", connectionString);

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("Applying migrations...");
        await context.Database.MigrateAsync(); // Replace EnsureCreated
        logger.LogInformation("Migrations applied. Seeding data...");
        await SeedData.Initialize(services);
        logger.LogInformation("Seeding completed.");
        var tables = await context.Database.SqlQueryRaw<string>("SELECT name FROM sqlite_master WHERE type='table'").ToListAsync();
        logger.LogInformation("Tables: {Tables}", string.Join(", ", tables));
        logger.LogInformation("Farmers: {Count}", await context.Farmers.CountAsync());
        logger.LogInformation("Products: {Count}", await context.Products.CountAsync());
        logger.LogInformation("Posts: {Count}", await context.DiscussionPosts.CountAsync());
        logger.LogInformation("Comments: {Count}", await context.Comments.CountAsync());
        logger.LogInformation("Projects: {Count}", await context.Projects.CountAsync());
        logger.LogInformation("Funding Opportunities: {Count}", await context.FundingOpportunities.CountAsync());
        logger.LogInformation("Users: {Count}", await context.Users.CountAsync());
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred seeding the DB.");
        throw;
    }
}

app.Run();