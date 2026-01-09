using MediatR;
using Microsoft.AspNetCore.Mvc;
using DesafioTecnico_Ache.Application.Commands;
using DesafioTecnico_Ache.Application.Queries;
using DesafioTecnico_Ache.Application.DTOs;
using FluentValidation;

namespace DesafioTecnico_Ache.API.Controllers;

/// <summary>
/// Controller para operações de Pedidos de Venda (Sales Orders) - Módulo SD do SAP S/4HANA
/// Implementa integração via OData/REST com o SAP
/// 
/// Segurança OWASP implementada:
/// - API1: Broken Object Level Authorization (validação de acesso)
/// - API2: Broken Authentication (via middleware)
/// - API3: Broken Object Property Level Authorization (DTOs controlados)
/// - API4: Unrestricted Resource Consumption (rate limiting)
/// - API8: Security Misconfiguration (validações de entrada)
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class PedidosVendaController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PedidosVendaController> _logger;

    public PedidosVendaController(
        IMediator mediator,
        ILogger<PedidosVendaController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Consulta um pedido de venda no SAP S/4HANA pelo número do documento
    /// </summary>
    /// <param name="numeroDocumento">Número do documento SAP (8 dígitos)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Pedido de venda encontrado ou 404 se não existir</returns>
    /// <response code="200">Pedido encontrado com sucesso</response>
    /// <response code="400">Número do documento inválido</response>
    /// <response code="404">Pedido não encontrado</response>
    /// <response code="429">Muitas requisições (rate limit)</response>
    /// <response code="500">Erro interno no servidor ou SAP</response>
    [HttpGet("{numeroDocumento}")]
    [ProducesResponseType(typeof(PedidoVendaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PedidoVendaResponseDto>> ObterPedidoPorNumero(
        [FromRoute] string numeroDocumento,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Requisição GET recebida para pedido {NumeroDocumento} | IP: {IpAddress}",
            numeroDocumento,
            HttpContext.Connection.RemoteIpAddress);

        // OWASP: Input Validation - Sanitização de entrada
        if (string.IsNullOrWhiteSpace(numeroDocumento) || 
            numeroDocumento.Length > 10 ||
            !numeroDocumento.All(char.IsLetterOrDigit))
        {
            _logger.LogWarning(
                "Tentativa de acesso com número de documento inválido: {NumeroDocumento}",
                numeroDocumento);

            return BadRequest(new ProblemDetails
            {
                Title = "Número do documento inválido",
                Detail = "O número do documento deve conter apenas caracteres alfanuméricos e ter no máximo 10 caracteres",
                Status = StatusCodes.Status400BadRequest,
                Instance = HttpContext.Request.Path
            });
        }

        try
        {
            var query = new ObterPedidoVendaPorNumeroQuery(numeroDocumento);
            var resultado = await _mediator.Send(query, cancellationToken);

            if (resultado == null)
            {
                _logger.LogInformation("Pedido {NumeroDocumento} não encontrado", numeroDocumento);

                return NotFound(new ProblemDetails
                {
                    Title = "Pedido não encontrado",
                    Detail = $"O pedido com número {numeroDocumento} não foi encontrado no SAP",
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });
            }

            _logger.LogInformation(
                "Pedido {NumeroDocumento} retornado com sucesso. Cliente: {ClienteId}",
                numeroDocumento,
                resultado.ClienteId);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao processar consulta do pedido {NumeroDocumento}",
                numeroDocumento);

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Erro ao consultar pedido",
                Detail = "Ocorreu um erro ao consultar o pedido no SAP. Tente novamente mais tarde.",
                Status = StatusCodes.Status500InternalServerError,
                Instance = HttpContext.Request.Path
            });
        }
    }

    /// <summary>
    /// Cria um novo pedido de venda no SAP S/4HANA
    /// </summary>
    /// <param name="command">Dados do pedido de venda a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Pedido de venda criado com número do documento SAP</returns>
    /// <response code="201">Pedido criado com sucesso no SAP</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="422">Erro de validação de negócio</response>
    /// <response code="429">Muitas requisições (rate limit)</response>
    /// <response code="500">Erro interno no servidor ou SAP</response>
    [HttpPost]
    [ProducesResponseType(typeof(PedidoVendaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PedidoVendaResponseDto>> CriarPedido(
        [FromBody] CriarPedidoVendaCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Requisição POST recebida para criar pedido | Cliente: {ClienteId} | IP: {IpAddress}",
            command.ClienteId,
            HttpContext.Connection.RemoteIpAddress);

        try
        {
            // OWASP: Mass Assignment Protection - O command já limita os campos aceitos
            var resultado = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation(
                "Pedido criado com sucesso. Número: {NumeroDocumento} | Cliente: {ClienteId} | Total: {ValorTotal}",
                resultado.NumeroDocumento,
                resultado.ClienteId,
                resultado.ValorTotal);

            // Retorna 201 Created com header Location
            return CreatedAtAction(
                nameof(ObterPedidoPorNumero),
                new { numeroDocumento = resultado.NumeroDocumento },
                resultado);
        }
        catch (ValidationException validationEx)
        {
            // OWASP: Input Validation - Tratamento de erros de validação
            _logger.LogWarning(
                validationEx,
                "Erro de validação ao criar pedido para cliente {ClienteId}",
                command.ClienteId);

            var errors = validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return UnprocessableEntity(new ValidationProblemDetails(errors)
            {
                Title = "Erro de validação",
                Detail = "Um ou mais campos contêm valores inválidos",
                Status = StatusCodes.Status422UnprocessableEntity,
                Instance = HttpContext.Request.Path
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao processar criação de pedido para cliente {ClienteId}",
                command.ClienteId);

            // OWASP: Security Misconfiguration - Não expor detalhes internos
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Erro ao criar pedido",
                Detail = "Ocorreu um erro ao criar o pedido no SAP. Tente novamente mais tarde.",
                Status = StatusCodes.Status500InternalServerError,
                Instance = HttpContext.Request.Path
            });
        }
    }
}
