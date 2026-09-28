using Gym_Inventory_Management_System.Context;
using Gym_Inventory_Management_System.Data;
using Gym_Inventory_Management_System.Helpers;
using Gym_Inventory_Management_System.IServices;
using Gym_Inventory_Management_System.Middleware;
using Gym_Inventory_Management_System.Models;
using Gym_Inventory_Management_System.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;

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

        builder.Services.AddJwtAuthentication(builder.Configuration);

        builder.Services.AddScoped<ICategoriesService, CategoriesService>();
        builder.Services.AddScoped<ILocationsService, LocationsService>();
        builder.Services.AddScoped<IEquipmentService, EquipmentService>();
        builder.Services.AddScoped<IConsumablesService, ConsumablesService>();
        builder.Services.AddScoped<IStockMovementsService, StockMovementsService>();
        builder.Services.AddScoped<IStockTransfersService, StockTransfersService>();
        builder.Services.AddScoped<IStockLevelHelperService, StockLevelHelperService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<JwtTokenGenerator>();
        builder.Services.AddScoped<PasswordHasher<User>>();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' followed by a space and your JWT."
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>(true, "Bearer");
        });

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

        var app = builder.Build();

        /*
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DbSeeder.Seed(context);
        }
        */

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandling();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }
}
