namespace DesafioTecnico_Ache.Application.DTOs;

/// <summary>
/// DTO de resposta para pedido de venda
/// </summary>
public class PedidoVendaResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty;
    public DateTime DataPedido { get; set; }
    public DateTime? DataEntregaSolicitada { get; set; }
    public string Status { get; set; } = string.Empty;
    public EnderecoDto EnderecoEntrega { get; set; } = new();
    public EnderecoDto? EnderecoCobranca { get; set; }
    public string OrganizacaoVendas { get; set; } = string.Empty;
    public string CanalDistribuicao { get; set; } = string.Empty;
    public string Setor { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string Moeda { get; set; } = "BRL";
    public string? Observacoes { get; set; }
    public List<ItemPedidoResponseDto> Itens { get; set; } = new();
    public DateTime DataCriacao { get; set; }
    public DateTime? DataUltimaAtualizacao { get; set; }
}

/// <summary>
/// DTO de resposta para item do pedido
/// </summary>
public class ItemPedidoResponseDto
{
    public string Id { get; set; } = string.Empty;
    public int NumeroItem { get; set; }
    public string MaterialId { get; set; } = string.Empty;
    public string DescricaoMaterial { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public string UnidadeMedida { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public string? Centro { get; set; }
    public string? Deposito { get; set; }
}
