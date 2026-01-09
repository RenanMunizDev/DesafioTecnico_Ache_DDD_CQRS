namespace DesafioTecnico_Ache.Domain.Enums;

/// <summary>
/// Status do pedido de venda no SAP SD
/// </summary>
public enum StatusPedido
{
    Rascunho = 0,
    Pendente = 1,
    Confirmado = 2,
    EmSeparacao = 3,
    Faturado = 4,
    Enviado = 5,
    Entregue = 6,
    Cancelado = 7
}
