namespace DesafioTecnico_Ache.API.Middleware;

/// <summary>
/// Middleware para logging de requisições e respostas
/// Implementa auditoria de todas as chamadas à API
/// OWASP: Security Logging and Monitoring
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString();
        context.Items["RequestId"] = requestId;

        // Log da requisição
        _logger.LogInformation(
            "[{RequestId}] Requisição recebida | Method: {Method} | Path: {Path} | IP: {IpAddress} | UserAgent: {UserAgent}",
            requestId,
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress,
            context.Request.Headers.UserAgent.ToString());

        var startTime = DateTime.UtcNow;

        try
        {
            await _next(context);
        }
        finally
        {
            var duration = DateTime.UtcNow - startTime;

            // Log da resposta
            _logger.LogInformation(
                "[{RequestId}] Resposta enviada | StatusCode: {StatusCode} | Duration: {Duration}ms",
                requestId,
                context.Response.StatusCode,
                duration.TotalMilliseconds);

            // Log de alerta para códigos de erro
            if (context.Response.StatusCode >= 400)
            {
                _logger.LogWarning(
                    "[{RequestId}] Requisição com erro | StatusCode: {StatusCode} | Path: {Path} | IP: {IpAddress}",
                    requestId,
                    context.Response.StatusCode,
                    context.Request.Path,
                    context.Connection.RemoteIpAddress);
            }
        }
    }
}
