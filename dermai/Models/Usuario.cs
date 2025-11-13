using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;

namespace dermai.Models;
    public class Usuario
    {
        [JsonProperty]
        public string Nombre {get; private set;}
        [JsonProperty]
        public string Email {get; private set;}
        [JsonProperty]
        public string Contraseña {get; private set;}
        [JsonProperty]
        public DateTime FechaDeNacimiento {get; private set;}
        [JsonProperty]
        public int IdPerfil {get; private set;}

        public Usuario() { }
        
        public Usuario (string Nombre, string Email,string Contraseña, DateTime FechaDeNacimiento, int IdPerfil)
        {
            this.Nombre = Nombre;
            this.Email = Email;
            this.Contraseña = Contraseña;
            this.FechaDeNacimiento = FechaDeNacimiento;
            this.IdPerfil = IdPerfil;
        }
    }