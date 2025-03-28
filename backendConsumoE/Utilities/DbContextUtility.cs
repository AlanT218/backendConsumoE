using System.Data.SqlClient;

namespace backendConsumoE.Utilities
{
    public class DbContextUtility
    {
        static readonly string SERVER = "DESKTOP-3AKB9J2\\SQLSERVER";  // Nombre de la instancia del servidor SQL
        static readonly string DB_NAME = "bdGestion";
        static readonly string DB_USER = "bd";
        static readonly string DB_PASSWORD = "1234";

        static readonly string Conn = "server=" + SERVER + ";database=" + DB_NAME + ";user id=" + DB_USER + ";password=" + DB_PASSWORD + ";MultipleActiveResultSets=true";
        //mi conexion:
        SqlConnection Con = new SqlConnection(Conn);

        //procedimiento que abre la conexion sqlsever
        public void Connect()
        {
            try
            {
                Con.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //procedimiento que cierra la conexion sqlserver
        public void Disconnect()
        {
            Con.Close();
        }

        //funcion que devuelve la conexion sqlserver
        public SqlConnection CONN()
        {
            return Con;
        }
    }

}

