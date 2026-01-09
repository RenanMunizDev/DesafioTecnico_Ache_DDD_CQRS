using MediatR;
using Microsoft.Extensions.Logging;
using DesafioTecnico_Ache.Application.Commands;
using DesafioTecnico_Ache.Application.DTOs;
using DesafioTecnico_Ache.Domain.Entities;
using DesafioTecnico_Ache.Domain.Interfaces;
using DesafioTecnico_Ache.Domain.ValueObjects;

namespace DesafioTecnico_Ache.Application.Handlers;

/// <summary>
/// Handler para comando de criação de pedido de venda
/// Implementa padrão CQRS com MediatR
/// </summary>
public class CriarPedidoVendaCommandHandler : IRequestHandler<CriarPedidoVendaCommand, PedidoVendaResponseDto>
{
    private readonly IPedidoVendaRepository _repository;
    private readonly ILogger<CriarPedidoVendaCommandHandler> _logger;

    public CriarPedidoVendaCommandHandler(
        IPedidoVendaRepository repository,
        ILogger<CriarPedidoVendaCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PedidoVendaResponseDto> Handle(CriarPedidoVendaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando criação de pedido de venda para cliente {ClienteId}", request.ClienteId);

        try
        {
            // Gerar número do documento (simulação - normalmente vem do SAP)
            var numeroDocumento = GerarNumeroDocumento();
            var pedidoId = Guid.NewGuid().ToString();

            // Mapear DTOs para Value Objects
            var enderecoEntrega = MapearEndereco(request.EnderecoEntrega);
            var enderecoCobranca = request.EnderecoCobranca != null 
                ? MapearEndereco(request.EnderecoCobranca) 
                : null;

            // Criar entidade de domínio
            var pedido = new PedidoVenda(
                pedidoId,
                numeroDocumento,
                request.ClienteId,
                request.NomeCliente,
                enderecoEntrega,
                request.OrganizacaoVendas,
                request.CanalDistribuicao,
                request.Setor,
                request.DataEntregaSolicitada,
                enderecoCobranca,
                request.Observacoes
            );

            // Adicionar itens ao pedido
            int numeroItem = 1;
            foreach (var itemDto in request.Itens)
            {
                var item = new ItemPedido(
                    Guid.NewGuid().ToString(),
                    pedidoId,
                    numeroItem++,
                    itemDto.MaterialId,
                    itemDto.DescricaoMaterial,
                    itemDto.Quantidade,
                    itemDto.UnidadeMedida,
                    new Dinheiro(itemDto.PrecoUnitario),
                    itemDto.Centro,
                    itemDto.Deposito
                );
                pedido.AdicionarItem(item);
            }

            // Persistir no repositório (que simula integração com SAP)
            var pedidoCriado = await _repository.CriarAsync(pedido, cancellationToken);

            _logger.LogInformation(
                "Pedido de venda {NumeroDocumento} criado com sucesso para cliente {ClienteId}. Total: {ValorTotal}", 
                numeroDocumento, 
                request.ClienteId,
                pedidoCriado.ValorTotal.Valor);

            // Mapear entidade de domínio para DTO de resposta
            return MapearParaDto(pedidoCriado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido de venda para cliente {ClienteId}", request.ClienteId);
            throw;
        }
    }

    private static string GerarNumeroDocumento()
    {
        // Simula a geração de número de documento do SAP
        // Formato típico: 8 dígitos
        return DateTime.UtcNow.ToString("yyyyMMdd") + new Random().Next(1000, 9999);
    }

    private static Endereco MapearEndereco(EnderecoDto dto)
    {
        return new Endereco(
            dto.Logradouro,
            dto.Numero,
            dto.Complemento,
            dto.Bairro,
            dto.Cidade,
            dto.Estado,
            dto.Cep,
            dto.Pais
        );
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
