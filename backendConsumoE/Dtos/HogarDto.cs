namespace backendConsumoE.Dtos
{
    public class HogarDto
    {
      public int Id { get; set; }
      public string Nombre { get; set; }
      public int IdTipo { get; set; }
      public string NombreTipo { get; set; }
      public int IdUsuario { get; set; }

      // Campos añadidos para roles
      public int IdRol { get; set; }
      public string NombreRol { get; set; }
    }

}
