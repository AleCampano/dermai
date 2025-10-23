using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;

namespace dermai;

public class Rutina
{
    [JsonProperty]
    public bool Rutinas {get; private set;}
    [JsonProperty]
    public string RutinaFinal {get; private set;}
    [JsonProperty]
    public int IdUsuario {get; private set;}

public Rutina (bool Rutinas, string RutinaFinal, int IdUsuario)
{
    this.Rutinas = Rutinas;
    this.RutinaFinal = RutinaFinal;
    this.IdUsuario = IdUsuario;
}
}