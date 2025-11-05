using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;
using System;
using System.Data;     

namespace dermai.Models;

public class BD
{
    private static string _connectionString = @"Server=localhost; DataBase=Dermai; Integrated Security=True; TrustServerCertificate=True;";
    
    public static Usuario Login (string Email, string Contraseña)
   {
        Usuario usu = null;
        using(SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM Usuario WHERE Email = @pEmail AND Contraseña = @pContraseña";
            usu = connection.QueryFirstOrDefault<Usuario>(query, new {pEmail = Email, pContraseña = Contraseña});
        }
        return usu;
   }

    public static bool ValidarRegistro(string Email)
   {
        using(SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(*) FROM Usuarios WHERE Email = @pEmail";
            int count = connection.QueryFirstOrDefault<int>(query, new { pEmail = Email});
            return count >0;
        }
   }

   public static void Registrarse(Usuario usuario)
   {
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        var parametros = new
                {
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Contraseña = usuario.Contraseña,
                    FechaDeNacimiento = usuario.FechaDeNacimiento,
                    IdPerfil = usuario.IdPerfil
                };

        connection.Execute("SP_RegistrarUsuario", parametros, commandType: CommandType.StoredProcedure);
    }
    }

    public static Usuario ObtenerUsuarioPorEmail(string email)
        {
            using (SqlConnection db = new SqlConnection(_connectionString))
            {
                return db.QueryFirstOrDefault<Usuario>(
                    "SELECT * FROM Usuario WHERE Email = @Email",
                    new { Email = email });
            }
        }

   public static int CrearPerfil(Perfil perfil)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
        string sp = "SP_CrearPerfil";
        var idPerfil = connection.QuerySingleOrDefault<int>(sp, new
        {
            IdUsuario = perfil.IdUsuario,
            CaracteristicasPiel = perfil.CaracteristicasPiel,
            PreferenciaProducto = perfil.PreferenciaProducto,
            Presupuesto = perfil.Presupuesto,
            FrecuenciaRutina = perfil.FrecuenciaRutina
        }, commandType: System.Data.CommandType.StoredProcedure);

        return idPerfil;
        }
    }
}