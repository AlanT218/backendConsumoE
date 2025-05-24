using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using backendConsumoE.Dtos;

namespace backendConsumoE.Utilities
{
    public class EmailConfigUtility
    {
        static EmailConfigUtility()
        {
            // Forzar TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private readonly ProviderSettings _gmailSettings;
        private readonly ProviderSettings _outlookSettings;
        private readonly IWebHostEnvironment _env;

        public EmailConfigUtility(IConfiguration config, IWebHostEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));

            // Cargar configuraciones de gmail y outlook desde appsettings.json
            _gmailSettings = config.GetSection("EmailSettings:Gmail").Get<ProviderSettings>()
                               ?? throw new ArgumentException("Faltan las configuraciones de Gmail en appsettings.json");
            _outlookSettings = config.GetSection("EmailSettings:Outlook").Get<ProviderSettings>()
                                 ?? throw new ArgumentException("Faltan las configuraciones de Outlook en appsettings.json");
        }

        public void EnviarCorreo(string destinatario, string asunto, int templateType, string nombre)
        {
            if (string.IsNullOrWhiteSpace(destinatario) || !EsCorreoValido(destinatario))
                throw new ArgumentException("El correo proporcionado no es válido.");

            // Elegir proveedor: si destinatario institucional, usar Outlook, si no Gmail
            bool esInstitucional = destinatario.EndsWith("@ucundinamarca.edu.co", StringComparison.OrdinalIgnoreCase);
            var settings = esInstitucional ? _outlookSettings : _gmailSettings;

            string cuerpoHtml = ObtenerPlantilla(templateType, nombre);
            using var message = new MailMessage()
            {
                From = new MailAddress(settings.User, "Opti Energy"),
                Subject = asunto,
                IsBodyHtml = true,
                Body = cuerpoHtml,
                Priority = MailPriority.High
            };
            message.To.Add(destinatario);
            message.Headers.Add("X-Priority", "1");
            message.Headers.Add("X-MSMail-Priority", "High");
            message.Headers.Add("Importance", "High");

            // Imagen embebida
            string logoPath = MapPath("~/Imagenes/LogoGestion.png");
            if (!File.Exists(logoPath))
                throw new FileNotFoundException("No se encontró la imagen del logo.", logoPath);

            var htmlView = AlternateView.CreateAlternateViewFromString(cuerpoHtml, null, MediaTypeNames.Text.Html);
            var logoResource = new LinkedResource(logoPath, MediaTypeNames.Image.Png)
            {
                ContentId = "LogoOptiEnergy",
                TransferEncoding = TransferEncoding.Base64
            };
            htmlView.LinkedResources.Add(logoResource);
            message.AlternateViews.Add(htmlView);

            using var smtp = new SmtpClient(settings.Host, settings.Port)
            {
                EnableSsl = settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(settings.User, settings.Password),
                Timeout = 20000
            };

            try
            {
                smtp.Send(message);
            }
            catch (SmtpException ex)
            {
                throw new InvalidOperationException($"Error al enviar el correo: {ex.StatusCode}", ex);
            }
        }

        private bool EsCorreoValido(string correo)
        {
            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(correo, pattern, RegexOptions.IgnoreCase);
        }

        private string ObtenerPlantilla(int templateType, string nombre)
        {
            return templateType switch
            {
                1 => ObtenerPlantillaBienvenida(nombre),
                _ => throw new ArgumentException("Tipo de plantilla no válido.")
            };
        }

        private string ObtenerPlantillaBienvenida(string nombre)
        {
            return $@"
            <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' align='center'>
                    <tr>
                        <td align='center'>
                            <table width='700' style='background-color: #ffffff; border-collapse: collapse;'>
                                <tr>
                                    <td style='background-color: #8dc63f; padding: 20px; text-align: center;'>
                                        <img src='cid:LogoOptiEnergy' alt='Logo Gestión' style='max-width: 150px; display: block; margin: auto;' />
                                        <h1 style='color: white; margin-top: 15px;'>Hola {nombre}!</h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding: 30px; text-align: center; color: #333333; font-size: 18px;'>
                                        <p>¡Gracias por unirte a nuestra comunidad!</p>
                                        <p>En Opti Energy, nos dedicamos a ayudarte a monitorear y optimizar tu consumo energético.</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding: 20px; text-align: center;'>
                                        <img src='https://i.imgur.com/kFFVLb0.jpeg' alt='Imagen Final' style='max-width: 100%; height: auto;' />
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding: 20px; text-align: center;'>
                                        <img src='https://i.imgur.com/u70UA6A.png' alt='Pie de página' style='max-width: 100%; height: auto;' />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>";
        }

        private string MapPath(string virtualPath)
        {
            if (string.IsNullOrWhiteSpace(virtualPath) || !virtualPath.StartsWith("~/"))
                throw new ArgumentException("La ruta virtual debe comenzar con '~/'.", nameof(virtualPath));

            string relative = virtualPath.Substring(2).Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(_env.ContentRootPath, relative);
        }
    }

}
