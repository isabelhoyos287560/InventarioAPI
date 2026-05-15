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
        new Producto { Id = 3, Nombre = "Teclado", Stock = 30, Precio = 120000 }
    };

    [HttpGet]
    public IActionResult GetTodos() => Ok(productos);

    [HttpGet("{id}")]
    public IActionResult GetPorId(int id)
    {
        var p = productos.FirstOrDefault(x => x.Id == id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public IActionResult Crear([FromBody] Producto p)
    {
        p.Id = productos.Max(x => x.Id) + 1;
        productos.Add(p);
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
}