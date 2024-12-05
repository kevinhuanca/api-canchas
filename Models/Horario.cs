public class Horario
{
    public int Id { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public int DiaId { get; set; }
    public Dia? Dia { get; set; }
}