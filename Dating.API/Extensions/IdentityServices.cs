using System.Text;
using Dating.Data.Entities;
using Dating.Repository.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Dating.API.Extensions;

public static class IdentityServices
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentityCore<AppUser>(options => { options.Password.RequireNonAlphanumeric = false; })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<DatingDbContext>();

        services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
            .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                var key = config["JWT:secretKey"] ?? "key does not exist";
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
                opt.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }
}