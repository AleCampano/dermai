using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;

namespace dermai;

public class BD
{
    private static string _connectionString = @"Server=localhost; DataBase=Dermai; Integrated Security=True; TrustServerCertificate=True;";
    
    public static Usuario Login (string Email, string Contraseña)
   {
        Usuario usu = null;
        using(SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT Email, Contraseña FROM Usuarios WHERE Email = @pEmail AND Contraseña = @pContraseña";
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
        connection.Execute("SP_RegistrarUsuario",
            new
            {
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Contraseña = usuario.Contraseña,
                FechaDeNacimiento = usuario.FechaDeNacimiento,
                IdPerfil = usuario.IdPerfil
            },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }
    }

   public static int CrearPerfil(Perfil perfil)
    {
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        string sp = "SP_CrearPerfil";
        var idPerfil = connection.QuerySingleOrDefault<int>(sp, new
        {
            CaracteristicasPiel = perfil.CaracteristicasPiel,
            PreferenciaProducto = perfil.PreferenciaProducto,
            Presupuesto = perfil.Presupuesto,
            FrecuenciaRutina = perfil.FrecuenciaRutina
        }, commandType: System.Data.CommandType.StoredProcedure);

        return idPerfil;
    }
    }
}