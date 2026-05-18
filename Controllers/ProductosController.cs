using InventarioAPI;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private static List<Producto> productos = new()
    {
        new Producto { Id = 1, Nombre = "Laptop", Stock = 10, Precio = 2500000 },
        new Producto { Id = 2, Nombre = "Mouse", Stock = 50, Precio = 45000 },
        new Producto { Id = 4, Nombre = "Monitor", Stock = 8, Precio = 850000 },
        new Producto { Id = 3, Nombre = "Cámara", Stock = 30, Precio = 500000 },
        new Producto { Id = 5, Nombre = "Teclado", Stock = 30, Precio = 100000 },
        new Producto { Id = 6, Nombre = "Impresora", Stock = 15, Precio = 650000 }
    };

    [HttpGet]
    public IActionResult GetTodos() => Ok(productos);

    [HttpGet("{id}")]
    public IActionResult GetPorId(int id)
    {
        var p = productos.FirstOrDefault(x => x.Id == id);
        return p is null ? NotFound() : Ok(p);
    }
    [HttpGet("buscar")]
    public IActionResult BuscarPorNombre(string nombre)
    {
        var resultado = productos
            .Where(x => x.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!resultado.Any())
            return NotFound("No se encontraron productos");

        return Ok(resultado);
    }
    
    [HttpPost]
    public IActionResult Crear([FromBody] Producto p)
    {
        p.Id = productos.Max(x => x.Id) + 1;
        productos.Add(p);
        return Ok(p);
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(int id, [FromBody] Producto actualizado)
    {
        var p = productos.FirstOrDefault(x => x.Id == id);

        if (p is null)
            return NotFound("Producto no encontrado");

        // VALIDACIONES
        if (string.IsNullOrWhiteSpace(actualizado.Nombre))
            return BadRequest("El nombre es obligatorio");

        if (actualizado.Stock < 0)
            return BadRequest("El stock no puede ser negativo");

        if (actualizado.Precio < 0)
            return BadRequest("El precio no puede ser negativo");

        p.Nombre = actualizado.Nombre;
        p.Stock = actualizado.Stock;
        p.Precio = actualizado.Precio;

        return Ok(p);
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        var p = productos.FirstOrDefault(x => x.Id == id);
        if (p is null) return NotFound();
        productos.Remove(p);
        return Ok();
    }
    [HttpPost("{id}/vender")]
    public IActionResult Vender(int id, int cantidad)
    {
        var p = productos.FirstOrDefault(x => x.Id == id);

        if (p is null)
            return NotFound("Producto no encontrado");

        if (cantidad <= 0)
            return BadRequest("La cantidad debe ser mayor a 0");

        if (p.Stock < cantidad)
            return BadRequest("Stock insuficiente");

        p.Stock -= cantidad;

        return Ok(new
        {
            mensaje = "Venta realizada correctamente",
            producto = p
        });
    }

    [HttpGet("valorTotal")]
    public IActionResult ValorTotal()
    {
        var total = productos.Sum(x => x.Precio * x.Stock);
        return Ok(new { valorTotal = total });
    }

    [HttpGet("bajoStock")]
    public IActionResult BajoStock()
    {
        var enAlerta = productos.Where(x => x.Stock < 10).ToList();
        return Ok(new { umbral = 10, cantidad = enAlerta.Count, productos = enAlerta });
    }

    [HttpPost("{id}/abastecer")]
    public IActionResult Abastecer(int id, int cantidad)
    {
        var p = productos.FirstOrDefault(x => x.Id == id);

        if (p is null)
            return NotFound("Producto no encontrado");

        if (cantidad <= 0)
            return BadRequest("La cantidad a abastecer debe ser mayor a 0");

        p.Stock += cantidad;

        return Ok(new
        {
            mensaje = "Stock abastecido correctamente",
            producto = p
        });
    }
}