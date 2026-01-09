using Microsoft.Extensions.Logging;
using DesafioTecnico_Ache.Domain.Entities;
using DesafioTecnico_Ache.Domain.Interfaces;
using DesafioTecnico_Ache.Domain.ValueObjects;
using DesafioTecnico_Ache.Infrastructure.SAP.Models;

namespace DesafioTecnico_Ache.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Pedidos de Venda
/// Realiza integração com SAP S/4HANA através do serviço OData
/// Segue padrão Repository (DDD) e Adapter (para conversão SAP <-> Domain)
/// </summary>
public class PedidoVendaRepository : IPedidoVendaRepository
{
    private readonly ISapODataService _sapService;
    private readonly ILogger<PedidoVendaRepository> _logger;

    public PedidoVendaRepository(
        ISapODataService sapService,
        ILogger<PedidoVendaRepository> logger)
    {
        _sapService = sapService;
        _logger = logger;
    }

    public async Task<PedidoVenda?> ObterPorIdAsync(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Obtendo pedido de venda por ID {PedidoId}", id);

        // Em um cenário real, consultaríamos pelo ID interno
        // Para esta simulação, vamos retornar null pois trabalhamos com número do documento
        await Task.CompletedTask;
        return null;
    }

    public async Task<PedidoVenda?> ObterPorNumeroDocumentoAsync(string numeroDocumento, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Consultando pedido {NumeroDocumento} no SAP via OData", numeroDocumento);

        try
        {
            var sapResponse = await _sapService.ConsultarPedidoVendaAsync<SapSalesOrderResponse>(
                numeroDocumento, 
                cancellationToken);

            if (sapResponse == null)
            {
                _logger.LogWarning("Pedido {NumeroDocumento} não encontrado no SAP", numeroDocumento);
                return null;
            }

            // Converter modelo SAP para entidade de domínio (Adapter Pattern)
            var pedido = ConverterSapParaDomain(sapResponse);

            _logger.LogInformation(
                "Pedido {NumeroDocumento} convertido com sucesso. Total: {ValorTotal}",
                numeroDocumento,
                pedido.ValorTotal.Valor);

            return pedido;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido {NumeroDocumento} do SAP", numeroDocumento);
            throw;
        }
    }

    public async Task<IEnumerable<PedidoVenda>> ObterPorClienteIdAsync(string clienteId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Consultando pedidos do cliente {ClienteId} no SAP via OData", clienteId);

        try
        {
            var filtro = $"SoldToParty eq '{clienteId}'";
            var sapResponses = await _sapService.ListarPedidosVendaAsync<SapSalesOrderResponse>(
                filtro, 
                cancellationToken);

            var pedidos = sapResponses
                .Select(ConverterSapParaDomain)
                .ToList();

            _logger.LogInformation(
                "Encontrados {Total} pedidos para o cliente {ClienteId}",
                pedidos.Count,
                clienteId);

            return pedidos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedidos do cliente {ClienteId} do SAP", clienteId);
            throw;
        }
    }

    public async Task<IEnumerable<PedidoVenda>> ListarTodosAsync(int pagina = 1, int tamanhoPagina = 20, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listando pedidos do SAP - Página {Pagina}, Tamanho {Tamanho}", pagina, tamanhoPagina);

        try
        {
            // Em OData real, usaríamos $top e $skip para paginação
            var sapResponses = await _sapService.ListarPedidosVendaAsync<SapSalesOrderResponse>(
                null, 
                cancellationToken);

            var pedidos = sapResponses
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .Select(ConverterSapParaDomain)
                .ToList();

            _logger.LogInformation("Retornados {Total} pedidos", pedidos.Count);

            return pedidos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar pedidos do SAP");
            throw;
        }
    }

    public async Task<PedidoVenda> CriarAsync(PedidoVenda pedido, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Criando pedido de venda no SAP para cliente {ClienteId}",
            pedido.ClienteId);

