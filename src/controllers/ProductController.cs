using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EVS4.src.data;
using EVS4.src.dto;
using EVS4.src.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EVS4.src.controllers
{
    [ApiController]
    [Authorize]
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public ProductController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProducts([FromQuery]int page=1,[FromQuery]int pageSize=10)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return BadRequest("Page and pageSize must be positive integers.");

                var query = _context.Productos
                                    .Where(p => p.Activo)
                                    .OrderBy(p => p.SKU);
                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var products = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Items = products
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{sku}")]
        public async Task<ActionResult<Producto>> GetProduct(string sku)
        {
            try
            {
                var product = await _context.Productos.FirstOrDefaultAsync(p=>p.SKU==sku &&p.Activo);
                if (product == null)
                    return NotFound();
                return product;
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> PostProduct(CreateProductoDTO productivo)
        {
            try
            {
                if (_context.Productos.Any(p => p.SKU == productivo.SKU))
                    return Conflict("Producto con este SKU ya existe.");
                if(productivo.Precio<=0 ||productivo.Stock<0) return BadRequest("Ingrese datos válidos.");
                var product= new Producto
                {
                    Nombre=productivo.Nombre,
                    SKU=productivo.SKU,
                    Precio=productivo.Precio,
                    Stock=productivo.Stock,
                    Activo=true
                };
                _context.Productos.Add(product);
                await _context.SaveChangesAsync();
                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPatch("{sku}")]
        public async Task<IActionResult> PatchProduct(string sku, [FromBody] ProductoDTO updatedProduct)
        {
            try
            {
                var product = await _context.Productos.FindAsync(sku);
                if (product == null||!product.Activo)
                    return NotFound();

                if (!string.IsNullOrEmpty(updatedProduct.Nombre))
                    product.Nombre = updatedProduct.Nombre;

                if (updatedProduct.Precio > 0)
                    product.Precio = updatedProduct.Precio;
                
                if (updatedProduct.Stock >= 0)
                    product.Stock = updatedProduct.Stock;

                TryValidateModel(product);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
    }

        [HttpDelete("{sku}")]
        public async Task<IActionResult> DeleteProduct(string sku)
        {
            try
            {
                var product = await _context.Productos.FindAsync(sku);
                if (product == null||!product.Activo)
                    return NotFound();

                product.Activo=false;
                _context.Update(product);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}