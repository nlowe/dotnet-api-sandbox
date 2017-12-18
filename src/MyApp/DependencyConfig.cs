using System;
using System.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyApp.Data.Repositories;
using MyApp.Filters;
using MyApp.Types.Models;
using MyApp.Types.Repositories;
using MyApp.Validation;
using Npgsql;

namespace MyApp
{
    internal static class DependencyConfig
    {
        private const string DEFAULT_CONNECTION_STRING = "Server=localhost;User ID=postgres;Database=postgres";
        private const string CONNECTION_STRING_ENV = "MYAPP_CONNECTION_STRING";
        
        private static Lazy<string> _connectionString = new Lazy<string>(() => Environment.GetEnvironmentVariable(CONNECTION_STRING_ENV) ?? DEFAULT_CONNECTION_STRING); 
        public static string ConnectionString => _connectionString.Value;

        public static void AddMyAppServices(this IServiceCollection services) => services
            .AddValidators()
            .AddRepositories()
            .AddMvc(options =>
            {
                options.Filters.Add(new ApiModelValidationFilter());
            }).AddFluentValidation();

        private static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.TryAddTransient<IValidator<Pizza>, PizzaValidator>();
            services.TryAddTransient<IValidator<Topping>, ToppingValidator>();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.TryAddScoped<IDbConnection>((s) => new NpgsqlConnection(ConnectionString));
            services.TryAddScoped<IPizzaRepository, PizzaRepository>();
            services.TryAddScoped<IToppingRepository, ToppingRepository>();
            
            return services;
        }
    }
}