using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;

namespace dermai;

public class Feedback
{
     [JsonProperty]
    public int IdUsuario {get; private set;}
    [JsonProperty]
    public int IdRutina {get; private set;}
}