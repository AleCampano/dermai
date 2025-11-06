using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;

namespace dermai.Models;
    public class Articulo
    {
        [JsonProperty]
        public int IdPerfil {get; private set;}

        public Articulo (int IdPerfil)
        {
            this.IdPerfil = IdPerfil;
        }

    }