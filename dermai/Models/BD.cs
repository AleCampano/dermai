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
                    return connection.QueryFirstOrDefault<Usuario>(query, new {pEmail = Email, pContraseña = Contraseña});
                }
        }

        public static bool ValidarRegistro(string Email)
        {
                using(SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT COUNT(*) FROM Usuario WHERE Email = @pEmail";
                    int count = connection.QueryFirstOrDefault<int>(query, new { pEmail = Email});
                    return count > 0;
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
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuario WHERE Email = @Email";
                return connection.QueryFirstOrDefault<Usuario>(query, new { Email = email });
            }
        }

       public static int ObtenerIdUsuarioPorEmail(string email)
    {
         using(SqlConnection connection = new SqlConnection(_connectionString))
    {
        string query = "SELECT IdUsuario FROM Usuario WHERE Email = @Email";
        int? idUsuario = connection.QueryFirstOrDefault<int?>(query, new { Email = email });
        return idUsuario ?? 0; // Retorna 0 si no encuentra el usuario
        }
    }
        public static void AsignarPerfilAUsuario(string email, int idPerfil)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Usuario SET IdPerfil = @IdPerfil WHERE Email = @Email";
                connection.Execute(query, new { IdPerfil = idPerfil, Email = email });
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
                
                try
                {
                    int idPerfil = connection.QuerySingle<int>(
                        sp,
                        parametros,
                        commandType: System.Data.CommandType.StoredProcedure
                    );

                    return idPerfil;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Error SQL al crear el perfil: {ex.Message}");
                    return 0; // o podrías lanzar una excepción personalizada
                }
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

        public static Perfil ObtenerPerfilPorId(int idPerfil)
        {
            using (SqlConnection db = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Perfil WHERE IdPerfil = @idPerfil";
                return db.QueryFirstOrDefault<Perfil>(sql, new { idPerfil });
            }
        }

        public static void CrearRutina(Rutina rutina)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string insertQuery = @"INSERT INTO Rutina (Rutinas, RutinaFinal, IdUsuario)
                                       VALUES (@Rutinas, @RutinaFinal, @IdUsuario)";
                connection.Execute(insertQuery, new
                {
                    Rutinas = rutina.Rutinas,
                    RutinaFinal = rutina.RutinaFinal,
                    IdUsuario = rutina.IdUsuario
                });
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

        public static Rutina ObtenerRutinaPorUsuario(int idUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Rutina WHERE IdUsuario = @idUsuario";
                return connection.QueryFirstOrDefault<Rutina>(query, new { idUsuario });
            }
        }

        public static void GuardarRutina(Rutina rutina)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string checkQuery = "SELECT COUNT(*) FROM Rutina WHERE IdUsuario = @IdUsuario";
                int count = connection.QueryFirstOrDefault<int>(checkQuery, new { IdUsuario = rutina.IdUsuario });

                if (count > 0)
                {
                    ModificarRutina(rutina);
                }
                else
                {
                    CrearRutina(rutina);
                }
            }
        }

        public static void EliminarUsuario(int idUsuario)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                
            }
        }

        public static List<Usuario> ObtenerTodosLosUsuarios()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Usuario ORDER BY IdUsuario";
                return connection.Query<Usuario>(query).ToList();
            }
        }
    }