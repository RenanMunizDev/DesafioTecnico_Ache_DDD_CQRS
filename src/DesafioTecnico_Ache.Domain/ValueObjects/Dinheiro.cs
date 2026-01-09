using DesafioTecnico_Ache.Domain.Common;

namespace DesafioTecnico_Ache.Domain.ValueObjects;

/// <summary>
/// Value Object representando valor monetário (SAP SD)
/// </summary>
public sealed class Dinheiro : ValueObject
{
    public decimal Valor { get; private set; }
    public string Moeda { get; private set; }

    private Dinheiro() 
    {
        Moeda = string.Empty;
    }

    public Dinheiro(decimal valor, string moeda = "BRL")
    {
        if (valor < 0)
            throw new ArgumentException("Valor não pode ser negativo", nameof(valor));

        if (string.IsNullOrWhiteSpace(moeda))
            throw new ArgumentException("Moeda não pode ser vazia", nameof(moeda));

        Valor = valor;
        Moeda = moeda;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Valor;
        yield return Moeda;
    }

    public static Dinheiro operator +(Dinheiro a, Dinheiro b)
    {
        if (a.Moeda != b.Moeda)
            throw new InvalidOperationException("Não é possível somar valores com moedas diferentes");

        return new Dinheiro(a.Valor + b.Valor, a.Moeda);
    }

    public static Dinheiro operator *(Dinheiro dinheiro, decimal multiplicador)
    {
        return new Dinheiro(dinheiro.Valor * multiplicador, dinheiro.Moeda);
    }
}
