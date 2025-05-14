using System.ComponentModel.DataAnnotations;
namespace EVS4.src.models
{
    public class Producto
    {
        [StringLength(50)]
        public string Nombre {get;set;}
        [Key]
        [StringLength(30)]
        public string SKU {get;set;}
        [Range(1,int.MaxValue)]
        public int Precio {get;set;}
        [Range(0,int.MaxValue)]
        public int Stock {get;set;}
        public bool Activo {get;set;}=true;
    }
}