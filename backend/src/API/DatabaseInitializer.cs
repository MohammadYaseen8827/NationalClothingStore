using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NationalClothingStore.Infrastructure.Data;
using Npgsql;

namespace NationalClothingStore.API
{
    public class DatabaseInitializer
    {
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NationalClothingStoreDbContext>();
            
            try
            {
                Console.WriteLine("Initializing database...");
                
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();
                
                Console.WriteLine("Database ensured.");
                
                try
                {
                    // Check if migrations need to be applied
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        Console.WriteLine($"Found {pendingMigrations.Count()} pending migrations. Applying...");
                        
                        // Apply pending migrations
                        await context.Database.MigrateAsync();
                        
                        Console.WriteLine("Migrations applied successfully.");
                    }
                    else
                    {
                        Console.WriteLine("No pending migrations.");
                    }
                }
                catch (Npgsql.PostgresException ex) when (ex.SqlState == "28P01") // Invalid password
                {
                    Console.WriteLine($"Authentication failed: {ex.Message}");
                    Console.WriteLine("Skipping database initialization due to authentication issues.");
                    Console.WriteLine("Please ensure PostgreSQL is running and credentials are correct in appsettings.");
                    return; // Exit gracefully without throwing
                }
                catch (Exception migrateEx)
                {
                    Console.WriteLine($"Error applying migrations: {migrateEx.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                Console.WriteLine("Database initialization skipped. The application will continue running without database access.");
                Console.WriteLine("To set up the database:");
                Console.WriteLine("1. Install PostgreSQL server");
                Console.WriteLine("2. Create the database 'NationalClothingStore_Dev'");
                Console.WriteLine("3. Ensure the connection string in appsettings is correct");
                // Don't throw the exception - let the app continue running
                return;
            }
        }
    }
}