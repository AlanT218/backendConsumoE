namespace backendConsumoE.Dtos
{
    public class HogarDto
    {
        public int Id { get; set; } // Id del hogar
        public string Nombre { get; set; } // Nombre del hogar
        public int IdUsuario { get; set; } // Usuario que lo registra

        public int IdTipo { get; set; } // FK al tipo de hogar
        public string NombreTipo { get; set; } // Nombre del tipo de hogar (para lectura)
    }

}
