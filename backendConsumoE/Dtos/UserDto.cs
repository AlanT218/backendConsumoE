using System.Text.Json.Serialization;

namespace backendConsumoE.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }  
        public string Correo { get; set; }
        public string Contra { get; set; }

        // Claves foráneas
        [JsonIgnore]
        public int IdRol { get; set; }  // FK a la tabla ROL
        [JsonIgnore]
        public int IdEstado { get; set; }  // FK a la tabla ESTADO

        // Propiedades (FK)
        public string RolNombre { get; set; }  
        public string EstadoNombre { get; set; }

        // Respuesta del login
        public int Response { get; set; }  // 1 = éxito, 0 = error
        public string Mensaje { get; set; }

    }
}
