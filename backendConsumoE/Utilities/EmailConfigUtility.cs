namespace backendConsumoE.Utilities;
using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;


    public class EmailConfigUtility
    {
        private SmtpClient cliente;
        private MailMessage email;
        private string Host = "smtp.gmail.com";
        private int Port = 587;
        private string User = "Pruebitaspss@gmail.com";
        private string Password = "ocehnkfhugyipyur"; // Contraseña de aplicación
        private bool EnabledSSL = true;

        public EmailConfigUtility()
        {
            cliente = new SmtpClient(Host, Port)
            {
                EnableSsl = EnabledSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(User, Password)
            };
        }

        public void EnviarCorreo(string destinatario, string asunto, int templateType, string name)
        {
            if (!EsCorreoValido(destinatario))
                throw new ArgumentException("El correo proporcionado no es válido.");

            try
            {
                string mensaje = ObtenerPlantilla(templateType, name);
                email = new MailMessage(User, destinatario, asunto, mensaje)
                {
                    IsBodyHtml = true
                };
                cliente.Send(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar correo: " + ex.Message);
                throw;
            }
        }

        private bool EsCorreoValido(string correo)
        {
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(correo, patron, RegexOptions.IgnoreCase);
        }

        private string ObtenerPlantilla(int templateType, string name)
        {
            return templateType switch
            {
                1 => ObtenerPlantillaBienvenida(name),
                _ => throw new ArgumentException("Tipo de plantilla no válido")
            };
        }

        private string ObtenerPlantillaBienvenida(string name) => $@"
        <!DOCTYPE html>
        <html lang='es'>
        <head><meta charset='UTF-8'><style>body{{font-family:Arial}}.header{{background:#800000;color:white}}</style></head>
        <body>
            <div class='header'><h1>Bienvenido al sistema, {name}!</h1></div>
            <div><p>Nos alegra que hayas ingresado correctamente.</p></div>
            <footer><p>&copy; 202 G.5E CASTILLO ASOCIADOS</p></footer>
        </body>
        </html>";

        
}
