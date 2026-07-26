using Microsoft.EntityFrameworkCore;
using AfroEvent.Data;
using AfroEvent.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register EF Core DbContext
builder.Services.AddDbContext<AfroEventDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.UseAuthorization();

app.MapStaticAssets();
app.MapHub<EventHub>("/eventHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
