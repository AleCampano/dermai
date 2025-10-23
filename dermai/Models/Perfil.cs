using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;

namespace dermai;

public class Perfil
{
    [JsonProperty]
    public int IdUsuario {get; private set;}
    [JsonProperty]
    public string Caracteristica {get; private set;}


    public Perfil (int IdUsuario, string Caracteristica)
{
    this.IdUsuario = IdUsuario;
    this.Caracteristica = Caracteristica;
    
}
}

