using System.ComponentModel.DataAnnotations;

namespace backendConsumoE.Dtos
{
    public class RequestInicioSesionDto
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Contra { get; set; }
    }
}
