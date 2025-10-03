using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly DataContext _context;

    public ReservasController(DataContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("todos")] // Listo
    public async Task<IActionResult> Todos()
    {
        try
        {
            var id = User.Claims.First(c => c.Type == "Id").Value;

            var reservas = await _context.Reservas
                .Include(r => r.Cancha)
                .Include(r => r.Cancha.Tipo)
                .Where(r => r.UsuarioId == int.Parse(id))
                .OrderByDescending(r => r.FechaHora)
                .Take(5)
                .ToListAsync();

            if (reservas.Count == 0)
                return NotFound("No se encontraron reservas.");

            return Ok(reservas);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("registrar")] // Listo
    public async Task<IActionResult> Registrar([FromBody] Reserva reserva)
    {
        try
        {
            var reservas = await _context.Reservas
                .Where(r => r.FechaHora == reserva.FechaHora && r.CanchaId == reserva.CanchaId)
                .FirstOrDefaultAsync();

            if (reservas != null)
                return BadRequest("La reserva ya existe.");

            var id = User.Claims.First(c => c.Type == "Id").Value;
            reserva.UsuarioId = int.Parse(id);

            _context.Add(reserva);
            await _context.SaveChangesAsync();

            return Ok("Reserva registrada.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}