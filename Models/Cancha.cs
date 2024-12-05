public class Cancha
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int Capacidad { get; set; }
    public decimal Precio { get; set; }
    public string Imagen { get; set; } = "";
    public bool Estado { get; set; }
    public int TipoId { get; set; }
    public Tipo? Tipo { get; set; }
}