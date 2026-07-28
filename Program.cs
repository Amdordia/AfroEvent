using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AfroEvent.Models;
using AfroEvent.Data;
using AfroEvent.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Augmenter la limite des headers pour éviter HTTP 431 en dev (accumulation cookies Identity)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestHeadersTotalSize = 131072; // 128 KB au lieu de 32 KB par défaut
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Requis pour les pages Identity scaffoldées (Areas/Identity)

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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Enregistrement du service d'envoi d'email factice pour Identity
builder.Services.AddSingleton<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, DummyEmailSender>();

builder.Services.AddSignalR();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register AfroEvent Business Services (Couche Métier)
// Scoped = une instance par requête HTTP (compatibilité DbContext + UserManager Identity)
builder.Services.AddScoped<AfroEvent.Services.Interfaces.IEventService, AfroEvent.Services.Implementations.EventService>();
builder.Services.AddScoped<AfroEvent.Services.Interfaces.IOrganizerService, AfroEvent.Services.Implementations.OrganizerService>();
builder.Services.AddScoped<AfroEvent.Services.Interfaces.IParticipantService, AfroEvent.Services.Implementations.ParticipantService>();
// AdminService dépend de UserManager<AppUser> (Scoped) → doit être Scoped
builder.Services.AddScoped<AfroEvent.Services.Interfaces.IAdminService, AfroEvent.Services.Implementations.AdminService>();

var app = builder.Build();

// Seeding des rôles et du compte Admin ainsi que des autres données de test
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
app.MapRazorPages(); // Active le routing des pages Identity (Login, Register, Logout)

app.Run();

// Dummy EmailSender required by scaffolded ASP.NET Core Identity UI
public class DummyEmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
    }
}
