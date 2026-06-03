using System.Text.Json.Serialization;

namespace DsGestor.Domain.Entities;

public class Departamento
{
    public int Id { get; set; }
    public string Descricao { get; set; } = null!;

}
