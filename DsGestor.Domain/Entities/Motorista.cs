namespace DsGestor.Domain.Entities;

public class Motorista
{
    public int Matricula { get; set; }
    public string Nome { get; set; } = null!;
    public string? Cpf { get; set; }
    public string? Situacao { get; set; }
}