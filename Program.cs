using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.IServicesl;
using Gym_Inventory_Management_System.Middleware;
using Gym_Inventory_Management_System.Services;
using Microsoft.EntityFrameworkCore;

namespace Gym_Inventory_Management_System;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string"
            + "'DefaultConnection' not found.");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddScoped<ICategoriesService, CategoriesService>();
        builder.Services.AddScoped<ILocationsService, LocationsService>();
        builder.Services.AddScoped<IEquipmentService, EquipmentService>();
        builder.Services.AddScoped<IConsumablesService, ConsumablesService>();
        builder.Services.AddScoped<IStockMovementsService, StockMovementsService>();
        builder.Services.AddScoped<IStockTransfersService, StockTransfersService>();
        builder.Services.AddScoped<IStockLevelHelperService, StockLevelHelperService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandling(); 

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

