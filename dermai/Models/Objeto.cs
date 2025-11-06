using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dermai.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Dapper;

namespace dermai.Models;

    public static class Objeto
    {
        public static string ObjectToString<T>(T? obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static T? StringToObject<T>(string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return default;
            else
                return JsonConvert.DeserializeObject<T>(txt);
        }
        public static string ListToString<T>(List<T> lista)
        {
            return JsonConvert.SerializeObject(lista);
        }
        public static List<T>? StringToList<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;
            else
                return JsonConvert.DeserializeObject<List<T>>(json);
        }
    }