using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AfroEvent.Models;
using AfroEvent.Data;
using AfroEvent.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register EF Core DbContext
builder.Services.AddDbContext<AfroEventDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- ASP.NET Core Identity ---
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AfroEventDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddSignalR();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register AfroEvent Business Services (Couche Métier)
builder.Services.AddSingleton<AfroEvent.Services.Interfaces.IEventService, AfroEvent.Services.Implementations.EventService>();
builder.Services.AddSingleton<AfroEvent.Services.Interfaces.IOrganizerService, AfroEvent.Services.Implementations.OrganizerService>();
builder.Services.AddSingleton<AfroEvent.Services.Interfaces.IParticipantService, AfroEvent.Services.Implementations.ParticipantService>();
builder.Services.AddSingleton<AfroEvent.Services.Interfaces.IAdminService, AfroEvent.Services.Implementations.AdminService>();

var app = builder.Build();

// Seeding des rôles et du compte Admin
using (var scope = app.Services.CreateScope())
{
    await AfroEvent.Data.DbSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapHub<EventHub>("/eventHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
