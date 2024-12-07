﻿using Dating.API.Errors;
using Dating.Data.IServices;
using Dating.Repository.Data;
using Dating.Service;
using Microsoft.AspNetCore.Mvc;
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

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                var errors = actionContext.ModelState.Where(x => x.Value!.Errors.Any())
                    .SelectMany(x => x.Value!.Errors).Select(x => x.ErrorMessage);
                var response = new ApiValidationErrorResponse()
                {
                    Errors = errors
                };
                return new BadRequestObjectResult(response);
            };
        });

        
        return services;
    }
}