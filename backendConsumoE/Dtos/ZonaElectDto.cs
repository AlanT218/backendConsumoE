namespace backendConsumoE.Dtos
{
    public class ZonaElectDto
    {
        public int IdZonaElect { get; set; }
        public int IdZona { get; set; }
        public int IdElectro { get; set; }
        public int IdHogar { get; set; }
        public double Consumo { get; set; }
        public bool Estado { get; set; }
    }
}
