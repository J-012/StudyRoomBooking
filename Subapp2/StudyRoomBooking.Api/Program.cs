using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StudyRoomBooking.Api.DAL;
using StudyRoomBooking.Api.Repositories;
using StudyRoomBooking.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<StudyRoomBookingDbContext>(options =>
{
    string? connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseSqlite(connectionString);
});

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

using (IServiceScope scope = app.Services.CreateScope())
{
    StudyRoomBookingDbContext context =
        scope.ServiceProvider.GetRequiredService<StudyRoomBookingDbContext>();

    await context.Database.MigrateAsync();
}

app.UseAuthorization();

app.MapControllers();

app.Run();