using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DI;
public static class DIRegister
{
    public static void ConfigureInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<AppDbContext>(options =>
           options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), option =>
           {
               option.CommandTimeout(45);
           }));

        services.AddScoped<DbContext, AppDbContext>();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                o => o.EnableRetryOnFailure()
            );

            options.LogTo(
                Console.WriteLine,
                LogLevel.Information
            );
        });
    }
}
