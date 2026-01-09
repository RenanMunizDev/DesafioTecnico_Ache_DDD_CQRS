using DesafioTecnico_Ache.Domain.Entities;

namespace DesafioTecnico_Ache.Domain.Interfaces;

/// <summary>
/// Contrato para repositório de Pedidos de Venda
/// Segue o padrão Repository (DDD)
/// </summary>
public interface IPedidoVendaRepository
{
    /// <summary>
    /// Obtém um pedido de venda por ID
    /// </summary>
    Task<PedidoVenda?> ObterPorIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um pedido de venda pelo número do documento SAP
    /// </summary>
    Task<PedidoVenda?> ObterPorNumeroDocumentoAsync(string numeroDocumento, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém pedidos de venda de um cliente específico
    /// </summary>
    Task<IEnumerable<PedidoVenda>> ObterPorClienteIdAsync(string clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todos os pedidos de venda com paginação
    /// </summary>
    Task<IEnumerable<PedidoVenda>> ListarTodosAsync(int pagina = 1, int tamanhoPagina = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo pedido de venda
    /// </summary>
    Task<PedidoVenda> CriarAsync(PedidoVenda pedido, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um pedido de venda existente
    /// </summary>
    Task<PedidoVenda> AtualizarAsync(PedidoVenda pedido, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um pedido de venda (soft delete)
    /// </summary>
    Task<bool> RemoverAsync(string id, CancellationToken cancellationToken = default);
}
