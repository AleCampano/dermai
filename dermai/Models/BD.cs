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
                    string query = "SELECT COUNT(*) FROM Usuario WHERE Email = @pEmail";
                    int count = connection.QueryFirstOrDefault<int>(query, new { pEmail = Email});
                    return count >0;
                }
        }

        public static int Registrarse(Usuario usuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sp = "SP_RegistrarUsuario";

                var parametros = new
                {
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Contraseña = usuario.Contraseña,
                    FechaDeNacimiento = usuario.FechaDeNacimiento,
                    IdPerfil = usuario.IdPerfil
                };

                int idUsuario = connection.QuerySingle<int>(sp, parametros, commandType: CommandType.StoredProcedure);
                return idUsuario;
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

        public static int CrearPerfil(int idUsuario, Perfil perfil)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sp = "SP_CrearPerfil";
                var parametros = new
                {
                    IdUsuario = idUsuario,
                    CaracteristicasPiel = perfil.CaracteristicasPiel,
                    PreferenciaProducto = perfil.PreferenciaProducto,
                    Presupuesto = perfil.Presupuesto,
                    FrecuenciaRutina = perfil.FrecuenciaRutina
                };
                
                int idPerfil = connection.QuerySingleOrDefault<int>(sp, parametros, commandType: System.Data.CommandType.StoredProcedure);
                return idPerfil;
            }
        }

        public static Perfil ObtenerPerfilPorId(int idPerfil)
        {
            using (SqlConnection db = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Perfil WHERE IdPerfil = @idPerfil";
                return db.QueryFirstOrDefault<Perfil>(sql, new { idPerfil });
            }
        }

        public static void GuardarRutina(Rutina rutina)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Rutina (Rutinas, RutinaFinal, IdUsuario) VALUES (@Rutinas, @RutinaFinal, @IdUsuario)";
                connection.Execute(query, new {
                    Rutinas = rutina.Rutinas,
                    RutinaFinal = rutina.RutinaFinal,
                    IdUsuario = rutina.IdUsuario
                });
            }
        }

        public static Rutina ObtenerRutinaPorUsuario(int idUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Rutina WHERE IdUsuario = @idUsuario";
                return connection.QueryFirstOrDefault<Rutina>(query, new { idUsuario });
            }
        }

        public static void ModificarRutina(Rutina rutina)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Rutina SET Rutinas = @Rutinas, RutinaFinal = @RutinaFinal WHERE IdUsuario = @IdUsuario";
                connection.Execute(query, new {
                    Rutinas = rutina.Rutinas,
                    RutinaFinal = rutina.RutinaFinal,
                    IdUsuario = rutina.IdUsuario
                });
            }
        }

        public static void ModificarPerfil(int idPerfil, Perfil perfil)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Perfil SET 
                    CaracteristicasPiel = @CaracteristicasPiel,
                    PreferenciaProducto = @PreferenciaProducto,
                    Presupuesto = @Presupuesto,
                    FrecuenciaRutina = @FrecuenciaRutina
                    WHERE IdPerfil = @IdPerfil";

                connection.Execute(query, new {
                    CaracteristicasPiel = perfil.CaracteristicasPiel,
                    PreferenciaProducto = perfil.PreferenciaProducto,
                    Presupuesto = perfil.Presupuesto,
                    FrecuenciaRutina = perfil.FrecuenciaRutina,
                    IdPerfil = idPerfil
                });
            }
        }

        public static void AsignarPerfilAUsuario(int idUsuario, int idPerfil)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Usuario SET IdPerfil = @IdPerfil WHERE IdUsuario = @IdUsuario";
                connection.Execute(query, new { IdPerfil = idPerfil, IdUsuario = idUsuario });
            }
        }
    }