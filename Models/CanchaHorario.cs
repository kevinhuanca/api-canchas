public class CanchaHorario
{
    public int Id { get; set; }
    public int CanchaId { get; set; }
    public Cancha? Cancha { get; set; }
    public int HorarioId { get; set; }
    public Horario? Horario { get; set; }
}