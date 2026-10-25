using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using StudyRoomBooking.Repositories;
using StudyRoomBooking.Services;

var builder = WebApplication.CreateBuilder(args);

// Legg til tjenester i containeren.
builder.Services.AddDbContext<StudyRoomDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Legg til MVC-tjenester.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IRoomService, RoomService>();

// Cookie authentication
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Utfør database-migrering ved oppstart.
using (IServiceScope scope = app.Services.CreateScope())
{
    StudyRoomDbContext context =
        scope.ServiceProvider.GetRequiredService<StudyRoomDbContext>();

    await context.Database.MigrateAsync();
}

// Konfigurer HTTP-forespørselsrørledningen.
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

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
