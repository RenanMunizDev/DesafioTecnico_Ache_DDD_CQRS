namespace DesafioTecnico_Ache.Application.DTOs;

/// <summary>
/// DTO para endereço usado nas requisições/respostas da API
/// </summary>
public class EnderecoDto
{
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Pais { get; set; } = "Brasil";
}
