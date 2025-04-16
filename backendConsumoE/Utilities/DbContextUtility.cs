using Microsoft.Data.SqlClient;
using System;

namespace backendConsumoE.Utilities
{
    public class DbContextUtility
    {
        static readonly string SERVER = "DESKTOP-3AKB9J2\\SQLSERVER";
        static readonly string DB_NAME = "bdGestion";
        static readonly string DB_USER = "bd";
        static readonly string DB_PASSWORD = "1234";

        //cadena 
        static readonly string Conn = $"Server={SERVER};Database={DB_NAME};User Id={DB_USER};Password={DB_PASSWORD};MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=True";

        public SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(Conn);
            try
            {
                Console.WriteLine("Abriendo conexión con la cadena:");
                Console.WriteLine(Conn); // Verifica que se está usando esta cadena

                connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir la conexión: {ex.Message}");
                throw;
            }
            return connection;
        }
    }
}
