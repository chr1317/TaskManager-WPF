using Pomelo.EntityFrameworkCore.MySql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options  =>
    options.UseMySql(connectionString, new MysqlServerVersion(new Version(10, 4, 28))));