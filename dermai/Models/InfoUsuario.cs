using System;
using Newtonsoft.Json;

namespace dermai.Models
{
    public class InfoUsuario
    {
        [JsonProperty]
        public int IdUsuario { get; set; }

        [JsonProperty]
        public List<string> Caracteristicas { get; set; }

        [JsonProperty]
        public List<string> Preferencias { get; set; }

        [JsonProperty]
        public string Presupuesto { get; set; }

        [JsonProperty]
        public string Frecuencia { get; set; }
    }
}