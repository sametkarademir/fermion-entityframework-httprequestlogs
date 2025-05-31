using System.Reflection;
using Fermion.EntityFramework.HttpRequestLogs.DependencyInjection;
using Fermion.Extensions.ServiceCollections;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi.Contexts;
using WebApi.Repositories;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"] ?? string.Empty;
builder.Services.AddDbContextFactory<ApplicationDbContext>(opt => opt.UseNpgsql(connectionString), ServiceLifetime.Scoped);

builder.Services.AddFermionHttpRequestLogServices<ApplicationDbContext>(opt =>
{
    opt.Enabled = true;
    opt.ExcludedPaths = ["/api/health"];
    opt.ExcludedHttpMethods = ["OPTIONS"];
    opt.ExcludedContentTypes = ["application/octet-stream", "application/pdf", "image/", "video/", "audio/"];
    opt.LogRequestBody = true;
    opt.MaxRequestBodyLength = 5000;
    opt.LogOnlySlowRequests = true;
    opt.SlowRequestThresholdMs = 10;
    opt.MaskPattern = "***MASKED***";
    opt.RequestBodySensitiveProperties = ["Password", "Token", "Secret", "Key", "Credential", "Ssn", "Credit", "Card", "Description"];
    opt.QueryStringSensitiveProperties = ["Password", "Token", "Secret", "ApiKey", "Key"];
    opt.HeaderSensitiveProperties = ["Authorization", "Cookie", "X-Api-Key"];
    opt.Authorization.RequireAuthentication = false;
    opt.Authorization.GlobalPolicy = null;
    opt.Authorization.EndpointPolicies = null;
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddSwaggerDocumentation(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.FermionHttpRequestLogMiddleware();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();