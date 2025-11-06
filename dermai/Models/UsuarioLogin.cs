using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;

namespace dermai.Models;
public class UsuarioLoginDTO
{
    public string Email { get; set; }
    public string Contraseña { get; set; }
}