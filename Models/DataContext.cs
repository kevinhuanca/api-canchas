using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cancha> Canchas { get; set; }
    public DbSet<Tipo> Tipos { get; set; }
    public DbSet<Horario> Horarios { get; set; }
    public DbSet<Dia> Dias { get; set; }
    public DbSet<CanchaHorario> Canchas_Horarios { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
}