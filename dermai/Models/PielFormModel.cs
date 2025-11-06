using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;

namespace dermai.Models;
    public class PielFormModel
    {
        public List<string> Caracteristicas { get; set; }
        public List<string> Preferencias { get; set; }
        public string Presupuesto { get; set; }
        public string Frecuencia { get; set; }
    }