        try
        {
            // Converter entidade de domínio para modelo SAP (Adapter Pattern)
            var sapRequest = ConverterDomainParaSap(pedido);

            // Enviar para SAP via OData
            var sapResponse = await _sapService.CriarPedidoVendaAsync<SapSalesOrderResponse>(
                sapRequest, 
                cancellationToken);

            // Atualizar número do documento gerado pelo SAP
            var pedidoAtualizado = ConverterSapParaDomain(sapResponse);

            _logger.LogInformation(
                "Pedido criado com sucesso no SAP. Número do documento: {NumeroDocumento}",
                pedidoAtualizado.NumeroDocumento);

            return pedidoAtualizado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido no SAP para cliente {ClienteId}", pedido.ClienteId);
            throw;
        }
    }

    public async Task<PedidoVenda> AtualizarAsync(PedidoVenda pedido, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Atualizando pedido {NumeroDocumento} no SAP", pedido.NumeroDocumento);

        try
        {
            var sapRequest = ConverterDomainParaSap(pedido);

            var sapResponse = await _sapService.AtualizarPedidoVendaAsync<SapSalesOrderResponse>(
                pedido.NumeroDocumento,
                sapRequest,
                cancellationToken);

            var pedidoAtualizado = ConverterSapParaDomain(sapResponse);

            _logger.LogInformation("Pedido {NumeroDocumento} atualizado com sucesso no SAP", pedido.NumeroDocumento);

            return pedidoAtualizado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar pedido {NumeroDocumento} no SAP", pedido.NumeroDocumento);
            throw;
        }
    }

    public Task<bool> RemoverAsync(string id, CancellationToken cancellationToken = default)
    {
        // SAP normalmente não permite exclusão de pedidos, apenas cancelamento
        _logger.LogWarning("Operação de remoção não suportada. Use cancelamento de pedido.");
        throw new NotSupportedException("Remoção de pedidos não é suportada. Use a operação de cancelamento.");
    }

    #region Métodos de Conversão (Adapter Pattern)

    /// <summary>
    /// Converte modelo SAP OData para entidade de domínio
    /// </summary>
    private static PedidoVenda ConverterSapParaDomain(SapSalesOrderResponse sapResponse)
    {
        // Para simplificação, estamos usando endereços mockados
        // Em cenário real, buscaríamos do master data de clientes
        var endereco = new Endereco(
            "Rua Principal",
            "100",
            null,
            "Centro",
            "São Paulo",
            "SP",
            "01000-000",
            "Brasil"
        );

        var pedido = new PedidoVenda(
            Guid.NewGuid().ToString(),
            sapResponse.SalesOrder,
            sapResponse.SoldToParty,
            sapResponse.SoldToPartyName,
            endereco,
            sapResponse.SalesOrganization,
            sapResponse.DistributionChannel,
            sapResponse.OrganizationDivision,
            sapResponse.RequestedDeliveryDate,
            null,
            sapResponse.CustomerPurchaseOrderNbr
        );

        // Adicionar itens
        int numeroItem = 1;
        foreach (var sapItem in sapResponse.Items)
        {
            var item = new ItemPedido(
                Guid.NewGuid().ToString(),
                pedido.Id,
                numeroItem++,
                sapItem.Material,
                sapItem.SalesOrderItemText,
                sapItem.OrderQuantity,
                sapItem.OrderQuantityUnit,
                new Dinheiro(sapItem.NetAmount / sapItem.OrderQuantity, sapItem.TransactionCurrency),
                sapItem.Plant,
                sapItem.StorageLocation
            );
            pedido.AdicionarItem(item);
        }

        return pedido;
    }

    /// <summary>
    /// Converte entidade de domínio para modelo SAP OData
    /// </summary>
    private static SapCreateSalesOrderRequest ConverterDomainParaSap(PedidoVenda pedido)
    {
        return new SapCreateSalesOrderRequest
        {
            SalesOrderType = "OR",
            SoldToParty = pedido.ClienteId,
            SalesOrganization = pedido.OrganizacaoVendas,
            DistributionChannel = pedido.CanalDistribuicao,
            OrganizationDivision = pedido.Setor,
            RequestedDeliveryDate = pedido.DataEntregaSolicitada,
            CustomerPurchaseOrderNbr = pedido.Observacoes ?? string.Empty,
            Items = pedido.Itens.Select(item => new SapCreateSalesOrderItemRequest
            {
                Material = item.MaterialId,
                RequestedQuantity = item.Quantidade,
                RequestedQuantityUnit = item.UnidadeMedida,
                Plant = item.Centro ?? "1000",
                StorageLocation = item.Deposito ?? "0001"
            }).ToList()
        };
    }

    #endregion
}
