using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DesafioTecnico_Ache.API.Middleware;

/// <summary>
/// Middleware para autenticação via API Key
/// Implementa OWASP API2: Broken Authentication
/// </summary>
public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;
    private readonly IConfiguration _configuration;
    
    private const string API_KEY_HEADER_NAME = "X-API-Key";

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<ApiKeyAuthenticationMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Endpoints públicos que não requerem autenticação
        var publicPaths = new[]
        {
            "/",
            "/swagger",
            "/health"
        };

        // Verifica se é um endpoint público
        if (publicPaths.Any(path => context.Request.Path.StartsWithSegments(path)))
        {
            await _next(context);
            return;
        }

        // Verifica se o header X-API-Key está presente
        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
        {
            _logger.LogWarning(
                "Tentativa de acesso sem API Key | Path: {Path} | IP: {IP}",
                context.Request.Path,
                context.Connection.RemoteIpAddress);

            await ReturnUnauthorizedResponse(context, "API Key não fornecida");
            return;
        }

        // Valida a API Key
        var validApiKeys = _configuration.GetSection("Authentication:ApiKeys").Get<string[]>() 
                          ?? Array.Empty<string>();

        if (!validApiKeys.Contains(extractedApiKey.ToString()))
        {
            _logger.LogWarning(
                "Tentativa de acesso com API Key inválida | Path: {Path} | IP: {IP} | ApiKey: {ApiKey}",
                context.Request.Path,
                context.Connection.RemoteIpAddress,
                MaskApiKey(extractedApiKey.ToString()));

            await ReturnUnauthorizedResponse(context, "API Key inválida");
            return;
        }

        // API Key válida - adiciona informação ao contexto
        context.Items["ApiKey"] = extractedApiKey.ToString();
        
        _logger.LogInformation(
            "Acesso autenticado com sucesso | Path: {Path}",
            context.Request.Path);

        await _next(context);
    }

    private static async Task ReturnUnauthorizedResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.Unauthorized,
            Title = "Não autorizado",
            Detail = message,
            Instance = context.Request.Path,
            Extensions = new Dictionary<string, object?>
            {
                ["timestamp"] = DateTime.UtcNow,
                ["hint"] = "Forneça uma API Key válida no header 'X-API-Key'"
            }
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static string MaskApiKey(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey.Length <= 8)
            return "***";

        return $"{apiKey[..4]}...{apiKey[^4..]}";
    }
}
