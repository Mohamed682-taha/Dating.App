using Dating.Data.IServices;
using Dating.Repository.Data;
using Dating.Service;
using Microsoft.EntityFrameworkCore;

namespace Dating.API.Extensions;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddDbContext<DatingDbContext>(opt =>
        {
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }
}