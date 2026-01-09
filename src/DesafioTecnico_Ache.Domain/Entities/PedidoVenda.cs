using DesafioTecnico_Ache.Domain.Common;
using DesafioTecnico_Ache.Domain.Enums;
using DesafioTecnico_Ache.Domain.ValueObjects;

namespace DesafioTecnico_Ache.Domain.Entities;

/// <summary>
/// Aggregate Root representando Pedido de Venda (SAP SD - Sales Order)
/// Representa um documento de venda completo no módulo SD do SAP S/4HANA
/// </summary>
public class PedidoVenda : Entity
{
    private readonly List<ItemPedido> _itens = new();

    public string NumeroDocumento { get; private set; }
    public string ClienteId { get; private set; }
    public string NomeCliente { get; private set; }
    public DateTime DataPedido { get; private set; }
    public DateTime? DataEntregaSolicitada { get; private set; }
    public StatusPedido Status { get; private set; }
    public Endereco EnderecoEntrega { get; private set; }
    public Endereco? EnderecoCobranca { get; private set; }
    public string OrganizacaoVendas { get; private set; }
    public string CanalDistribuicao { get; private set; }
    public string Setor { get; private set; }
    public Dinheiro ValorTotal { get; private set; }
    public string? Observacoes { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataUltimaAtualizacao { get; private set; }

    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

    private PedidoVenda() : base()
    {
        NumeroDocumento = string.Empty;
        ClienteId = string.Empty;
        NomeCliente = string.Empty;
        EnderecoEntrega = null!;
        OrganizacaoVendas = string.Empty;
        CanalDistribuicao = string.Empty;
        Setor = string.Empty;
        ValorTotal = new Dinheiro(0);
    }

    public PedidoVenda(
        string id,
        string numeroDocumento,
        string clienteId,
        string nomeCliente,
        Endereco enderecoEntrega,
        string organizacaoVendas,
        string canalDistribuicao,
        string setor,
        DateTime? dataEntregaSolicitada = null,
        Endereco? enderecoCobranca = null,
        string? observacoes = null) 
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(clienteId))
            throw new ArgumentException("Cliente ID não pode ser vazio", nameof(clienteId));

        if (string.IsNullOrWhiteSpace(numeroDocumento))
            throw new ArgumentException("Número do documento não pode ser vazio", nameof(numeroDocumento));

        NumeroDocumento = numeroDocumento;
        ClienteId = clienteId;
        NomeCliente = nomeCliente;
        DataPedido = DateTime.UtcNow;
        DataEntregaSolicitada = dataEntregaSolicitada;
        Status = StatusPedido.Rascunho;
        EnderecoEntrega = enderecoEntrega;
        EnderecoCobranca = enderecoCobranca ?? enderecoEntrega;
        OrganizacaoVendas = organizacaoVendas;
        CanalDistribuicao = canalDistribuicao;
        Setor = setor;
        ValorTotal = new Dinheiro(0);
        Observacoes = observacoes;
        DataCriacao = DateTime.UtcNow;
    }

    public void AdicionarItem(ItemPedido item)
    {
        if (Status != StatusPedido.Rascunho && Status != StatusPedido.Pendente)
            throw new InvalidOperationException($"Não é possível adicionar itens a um pedido com status {Status}");

        _itens.Add(item);
        RecalcularValorTotal();
        DataUltimaAtualizacao = DateTime.UtcNow;
    }

    public void RemoverItem(string itemId)
    {
        if (Status != StatusPedido.Rascunho && Status != StatusPedido.Pendente)
            throw new InvalidOperationException($"Não é possível remover itens de um pedido com status {Status}");

        var item = _itens.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _itens.Remove(item);
            RecalcularValorTotal();
            DataUltimaAtualizacao = DateTime.UtcNow;
        }
    }

    public void AlterarStatus(StatusPedido novoStatus)
    {
        // Validação de transições de status (regra de negócio)
        if (Status == StatusPedido.Cancelado)
            throw new InvalidOperationException("Não é possível alterar o status de um pedido cancelado");

        if (novoStatus == StatusPedido.Confirmado && !_itens.Any())
            throw new InvalidOperationException("Não é possível confirmar um pedido sem itens");

        Status = novoStatus;
        DataUltimaAtualizacao = DateTime.UtcNow;
    }

    public void Cancelar(string motivo)
    {
        if (Status == StatusPedido.Entregue)
            throw new InvalidOperationException("Não é possível cancelar um pedido já entregue");

        Status = StatusPedido.Cancelado;
        Observacoes = $"{Observacoes}\nMotivo do cancelamento: {motivo}";
        DataUltimaAtualizacao = DateTime.UtcNow;
    }

    private void RecalcularValorTotal()
    {
        if (_itens.Any())
        {
            var valor = _itens
                .Select(i => i.ValorTotal.Valor)
                .Sum();
            
            ValorTotal = new Dinheiro(valor, _itens.First().ValorTotal.Moeda);
        }
        else
        {
            ValorTotal = new Dinheiro(0);
        }
    }
}
