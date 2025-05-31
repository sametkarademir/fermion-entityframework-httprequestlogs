using System.Reflection;
using Fermion.Domain.Shared.Conventions;
using Fermion.EntityFramework.HttpRequestLogs.Application.Services;
using Fermion.EntityFramework.HttpRequestLogs.Core.Interfaces;
using Fermion.EntityFramework.HttpRequestLogs.Core.Options;
using Fermion.EntityFramework.HttpRequestLogs.Infrastructure.Repositories;
using Fermion.EntityFramework.HttpRequestLogs.Presentation.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fermion.EntityFramework.HttpRequestLogs.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFermionHttpRequestLogServices<TContext>(this IServiceCollection services, Action<HttpRequestLogOptions> configureOptions) where TContext : DbContext
    {
        var options = new HttpRequestLogOptions();
        configureOptions.Invoke(options);
        services.Configure<HttpRequestLogOptions>(opt => configureOptions.Invoke(opt));

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddHttpContextAccessor();

        services.AddScoped<IHttpRequestLogRepository, HttpRequestLogRepository<TContext>>();
        services.AddScoped<IHttpRequestLogAppService, HttpRequestLogAppService>();

        if (options.EnableApiEndpoints)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(HttpRequestLogController).Assembly));
                });

            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerAuthorizationConvention(
                    controllerType: typeof(HttpRequestLogController),
                    route: options.ApiRoute,
                    requireAuthentication: options.Authorization.RequireAuthentication,
                    globalPolicy: options.Authorization.GlobalPolicy,
                    allowedRoles: options.Authorization.EndpointPolicies
                ));
            });
        }
        else
        {
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerDisablingConvention(typeof(HttpRequestLogController)));
            });
        }

        return services;
    }
}