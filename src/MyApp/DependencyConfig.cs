using System;
using System.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Data.Repositories;
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
            .AddMvc().AddFluentValidation();

        private static IServiceCollection AddValidators(this IServiceCollection services) => services
            .AddTransient<IValidator<Pizza>, PizzaValidator>()
            .AddTransient<IValidator<Topping>, ToppingValidator>();

        private static IServiceCollection AddRepositories(this IServiceCollection services) => services
            .AddScoped<IDbConnection>((s) => new NpgsqlConnection(ConnectionString))
            .AddScoped<IPizzaRepository, PizzaRepository>()
            .AddScoped<IToppingRepository, ToppingRepository>();
    }
}