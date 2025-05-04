using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace backendConsumoE.Dtos
{
    public class RequestUserDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Contra { get; set; } = string.Empty;

        public int IdRol { get; set; }
        public int IdEstado { get; set; }
    }

}
