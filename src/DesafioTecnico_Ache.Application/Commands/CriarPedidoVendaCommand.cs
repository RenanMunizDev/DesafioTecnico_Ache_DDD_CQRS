using MediatR;
using DesafioTecnico_Ache.Application.DTOs;

namespace DesafioTecnico_Ache.Application.Commands;

/// <summary>
/// Command para criar um novo pedido de venda no SAP SD
/// Segue padrão CQRS
/// </summary>
public class CriarPedidoVendaCommand : IRequest<PedidoVendaResponseDto>
{
    public string ClienteId { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty;
    public DateTime? DataEntregaSolicitada { get; set; }
    public EnderecoDto EnderecoEntrega { get; set; } = new();
    public EnderecoDto? EnderecoCobranca { get; set; }
    public string OrganizacaoVendas { get; set; } = string.Empty;
    public string CanalDistribuicao { get; set; } = string.Empty;
    public string Setor { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public List<ItemPedidoDto> Itens { get; set; } = new();
}
