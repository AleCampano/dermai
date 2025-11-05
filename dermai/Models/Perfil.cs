using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;


public class Perfil
{
    [JsonProperty]
    public int IdUsuario {get; private set;}
    [JsonProperty]
    public string CaracteristicasPiel {get; private set;}
    [JsonProperty]
    public string PreferenciaProducto {get; private set;}
    [JsonProperty]
    public string Presupuesto {get; private set;}
    [JsonProperty]
    public string FrecuenciaRutina {get; private set;}


    public Perfil (string CaracteristicasPiel, string PreferenciaProducto, string Presupuesto, string FrecuenciaRutina)
    {
    this.CaracteristicasPiel = CaracteristicasPiel;
    this.PreferenciaProducto = PreferenciaProducto;
    this.Presupuesto = Presupuesto;
    this.FrecuenciaRutina = FrecuenciaRutina;
    }
    public Perfil(int idUsuario, string caracteristicasPiel, string preferenciaProducto, string presupuesto, string frecuenciaRutina)
    {
        IdUsuario = idUsuario;
        CaracteristicasPiel = caracteristicasPiel;
        PreferenciaProducto = preferenciaProducto;
        Presupuesto = presupuesto;
        FrecuenciaRutina = frecuenciaRutina;
    }
}

