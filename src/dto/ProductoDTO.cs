using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVS4.src.dto
{
    public class ProductoDTO
    {
        public string? Nombre {get;set;}
        public int Precio {get;set;}
        public int Stock {get;set;}
    }

    public class CreateProductoDTO
    {
        public string Nombre {get;set;}
        public string SKU {get;set;}
        public int Precio {get;set;}
        public int Stock {get;set;}
    }
}