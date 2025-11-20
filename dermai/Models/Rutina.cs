using Newtonsoft.Json;
using Dapper.Contrib.Extensions;

namespace dermai.Models;

public class Rutina
{
    // 🛑 AGREGAR ESTO: Marca la PK como clave, Dapper la ignorará en INSERT
    [Key]
    public int IdRutina { get; set; }
    [JsonProperty]
    public string Rutinas {get; set;}
    [JsonProperty]
    public string RutinaFinal {get; set;}
    [JsonProperty]
    public int IdUsuario {get; set;}

public Rutina() { }

public Rutina (string Rutinas, string RutinaFinal, int IdUsuario)
{
    this.Rutinas = Rutinas;
    this.RutinaFinal = RutinaFinal;
    this.IdUsuario = IdUsuario;
}
}