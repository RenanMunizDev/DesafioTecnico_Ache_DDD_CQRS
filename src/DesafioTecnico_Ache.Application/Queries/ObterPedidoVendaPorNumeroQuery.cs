using MediatR;
using DesafioTecnico_Ache.Application.DTOs;

namespace DesafioTecnico_Ache.Application.Queries;

/// <summary>
/// Query para obter um pedido de venda por número do documento SAP
/// Segue padrão CQRS
/// </summary>
public class ObterPedidoVendaPorNumeroQuery : IRequest<PedidoVendaResponseDto?>
{
    public string NumeroDocumento { get; set; } = string.Empty;

    public ObterPedidoVendaPorNumeroQuery(string numeroDocumento)
    {
        NumeroDocumento = numeroDocumento;
    }
}
