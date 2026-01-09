namespace DesafioTecnico_Ache.API.Middleware;

/// <summary>
/// Middleware para adicionar headers de segurança conforme recomendações OWASP
/// Implementa proteções contra diversos ataques:
/// - X-Content-Type-Options: Previne MIME sniffing
/// - X-Frame-Options: Previne Clickjacking
/// - X-XSS-Protection: Proteção XSS (legacy)
/// - Strict-Transport-Security: Force HTTPS
/// - Content-Security-Policy: Previne XSS e injection
/// - Referrer-Policy: Controla informações de referência
/// - Permissions-Policy: Controla recursos do navegador
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(
        RequestDelegate next,
        ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // OWASP: Security Misconfiguration - Security Headers

        // Não aplicar CSP restritivo para Swagger
        var isSwaggerPath = context.Request.Path.StartsWithSegments("/swagger") || 
                            context.Request.Path.StartsWithSegments("/swagger-ui") ||
                            context.Request.Path == "/";

        if (!isSwaggerPath)
        {
            // Previne MIME sniffing
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // Previne Clickjacking
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // Proteção XSS (para navegadores antigos)
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Content Security Policy - Restritiva para API
            context.Response.Headers.Append(
                "Content-Security-Policy",
                "default-src 'none'; frame-ancestors 'none'");
        }
        else
        {
            // CSP mais permissivo para Swagger
            context.Response.Headers.Append(
                "Content-Security-Policy",
                "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:");
        }

        // Force HTTPS (HSTS) - 1 ano
        if (context.Request.IsHttps)
        {
            context.Response.Headers.Append(
                "Strict-Transport-Security",
                "max-age=31536000; includeSubDomains; preload");
        }

        // Controla informações de referência
        context.Response.Headers.Append("Referrer-Policy", "no-referrer");

        // Permissions Policy - Desabilita recursos não necessários
        context.Response.Headers.Append(
            "Permissions-Policy",
            "geolocation=(), microphone=(), camera=()");

        // Remove header que expõe versão do servidor
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("X-AspNet-Version");

        await _next(context);
    }
}
