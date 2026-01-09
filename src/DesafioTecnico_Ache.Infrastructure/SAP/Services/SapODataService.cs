using Microsoft.Extensions.Logging;
using DesafioTecnico_Ache.Domain.Interfaces;
using DesafioTecnico_Ache.Infrastructure.SAP.Models;
using System.Text.Json;

namespace DesafioTecnico_Ache.Infrastructure.SAP.Services;

/// <summary>
/// Implementação mockada do serviço de integração SAP OData
/// Simula chamadas HTTP para API_SALES_ORDER_SRV do SAP S/4HANA
/// 
/// TIPO DE INTEGRAÇÃO: OData/REST
/// Endpoint base simulado: https://sap-system.com:443/sap/opu/odata/sap/API_SALES_ORDER_SRV
/// 
/// Autenticação: Basic Auth (simulada)
/// Protocolo: HTTPS
/// Formato: JSON (OData v2)
/// </summary>
public class SapODataService : ISapODataService
{
    private readonly ILogger<SapODataService> _logger;
    private readonly Dictionary<string, SapSalesOrderResponse> _mockDatabase;

    public SapODataService(ILogger<SapODataService> logger)
    {
        _logger = logger;
        _mockDatabase = new Dictionary<string, SapSalesOrderResponse>();
        InicializarDadosMock();
    }

    /// <summary>
    /// Simula GET: /sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('{numeroDocumento}')
    /// Headers: Authorization: Basic {credentials}, Accept: application/json
    /// </summary>
    public async Task<T?> ConsultarPedidoVendaAsync<T>(string numeroDocumento, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[SAP OData] GET /sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('{NumeroDocumento}')",
            numeroDocumento);

        // Simula latência de rede
        await Task.Delay(100, cancellationToken);

        if (_mockDatabase.TryGetValue(numeroDocumento, out var pedido))
        {
            _logger.LogInformation(
                "[SAP OData] Pedido {NumeroDocumento} encontrado no SAP. Status: {Status}",
                numeroDocumento,
                pedido.OverallSDProcessStatus);

            var json = JsonSerializer.Serialize(pedido);
            return JsonSerializer.Deserialize<T>(json);
        }

