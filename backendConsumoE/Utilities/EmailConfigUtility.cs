namespace backendConsumoE.Utilities;

using backendConsumoE.Dtos;
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
        private string User = "opti.energyst@gmail.com";
        private string Password = "iazmqwxcecesxvkk"; // Contraseña de aplicación
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

        //private string ObtenerPlantillaBienvenida(string name) => $@"
        //<!DOCTYPE html>
        //<html lang='es'>
        //<head><meta charset='UTF-8'><style>body{{font-family:Arial}}.header{{background:#800000;color:white}}</style></head>
        //<body>
        //    <div class='header'><h1>Bienvenido al sistema, {name}!</h1></div>
        //    <div><p>Nos alegra que hayas ingresado correctamente.</p></div>
        //    <footer><p>&copy; 202 G.5E CASTILLO ASOCIADOS</p></footer>
        //</body>
        //</html>";

    private string ObtenerPlantillaBienvenida(string name )
    {
        return $@"
            <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' align='center'>
                    <tr>
                        <td align='center'>
                            <table width='700' style='background-color: #ffffff; border-collapse: collapse;'>

                                <!-- Encabezado -->
                                <tr>
                                    <td style='background-color: #8dc63f; padding: 20px; text-align: center;'>
                                        <img src='https://i.imgur.com/IMOmkV4.png' alt='Logo Gestión' style='max-width: 150px; display: block; margin: auto;' />
                                        <h1 style='color: white; margin-top: 15px;'>Hola! {name}</h1>
                                    </td>
                                </tr>

                                <!-- Cuerpo del mensaje -->
                                <tr>
                                    <td style='padding: 30px; text-align: center; color: #333333; font-size: 18px;'>
                                        <p>¡Gracias por unirte a nuestra comunidad! Estamos emocionados de tenerte 
                                        con nosotros. En Opti Energy, nos dedicamos a ayudarte a monitorear y optimizar tu consumo energético, para que puedas reducir tus facturas de electricidad y usar la energía de manera más responsable.
                                        .</p>
                                        <p>A partir de ahora, puedes esperar recibir consejos prácticos para ahorrar energía, actualizaciones sobre nuestras herramientas
                                        de monitoreo y las últimas noticias sobre eficiencia energética.</p>
                                    </td>
                                </tr>

                                <!-- Imagen final -->
                                <tr>
                                    <td style='padding: 20px; text-align: center;'>
                                        <img src='https://i.imgur.com/kFFVLb0.jpeg' alt='Imagen Final' style='max-width: 100%; height: auto;' />
                                    </td>
                                </tr>

                                <!-- Footer -->
                                <tr>
                                    <td style='padding: 20px; text-align: center;'> 
                                           <img src='https://i.imgur.com/u70UA6A.png' alt='Imagen Final' style='max-width: 100%; height: auto;' />
                                    </td>
                                </tr>

                            </table>
                        </td>
                    </tr>
                </table>
            </body>";
    }
}
