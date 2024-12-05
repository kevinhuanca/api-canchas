public class Reserva
{
    public int Id { get; set; }
    public DateTime FechaHora { get; set; }
    public decimal Precio { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public int CanchaId { get; set; }
    public Cancha? Cancha { get; set; }
}