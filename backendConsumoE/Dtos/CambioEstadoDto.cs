namespace backendConsumoE.Dtos
{
    public class CambioEstadoDto
    {
        public int IdZonaElect { get; set; }
        public bool NuevoEstado { get; set; } // true = encender, false = apagar
        public int IdUsuario { get; set; } // quien realiza la acción
    }
}

