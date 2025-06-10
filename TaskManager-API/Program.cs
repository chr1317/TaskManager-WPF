using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using TaskManager_API.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TaskManager_API.AppDbContext>(options  =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(10, 4, 28))));

builder.Services.AddControllers();




var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TaskManager_API.AppDbContext>();

    if (!context.Users.Any(u => u.Username == "admin"))
    {
        var passwordHasher = new PasswordHasher<Users>();
        var admin = new Users
        {
            Username = "admin"
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, "admin123");

        context.Users.Add(admin);
        context.SaveChanges();
    }
    if (!context.Categories.Any())
    {
        context.Categories.AddRange(new List<Category>
    {
        new Category { Id = 1, Name = "Dom" },
        new Category { Id = 2, Name = "Praca" }
    });
        context.SaveChanges();
    }
    if (!context.Tasks.Any(t => t.UserId == 5))
    {
        var parentTask = new TaskItem
        {
            Title = "Pierwsze seedowane zadanie",
            Description = "To jest przykładowe zadanie seedowane.",
            UserId = 5,
            CategoryId = 1,
            Priority = PriorityLevel.High,
            Status = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(2)
        };

        var secondTask = new TaskItem
        {
            Title = "Drugie seedowane zadanie",
            Description = "Drugie zadanie przykładowe.",
            UserId = 5,
            CategoryId = 2,
            Priority = PriorityLevel.High,
            Status = 0,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(4)
        };

        context.Tasks.AddRange(parentTask, secondTask);
        context.SaveChanges();

        var subTask = new TaskItem
        {
            Title = "Podzadanie do pierwszego",
            Description = "To jest podzadanie.",
            UserId = 5,
            CategoryId = 2,
            Priority = PriorityLevel.Medium,
            Status = 0,
            PID = parentTask.Id,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(4)
        };

        context.Tasks.Add(subTask);
        context.SaveChanges();
    }




//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();



    app.Run();
}

