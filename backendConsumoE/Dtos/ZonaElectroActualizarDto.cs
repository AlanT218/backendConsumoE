namespace backendConsumoE.Dtos
{
    public class ZonaElectroActualizarDto
    {
        public int IdZonaElectro { get; set; }  // Obligatorio para identificar el registro
        public int IdZona { get; set; }
        public int IdElectrodomestico { get; set; }
        public float Consumo { get; set; }
    }
}
