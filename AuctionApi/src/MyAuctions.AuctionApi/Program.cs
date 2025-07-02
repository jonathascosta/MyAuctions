using AuctionApi.Repositories;
using AuctionApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyAuctions.AuctionApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var cfg = builder.Configuration;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = cfg["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = cfg["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!)),
                    ValidateLifetime = true
                };
            });

        builder.Services.AddAuthorization(opts =>
        {
            opts.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
            opts.AddPolicy("BidderOnly", p => p.RequireRole("Bidder"));
        });

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IVehicleRepository, InMemoryVehicleRepository>();
        builder.Services.AddScoped<IVehicleService, VehicleService>();
        builder.Services.AddSingleton<IAuctionRepository, InMemoryAuctionRepository>();
        builder.Services.AddScoped<IAuctionService, AuctionService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}
