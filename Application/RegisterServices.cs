using Application.Abstraction;
using Application.Services;
using FluentValidation.AspNetCore;
using Invoice.Applicaion.CQRS.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class RegisterServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddFluentValidation(opt => opt.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddMediatR(config=> config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddLazyCache();
        services.AddScoped<ITokenService, TokenService>();

        return services;

    }
}