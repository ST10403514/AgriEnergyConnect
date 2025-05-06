using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Seed roles and test users
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Seed roles
    string[] roles = new[] { "Farmer", "Employee" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Seed a test Farmer user
    var farmerUser = await userManager.FindByEmailAsync("farmer@example.com");
    if (farmerUser == null)
    {
        farmerUser = new ApplicationUser
        {
            UserName = "farmer@example.com",
            Email = "farmer@example.com",
            Role = "Farmer"
        };
        await userManager.CreateAsync(farmerUser, "Farmer@123");
        await userManager.AddToRoleAsync(farmerUser, "Farmer");

        // Create a Farmer record
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Farmers.Add(new Farmer
        {
            Name = "Test Farmer",
            Email = "farmer@example.com",
            Address = "789 Farm Lane"
        });
        await dbContext.SaveChangesAsync();
    }

    // Seed a test Employee user
    var employeeUser = await userManager.FindByEmailAsync("employee@example.com");
    if (employeeUser == null)
    {
        employeeUser = new ApplicationUser
        {
            UserName = "employee@example.com",
            Email = "employee@example.com",
            Role = "Employee"
        };
        await userManager.CreateAsync(employeeUser, "Employee@123");
        await userManager.AddToRoleAsync(employeeUser, "Employee");
    }
}

app.Run();