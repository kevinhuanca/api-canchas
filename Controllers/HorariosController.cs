using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class HorariosController : ControllerBase
{
    private readonly DataContext _context;

    public HorariosController(DataContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("disponible/{idCancha}/{fecha}")]
    public async Task<IActionResult> Disponible(int idCancha, DateTime fecha)
    {
        try
        {
            int numeroDiaSemana = (int)fecha.DayOfWeek;
            numeroDiaSemana = numeroDiaSemana == 0 ? 1 : numeroDiaSemana + 1;

            var horarioDisponible = await _context.Canchas_Horarios
                .Where(ch => ch.CanchaId == idCancha)
                .Include(ch => ch.Horario)
                .Include(ch => ch.Horario.Dia)
                .Where(ch => ch.Horario.DiaId == numeroDiaSemana)
                .Select(ch => new
                {
                    ch.Horario.HoraInicio,
                    ch.Horario.HoraFin
                })
                .FirstOrDefaultAsync();

            if (horarioDisponible == null)
                return NotFound("No hay horarios disponibles.");

            var horas = new List<HoraView>();
            var horaInicio = horarioDisponible.HoraInicio;
            var horaFin = horarioDisponible.HoraFin;

            var reservas = await _context.Reservas
                .Where(r => r.CanchaId == idCancha && r.FechaHora.Date == fecha.Date)
                .Select(r => new
                {
                    r.FechaHora
                })
                .ToListAsync();

            var horaActual = DateTime.Now.TimeOfDay;
            var horasReservadas = new HashSet<int>(reservas.Select(r => r.FechaHora.Hour));

            for (var hora = horaInicio; hora.IsBetween(horaInicio, horaFin); hora = hora.AddHours(1))
            {
                if (fecha.Date == DateTime.Today && hora.ToTimeSpan() < horaActual)
                    continue;

                if (!horasReservadas.Contains(hora.Hour))
                {
                    horas.Add(new HoraView
                    {
                        Hora = hora,
                        Seleccionado = false
                    });
                }
            }

            if (horas.Count == 0)
                return NotFound("No hay horarios disponibles.");

            return Ok(horas);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}