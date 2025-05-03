namespace backendConsumoE.Dtos
{
    public class HogarDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreTipo { get; set; } // Nuevo nombre descriptivo de la categoría
        public int IdUsuario { get; set; }
        public int IdTipo { get; set; } // Si también quieres enviar el ID del tipo
    }
}
