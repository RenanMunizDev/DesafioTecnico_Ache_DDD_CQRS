using DesafioTecnico_Ache.Domain.Common;

namespace DesafioTecnico_Ache.Domain.ValueObjects;

/// <summary>
/// Value Object representando endereço de entrega/cobrança (SAP SD)
/// </summary>
public sealed class Endereco : ValueObject
{
    public string Logradouro { get; private set; }
    public string Numero { get; private set; }
    public string? Complemento { get; private set; }
    public string Bairro { get; private set; }
    public string Cidade { get; private set; }
    public string Estado { get; private set; }
    public string Cep { get; private set; }
    public string Pais { get; private set; }

    private Endereco() 
    {
        Logradouro = string.Empty;
        Numero = string.Empty;
        Bairro = string.Empty;
        Cidade = string.Empty;
        Estado = string.Empty;
        Cep = string.Empty;
        Pais = string.Empty;
    }

    public Endereco(string logradouro, string numero, string? complemento, 
                    string bairro, string cidade, string estado, string cep, string pais)
    {
        if (string.IsNullOrWhiteSpace(logradouro))
            throw new ArgumentException("Logradouro não pode ser vazio", nameof(logradouro));
        
        if (string.IsNullOrWhiteSpace(cidade))
            throw new ArgumentException("Cidade não pode ser vazia", nameof(cidade));

        if (string.IsNullOrWhiteSpace(cep))
            throw new ArgumentException("CEP não pode ser vazio", nameof(cep));

        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        Cep = cep;
        Pais = pais;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Logradouro;
        yield return Numero;
        yield return Complemento;
        yield return Bairro;
        yield return Cidade;
        yield return Estado;
        yield return Cep;
        yield return Pais;
    }
}
