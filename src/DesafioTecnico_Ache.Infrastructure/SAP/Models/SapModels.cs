namespace DesafioTecnico_Ache.Infrastructure.SAP.Models;

/// <summary>
/// Modelo de resposta do SAP OData para consulta de pedido de venda
/// Simula a estrutura de dados retornada pela API_SALES_ORDER_SRV do SAP S/4HANA
/// </summary>
public class SapSalesOrderResponse
{
    public string SalesOrder { get; set; } = string.Empty;
    public string SoldToParty { get; set; } = string.Empty;
    public string SoldToPartyName { get; set; } = string.Empty;
    public DateTime SalesOrderDate { get; set; }
    public DateTime? RequestedDeliveryDate { get; set; }
    public string SalesOrderType { get; set; } = string.Empty;
    public string SalesOrganization { get; set; } = string.Empty;
    public string DistributionChannel { get; set; } = string.Empty;
    public string OrganizationDivision { get; set; } = string.Empty;
    public decimal TotalNetAmount { get; set; }
    public string TransactionCurrency { get; set; } = "BRL";
    public string OverallSDProcessStatus { get; set; } = string.Empty;
    public string CustomerPurchaseOrderNbr { get; set; } = string.Empty;
    public List<SapSalesOrderItemResponse> Items { get; set; } = new();
}

/// <summary>
/// Modelo de item do pedido de venda SAP OData
/// </summary>
public class SapSalesOrderItemResponse
{
    public string SalesOrder { get; set; } = string.Empty;
    public string SalesOrderItem { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public string SalesOrderItemText { get; set; } = string.Empty;
    public decimal OrderQuantity { get; set; }
    public string OrderQuantityUnit { get; set; } = string.Empty;
    public decimal NetAmount { get; set; }
    public string TransactionCurrency { get; set; } = "BRL";
    public string Plant { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
}

/// <summary>
/// Modelo de requisição para criação de pedido de venda no SAP OData
/// </summary>
public class SapCreateSalesOrderRequest
{
    public string SalesOrderType { get; set; } = "OR"; // Tipo padrão: Order
    public string SoldToParty { get; set; } = string.Empty;
    public string SalesOrganization { get; set; } = string.Empty;
    public string DistributionChannel { get; set; } = string.Empty;
    public string OrganizationDivision { get; set; } = string.Empty;
    public DateTime? RequestedDeliveryDate { get; set; }
    public string CustomerPurchaseOrderNbr { get; set; } = string.Empty;
    public List<SapCreateSalesOrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Modelo de item para criação no SAP OData
/// </summary>
public class SapCreateSalesOrderItemRequest
{
    public string Material { get; set; } = string.Empty;
    public decimal RequestedQuantity { get; set; }
    public string RequestedQuantityUnit { get; set; } = string.Empty;
    public string Plant { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
}
