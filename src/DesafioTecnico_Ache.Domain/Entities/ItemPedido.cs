using DesafioTecnico_Ache.Domain.Common;
using DesafioTecnico_Ache.Domain.ValueObjects;

namespace DesafioTecnico_Ache.Domain.Entities;

/// <summary>
/// Entidade representando item do pedido de venda (SAP SD - Sales Order Item)
/// </summary>
public class ItemPedido : Entity
{
    public string PedidoId { get; private set; }
    public int NumeroItem { get; private set; }
    public string MaterialId { get; private set; }
    public string DescricaoMaterial { get; private set; }
    public decimal Quantidade { get; private set; }
    public string UnidadeMedida { get; private set; }
    public Dinheiro PrecoUnitario { get; private set; }
    public Dinheiro ValorTotal { get; private set; }
    public string? Centro { get; private set; }
    public string? Deposito { get; private set; }

    private ItemPedido() : base()
    {
        PedidoId = string.Empty;
        MaterialId = string.Empty;
        DescricaoMaterial = string.Empty;
        UnidadeMedida = string.Empty;
        PrecoUnitario = new Dinheiro(0);
        ValorTotal = new Dinheiro(0);
    }

    public ItemPedido(string id, string pedidoId, int numeroItem, string materialId, 
                      string descricaoMaterial, decimal quantidade, string unidadeMedida,
                      Dinheiro precoUnitario, string? centro = null, string? deposito = null) 
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(materialId))
            throw new ArgumentException("Material ID não pode ser vazio", nameof(materialId));

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

        PedidoId = pedidoId;
        NumeroItem = numeroItem;
        MaterialId = materialId;
        DescricaoMaterial = descricaoMaterial;
        Quantidade = quantidade;
        UnidadeMedida = unidadeMedida;
        PrecoUnitario = precoUnitario;
        ValorTotal = precoUnitario * quantidade;
        Centro = centro;
        Deposito = deposito;
    }

    public void AtualizarQuantidade(decimal novaQuantidade)
    {
        if (novaQuantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(novaQuantidade));

        Quantidade = novaQuantidade;
        ValorTotal = PrecoUnitario * novaQuantidade;
    }
}
