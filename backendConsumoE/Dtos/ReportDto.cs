namespace backendConsumoE.Dtos
{
    public class ReportDto
    {
        public int HogarId { get; set; }
        public DateTime GeneradoEn { get; set; }
        public List<ConsumoReporteDto> Items { get; set; }
    }
}
