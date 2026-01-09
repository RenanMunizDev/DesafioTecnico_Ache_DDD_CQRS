namespace DesafioTecnico_Ache.Domain.Interfaces;

/// <summary>
/// Interface para integração com SAP S/4HANA via OData
/// Simula a comunicação com os serviços OData do módulo SD
/// </summary>
public interface ISapODataService
{
    /// <summary>
    /// Consulta pedido de venda no SAP via OData (GET)
    /// Endpoint simulado: /sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('{id}')
    /// </summary>
    Task<T?> ConsultarPedidoVendaAsync<T>(string numeroDocumento, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria pedido de venda no SAP via OData (POST)
    /// Endpoint simulado: /sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder
    /// </summary>
    Task<T> CriarPedidoVendaAsync<T>(object pedidoRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza pedido de venda no SAP via OData (PATCH)
    /// Endpoint simulado: /sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('{id}')
    /// </summary>
    Task<T> AtualizarPedidoVendaAsync<T>(string numeroDocumento, object pedidoRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista pedidos de venda no SAP via OData com filtros (GET)
    /// Endpoint simulado: /sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder?$filter=...
    /// </summary>
    Task<IEnumerable<T>> ListarPedidosVendaAsync<T>(string? filtro = null, CancellationToken cancellationToken = default);
}
