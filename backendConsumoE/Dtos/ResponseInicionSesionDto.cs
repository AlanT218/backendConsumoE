﻿namespace backendConsumoE.Dtos
{
    public class ResponseInicionSesionDto
    {
        public int IdUsuario { get; set; }
        public int Respuesta { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime TiempoExpiracion { get; set; }
    }

}
