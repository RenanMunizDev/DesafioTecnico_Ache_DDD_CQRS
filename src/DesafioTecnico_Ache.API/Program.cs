using DesafioTecnico_Ache.API.Middleware;
using DesafioTecnico_Ache.Application.Behaviors;
using DesafioTecnico_Ache.Application.Validators;
using DesafioTecnico_Ache.Application.Commands;
using DesafioTecnico_Ache.Domain.Interfaces;
using DesafioTecnico_Ache.Infrastructure.Repositories;
using DesafioTecnico_Ache.Infrastructure.SAP.Services;
using FluentValidation;
using MediatR;
using AspNetCoreRateLimit;
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAÇÃO DE LOGGING =====
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ===== CONFIGURAÇÃO DE CONTROLLERS =====
builder.Services.AddControllers();

// ===== CONFIGURAÇÃO DO MEDIATR (CQRS) =====
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CriarPedidoVendaCommand).Assembly);
});

// Adiciona comportamento de validação automática
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// ===== CONFIGURAÇÃO DO FLUENTVALIDATION =====
builder.Services.AddValidatorsFromAssembly(
    typeof(CriarPedidoVendaCommandValidator).Assembly);

// ===== CONFIGURAÇÃO DE DEPENDENCY INJECTION (DI) =====
// Repositories
builder.Services.AddScoped<IPedidoVendaRepository, PedidoVendaRepository>();

// SAP Services
builder.Services.AddSingleton<ISapODataService, SapODataService>();

// ===== CONFIGURAÇÃO DE RATE LIMITING (OWASP: API4) =====
builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.ClientIdHeader = "X-ClientId";
    
    // Endpoints que não devem ter Rate Limiting
    options.EndpointWhitelist = new List<string>
    {
        "get:/",
        "get:/swagger*",
        "*:/health"
    };
    
    options.GeneralRules = new List<RateLimitRule>
    {
        new()
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 30 // 30 requisições por minuto
        },
        new()
        {
            Endpoint = "*",
            Period = "1h",
            Limit = 500 // 500 requisições por hora
        }
    };
});

builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

// ===== CONFIGURAÇÃO DO SWAGGER/OPENAPI =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SAP S/4HANA SD Integration API",
        Version = "v1",
        Description = @"API REST para integração com módulo SD (Sales & Distribution) do SAP S/4HANA.
        
**Tipo de Integração**: OData/REST
**Serviço SAP**: API_SALES_ORDER_SRV
**Protocolo**: HTTPS
**Autenticação**: API Key (via header X-API-Key)

Esta API implementa as melhores práticas de:
- Clean Architecture
- Domain-Driven Design (DDD)
- CQRS (Command Query Responsibility Segregation)
- SOLID Principles
- OWASP API Security Top 10

**Segurança OWASP implementada**:
- API1: Broken Object Level Authorization
- API2: Broken Authentication (API Key Authentication)
- API3: Broken Object Property Level Authorization  
- API4: Unrestricted Resource Consumption (Rate Limiting)
- API8: Security Misconfiguration

**Como usar a autenticação**:
1. Obtenha uma API Key válida
2. Adicione o header: X-API-Key: sua-chave-aqui
3. Faça suas requisições normalmente
",
        Contact = new OpenApiContact
        {
            Name = "Equipe de Desenvolvimento",
            Email = "dev@ache.com.br"
        }
    });

    // Configuração de segurança para API Key
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-API-Key",
        Description = "Insira sua API Key no campo abaixo. Exemplo: dev-test-key-ache-2024",
        Scheme = "ApiKeyScheme"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// ===== CONFIGURAÇÃO DE CORS (OWASP: Security Misconfiguration) =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:5001") // Em produção, configurar origins específicas
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("X-Request-Id", "X-Correlation-Id");
    });
});

// ===== CONFIGURAÇÃO DE HEALTHCHECKS =====
builder.Services.AddHealthChecks();

var app = builder.Build();

// ===== CONFIGURAÇÃO DO PIPELINE DE MIDDLEWARES =====

// Swagger DEVE vir ANTES dos outros middlewares
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SAP SD Integration API v1");
    options.RoutePrefix = string.Empty; // Swagger na raiz
    options.DocumentTitle = "SAP S/4HANA SD Integration API";
});

// HTTPS Redirect - deve vir antes dos outros middlewares
app.UseHttpsRedirection();

// Middleware de logging de requisições
app.UseMiddleware<RequestLoggingMiddleware>();

// Middleware de segurança headers (OWASP)
app.UseMiddleware<SecurityHeadersMiddleware>();

// Middleware de autenticação API Key (OWASP: API2)
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

// Rate Limiting (OWASP: API4) - antes do exception handler
app.UseIpRateLimiting();

// CORS
app.UseCors("DefaultPolicy");

// Middleware de tratamento global de exceções - deve ser um dos últimos
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Controllers
app.MapControllers();

// Health Check endpoint
app.MapHealthChecks("/health");

app.Logger.LogInformation("===========================================");
app.Logger.LogInformation("SAP S/4HANA SD Integration API iniciada");
app.Logger.LogInformation("Tipo de Integração: OData/REST");
app.Logger.LogInformation("Módulo SAP: SD (Sales & Distribution)");
app.Logger.LogInformation("Ambiente: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("URL Swagger: Acesse a raiz da aplicação");
app.Logger.LogInformation("Autenticação: API Key via header X-API-Key");
app.Logger.LogInformation("===========================================");

app.Run();
