using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CanchasController : ControllerBase
{
    private readonly DataContext _context;

    public CanchasController(DataContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("todos")] // Listo
    public async Task<IActionResult> Todos()
    {
        try
        {
            var canchas = await _context.Canchas
                .Include(c => c.Tipo)
                .OrderByDescending(c => c.Estado)
                .ToListAsync();

            return Ok(canchas);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
}