using FluentValidation;
using DesafioTecnico_Ache.Application.Commands;

namespace DesafioTecnico_Ache.Application.Validators;

/// <summary>
/// Validador para comando de criação de pedido de venda
/// Implementa validações de negócio seguindo OWASP (Input Validation)
/// </summary>
public class CriarPedidoVendaCommandValidator : AbstractValidator<CriarPedidoVendaCommand>
{
    public CriarPedidoVendaCommandValidator()
    {
        // Validação de Cliente
        RuleFor(x => x.ClienteId)
            .NotEmpty()
            .WithMessage("Cliente ID é obrigatório")
            .MaximumLength(10)
            .WithMessage("Cliente ID não pode ter mais de 10 caracteres")
            .Matches("^[a-zA-Z0-9]+$")
            .WithMessage("Cliente ID deve conter apenas caracteres alfanuméricos");

        RuleFor(x => x.NomeCliente)
            .NotEmpty()
            .WithMessage("Nome do cliente é obrigatório")
            .MaximumLength(100)
            .WithMessage("Nome do cliente não pode ter mais de 100 caracteres");

        // Validação de Data de Entrega
        RuleFor(x => x.DataEntregaSolicitada)
            .Must(data => !data.HasValue || data.Value > DateTime.UtcNow)
            .WithMessage("Data de entrega deve ser futura");

        // Validação de Endereço de Entrega
        RuleFor(x => x.EnderecoEntrega)
            .NotNull()
            .WithMessage("Endereço de entrega é obrigatório")
            .SetValidator(new EnderecoValidator());

        // Validação de Endereço de Cobrança (se fornecido)
        RuleFor(x => x.EnderecoCobranca)
            .SetValidator(new EnderecoValidator()!)
            .When(x => x.EnderecoCobranca != null);

        // Validação de Organização de Vendas
        RuleFor(x => x.OrganizacaoVendas)
            .NotEmpty()
            .WithMessage("Organização de vendas é obrigatória")
            .MaximumLength(4)
            .WithMessage("Organização de vendas não pode ter mais de 4 caracteres");

        // Validação de Canal de Distribuição
        RuleFor(x => x.CanalDistribuicao)
            .NotEmpty()
            .WithMessage("Canal de distribuição é obrigatório")
            .MaximumLength(2)
            .WithMessage("Canal de distribuição não pode ter mais de 2 caracteres");

        // Validação de Setor
        RuleFor(x => x.Setor)
            .NotEmpty()
            .WithMessage("Setor é obrigatório")
            .MaximumLength(2)
            .WithMessage("Setor não pode ter mais de 2 caracteres");

        // Validação de Observações
        RuleFor(x => x.Observacoes)
            .MaximumLength(500)
            .WithMessage("Observações não podem ter mais de 500 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Observacoes));

        // Validação de Itens
        RuleFor(x => x.Itens)
            .NotEmpty()
            .WithMessage("Pedido deve conter ao menos um item")
            .Must(itens => itens.Count <= 100)
            .WithMessage("Pedido não pode ter mais de 100 itens");

        RuleForEach(x => x.Itens)
            .SetValidator(new ItemPedidoValidator());
    }
}

/// <summary>
/// Validador para endereço
/// </summary>
public class EnderecoValidator : AbstractValidator<DTOs.EnderecoDto>
{
    public EnderecoValidator()
    {
        RuleFor(x => x.Logradouro)
            .NotEmpty()
            .WithMessage("Logradouro é obrigatório")
            .MaximumLength(100)
            .WithMessage("Logradouro não pode ter mais de 100 caracteres");

        RuleFor(x => x.Numero)
            .NotEmpty()
            .WithMessage("Número é obrigatório")
            .MaximumLength(10)
            .WithMessage("Número não pode ter mais de 10 caracteres");

        RuleFor(x => x.Complemento)
            .MaximumLength(50)
            .WithMessage("Complemento não pode ter mais de 50 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Complemento));

        RuleFor(x => x.Bairro)
            .NotEmpty()
            .WithMessage("Bairro é obrigatório")
            .MaximumLength(50)
            .WithMessage("Bairro não pode ter mais de 50 caracteres");

        RuleFor(x => x.Cidade)
            .NotEmpty()
            .WithMessage("Cidade é obrigatória")
            .MaximumLength(50)
            .WithMessage("Cidade não pode ter mais de 50 caracteres");

        RuleFor(x => x.Estado)
            .NotEmpty()
            .WithMessage("Estado é obrigatório")
            .Length(2)
            .WithMessage("Estado deve ter 2 caracteres")
            .Matches("^[A-Z]{2}$")
            .WithMessage("Estado deve ser uma sigla válida (ex: SP, RJ)");

        RuleFor(x => x.Cep)
            .NotEmpty()
            .WithMessage("CEP é obrigatório")
            .Matches(@"^\d{5}-?\d{3}$")
            .WithMessage("CEP deve estar no formato 00000-000 ou 00000000");

        RuleFor(x => x.Pais)
            .NotEmpty()
            .WithMessage("País é obrigatório")
            .MaximumLength(50)
            .WithMessage("País não pode ter mais de 50 caracteres");
    }
}

/// <summary>
/// Validador para item do pedido
/// </summary>
public class ItemPedidoValidator : AbstractValidator<DTOs.ItemPedidoDto>
{
    public ItemPedidoValidator()
    {
        RuleFor(x => x.MaterialId)
            .NotEmpty()
            .WithMessage("Material ID é obrigatório")
            .MaximumLength(18)
            .WithMessage("Material ID não pode ter mais de 18 caracteres")
            .Matches("^[a-zA-Z0-9]+$")
            .WithMessage("Material ID deve conter apenas caracteres alfanuméricos");

        RuleFor(x => x.DescricaoMaterial)
            .NotEmpty()
            .WithMessage("Descrição do material é obrigatória")
            .MaximumLength(200)
            .WithMessage("Descrição do material não pode ter mais de 200 caracteres");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0)
            .WithMessage("Quantidade deve ser maior que zero")
            .LessThanOrEqualTo(9999999)
            .WithMessage("Quantidade não pode ser maior que 9999999");

        RuleFor(x => x.UnidadeMedida)
            .NotEmpty()
            .WithMessage("Unidade de medida é obrigatória")
            .MaximumLength(3)
            .WithMessage("Unidade de medida não pode ter mais de 3 caracteres");

        RuleFor(x => x.PrecoUnitario)
            .GreaterThan(0)
            .WithMessage("Preço unitário deve ser maior que zero")
            .LessThanOrEqualTo(999999999.99m)
            .WithMessage("Preço unitário muito alto");

        RuleFor(x => x.Centro)
            .MaximumLength(4)
            .WithMessage("Centro não pode ter mais de 4 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Centro));

        RuleFor(x => x.Deposito)
            .MaximumLength(4)
            .WithMessage("Depósito não pode ter mais de 4 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Deposito));
    }
}