        _logger.LogWarning("[SAP OData] Pedido {NumeroDocumento} não encontrado no SAP", numeroDocumento);
        return default;
    }

    /// <summary>
    /// Simula POST: /sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder
    /// Headers: Authorization: Basic {credentials}, Content-Type: application/json
    /// Body: JSON com dados do pedido
    /// </summary>
    public async Task<T> CriarPedidoVendaAsync<T>(object pedidoRequest, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[SAP OData] POST /sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder");

        // Simula latência de rede e processamento do SAP
        await Task.Delay(200, cancellationToken);

        var json = JsonSerializer.Serialize(pedidoRequest);
        var request = JsonSerializer.Deserialize<SapCreateSalesOrderRequest>(json);

        if (request == null)
            throw new InvalidOperationException("Requisição inválida para criação de pedido no SAP");

        // Gerar número do documento SAP (simulado)
        var numeroDocumento = GerarNumeroDocumentoSap();

        var sapResponse = new SapSalesOrderResponse
        {
            SalesOrder = numeroDocumento,
            SoldToParty = request.SoldToParty,
            SoldToPartyName = $"Cliente {request.SoldToParty}",
            SalesOrderDate = DateTime.UtcNow,
            RequestedDeliveryDate = request.RequestedDeliveryDate,
            SalesOrderType = request.SalesOrderType,
            SalesOrganization = request.SalesOrganization,
            DistributionChannel = request.DistributionChannel,
            OrganizationDivision = request.OrganizationDivision,
            OverallSDProcessStatus = "A", // A = Aberto
            CustomerPurchaseOrderNbr = request.CustomerPurchaseOrderNbr,
            Items = request.Items.Select((item, index) => new SapSalesOrderItemResponse
            {
                SalesOrder = numeroDocumento,
                SalesOrderItem = ((index + 1) * 10).ToString("D6"),
                Material = item.Material,
                SalesOrderItemText = $"Material {item.Material}",
                OrderQuantity = item.RequestedQuantity,
                OrderQuantityUnit = item.RequestedQuantityUnit,
                Plant = item.Plant,
                StorageLocation = item.StorageLocation
            }).ToList()
        };

        // Calcular valor total (simplificado - normalmente vem do pricing do SAP)
        sapResponse.TotalNetAmount = sapResponse.Items.Sum(i => i.NetAmount);

        // Armazenar no mock database
        _mockDatabase[numeroDocumento] = sapResponse;

        _logger.LogInformation(
            "[SAP OData] Pedido {NumeroDocumento} criado com sucesso no SAP. Total de itens: {TotalItens}",
            numeroDocumento,
            sapResponse.Items.Count);

        var responseJson = JsonSerializer.Serialize(sapResponse);
        return JsonSerializer.Deserialize<T>(responseJson)!;
    }

    /// <summary>
    /// Simula PATCH: /sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('{numeroDocumento}')
    /// Headers: Authorization: Basic {credentials}, Content-Type: application/json
    /// </summary>
    public async Task<T> AtualizarPedidoVendaAsync<T>(string numeroDocumento, object pedidoRequest, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[SAP OData] PATCH /sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder('{NumeroDocumento}')",
            numeroDocumento);

        // Simula latência de rede
        await Task.Delay(150, cancellationToken);

        if (!_mockDatabase.ContainsKey(numeroDocumento))
        {
            _logger.LogError("[SAP OData] Erro ao atualizar: Pedido {NumeroDocumento} não encontrado", numeroDocumento);
            throw new KeyNotFoundException($"Pedido {numeroDocumento} não encontrado no SAP");
        }

        var pedidoAtual = _mockDatabase[numeroDocumento];
        
        _logger.LogInformation(
            "[SAP OData] Pedido {NumeroDocumento} atualizado com sucesso no SAP",
            numeroDocumento);

        var json = JsonSerializer.Serialize(pedidoAtual);
        return JsonSerializer.Deserialize<T>(json)!;
    }

    /// <summary>
    /// Simula GET: /sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder?$filter={filtro}
    /// Exemplo: $filter=SoldToParty eq '0001000000'
    /// </summary>
    public async Task<IEnumerable<T>> ListarPedidosVendaAsync<T>(string? filtro = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[SAP OData] GET /sap/opu/odata/sap/API_SALES_ORDER_SRV/A_SalesOrder?$filter={Filtro}",
            filtro ?? "sem filtro");

        // Simula latência de rede
        await Task.Delay(150, cancellationToken);

        var pedidos = _mockDatabase.Values.ToList();

        _logger.LogInformation(
            "[SAP OData] Retornados {Total} pedidos do SAP",
            pedidos.Count);

        var json = JsonSerializer.Serialize(pedidos);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    private static string GerarNumeroDocumentoSap()
    {
        // Simula a numeração interna do SAP (8 dígitos)
        return new Random().Next(10000000, 99999999).ToString();
    }

    private void InicializarDadosMock()
    {
        // Adiciona alguns pedidos de exemplo para testes
        var pedidoExemplo = new SapSalesOrderResponse
        {
            SalesOrder = "12345678",
            SoldToParty = "0001000000",
            SoldToPartyName = "Farmácia Central Ltda",
            SalesOrderDate = DateTime.UtcNow.AddDays(-5),
            RequestedDeliveryDate = DateTime.UtcNow.AddDays(2),
            SalesOrderType = "OR",
            SalesOrganization = "1000",
            DistributionChannel = "10",
            OrganizationDivision = "00",
            TotalNetAmount = 1500.00m,
            TransactionCurrency = "BRL",
            OverallSDProcessStatus = "B", // B = Em processamento
            CustomerPurchaseOrderNbr = "PO-2024-001",
            Items = new List<SapSalesOrderItemResponse>
            {
                new()
                {
                    SalesOrder = "12345678",
                    SalesOrderItem = "000010",
                    Material = "MED001",
                    SalesOrderItemText = "Medicamento Ache A - 500mg",
                    OrderQuantity = 10,
                    OrderQuantityUnit = "CX",
                    NetAmount = 500.00m,
                    TransactionCurrency = "BRL",
                    Plant = "1000",
                    StorageLocation = "0001"
                },
                new()
                {
                    SalesOrder = "12345678",
                    SalesOrderItem = "000020",
                    Material = "MED002",
                    SalesOrderItemText = "Medicamento Ache B - 250mg",
                    OrderQuantity = 20,
                    OrderQuantityUnit = "CX",
                    NetAmount = 1000.00m,
                    TransactionCurrency = "BRL",
                    Plant = "1000",
                    StorageLocation = "0001"
                }
            }
        };

        _mockDatabase["12345678"] = pedidoExemplo;

        _logger.LogInformation("[SAP OData] Dados mock inicializados com {Total} pedidos", _mockDatabase.Count);
    }
}
