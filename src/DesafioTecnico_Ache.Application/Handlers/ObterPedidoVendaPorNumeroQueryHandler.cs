using MediatR;
using Microsoft.Extensions.Logging;
using DesafioTecnico_Ache.Application.DTOs;
using DesafioTecnico_Ache.Application.Queries;
using DesafioTecnico_Ache.Domain.Interfaces;
using DesafioTecnico_Ache.Domain.Entities;

namespace DesafioTecnico_Ache.Application.Handlers;

/// <summary>
/// Handler para query de obtenção de pedido de venda por número
/// Implementa padrão CQRS com MediatR
/// </summary>
public class ObterPedidoVendaPorNumeroQueryHandler : IRequestHandler<ObterPedidoVendaPorNumeroQuery, PedidoVendaResponseDto?>
{
    private readonly IPedidoVendaRepository _repository;
    private readonly ILogger<ObterPedidoVendaPorNumeroQueryHandler> _logger;

    public ObterPedidoVendaPorNumeroQueryHandler(
        IPedidoVendaRepository repository,
        ILogger<ObterPedidoVendaPorNumeroQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PedidoVendaResponseDto?> Handle(ObterPedidoVendaPorNumeroQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Consultando pedido de venda com número {NumeroDocumento}", request.NumeroDocumento);

        try
        {
            var pedido = await _repository.ObterPorNumeroDocumentoAsync(request.NumeroDocumento, cancellationToken);

            if (pedido == null)
            {
                _logger.LogWarning("Pedido de venda {NumeroDocumento} não encontrado", request.NumeroDocumento);
                return null;
            }

            _logger.LogInformation("Pedido de venda {NumeroDocumento} encontrado", request.NumeroDocumento);

            return MapearParaDto(pedido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar pedido de venda {NumeroDocumento}", request.NumeroDocumento);
            throw;
        }
    }

    private static PedidoVendaResponseDto MapearParaDto(PedidoVenda pedido)
    {
        return new PedidoVendaResponseDto
        {
            Id = pedido.Id,
            NumeroDocumento = pedido.NumeroDocumento,
            ClienteId = pedido.ClienteId,
            NomeCliente = pedido.NomeCliente,
            DataPedido = pedido.DataPedido,
            DataEntregaSolicitada = pedido.DataEntregaSolicitada,
            Status = pedido.Status.ToString(),
            EnderecoEntrega = new EnderecoDto
            {
                Logradouro = pedido.EnderecoEntrega.Logradouro,
                Numero = pedido.EnderecoEntrega.Numero,
                Complemento = pedido.EnderecoEntrega.Complemento,
                Bairro = pedido.EnderecoEntrega.Bairro,
                Cidade = pedido.EnderecoEntrega.Cidade,
                Estado = pedido.EnderecoEntrega.Estado,
                Cep = pedido.EnderecoEntrega.Cep,
                Pais = pedido.EnderecoEntrega.Pais
            },
            EnderecoCobranca = pedido.EnderecoCobranca != null ? new EnderecoDto
            {
                Logradouro = pedido.EnderecoCobranca.Logradouro,
                Numero = pedido.EnderecoCobranca.Numero,
                Complemento = pedido.EnderecoCobranca.Complemento,
                Bairro = pedido.EnderecoCobranca.Bairro,
                Cidade = pedido.EnderecoCobranca.Cidade,
                Estado = pedido.EnderecoCobranca.Estado,
                Cep = pedido.EnderecoCobranca.Cep,
                Pais = pedido.EnderecoCobranca.Pais
            } : null,
            OrganizacaoVendas = pedido.OrganizacaoVendas,
            CanalDistribuicao = pedido.CanalDistribuicao,
            Setor = pedido.Setor,
            ValorTotal = pedido.ValorTotal.Valor,
            Moeda = pedido.ValorTotal.Moeda,
            Observacoes = pedido.Observacoes,
            Itens = pedido.Itens.Select(i => new ItemPedidoResponseDto
            {
                Id = i.Id,
                NumeroItem = i.NumeroItem,
                MaterialId = i.MaterialId,
                DescricaoMaterial = i.DescricaoMaterial,
                Quantidade = i.Quantidade,
                UnidadeMedida = i.UnidadeMedida,
                PrecoUnitario = i.PrecoUnitario.Valor,
                ValorTotal = i.ValorTotal.Valor,
                Centro = i.Centro,
                Deposito = i.Deposito
            }).ToList(),
            DataCriacao = pedido.DataCriacao,
            DataUltimaAtualizacao = pedido.DataUltimaAtualizacao
        };
    }
}
