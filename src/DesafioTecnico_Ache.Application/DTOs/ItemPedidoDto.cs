namespace DesafioTecnico_Ache.Application.DTOs;

/// <summary>
/// DTO para item do pedido de venda
/// </summary>
public class ItemPedidoDto
{
    public string MaterialId { get; set; } = string.Empty;
    public string DescricaoMaterial { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public string UnidadeMedida { get; set; } = "UN";
    public decimal PrecoUnitario { get; set; }
    public string? Centro { get; set; }
    public string? Deposito { get; set; }
}
