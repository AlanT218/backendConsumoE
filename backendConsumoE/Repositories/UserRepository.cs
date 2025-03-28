using backendConsumoE.Dtos;
using backendConsumoE.Utilities;
using System.Data.SqlClient;
public class UserRepository
{
    public async Task<List<UserDto>> ObtenerUsuarios()
    {
        List<UserDto> listUserDto = new List<UserDto>();

        string sql = "SELECT U.id_usuario, U.nombre, U.apellido, U.correo, " +
                     "U.id_rol, R.nombre AS rol_nombre, U.id_estado, S.nombre AS estado_nombre " +
                     "FROM [bdGestion].[dbo].[usuario] U " +
                     "JOIN [bdGestion].[dbo].[ROL] R ON R.id_rol = U.id_rol " +
                     "JOIN [bdGestion].[dbo].[ESTADO] S ON S.id_estado = U.id_estado";

        DbContextUtility Connection = new DbContextUtility();
        Connection.Connect();

        using (SqlCommand command = new SqlCommand(sql, Connection.CONN()))
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    listUserDto.Add(new UserDto
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Apellido = reader.GetString(2),
                        Correo = reader.GetString(3),
                        IdRol = reader.GetInt32(4),
                        RolNombre = reader.GetString(5),
                        IdEstado = reader.GetInt32(6),
                        EstadoNombre = reader.GetString(7)
                    });
                }

            }

            Connection.Disconnect();
            return listUserDto;
        }
    }
    public void RegistrarUsuario(UserDto usuario)
    {
        string sql = "INSERT INTO [bdGestion].[dbo].[usuario] (nombre, apellido, correo, contra, id_rol, id_estado) " +
                     "VALUES (@Nombre, @Apellido, @Correo, @Contra, @IdRol, @IdEstado)";

        DbContextUtility Connection = new DbContextUtility();
        Connection.Connect();

        try
        {
            using (SqlCommand command = new SqlCommand(sql, Connection.CONN()))
            {
                command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                command.Parameters.AddWithValue("@Correo", usuario.Correo);
                command.Parameters.AddWithValue("@Contra", usuario.Contra);
                command.Parameters.AddWithValue("@IdRol", 2); // Se ignora lo recibido en la petición
                command.Parameters.AddWithValue("@IdEstado", 1); // Se ignora lo recibido en la petición

                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al registrar el usuario en la base de datos: " + ex.Message);
        }
        finally
        {
            Connection.Disconnect();
        }
    }
    public async Task<UserDto> Login(UserDto user)
    {
        UserDto userResult = new UserDto();

        string sql = "SELECT U.nombre, R.nombre AS rol_nombre " +
                     "FROM [bdGestion].[dbo].[usuario] U " +
                     "JOIN [bdGestion].[dbo].[ROL] R ON R.id_rol = U.id_rol " +
                     "WHERE U.correo = @Correo AND U.contra = @Contra;";

        DbContextUtility Connection = new DbContextUtility();
        Connection.Connect();

        try
        {
            using (SqlCommand command = new SqlCommand(sql, Connection.CONN()))
            {
                command.Parameters.AddWithValue("@Correo", user.Correo);
                command.Parameters.AddWithValue("@Contra", user.Contra);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        userResult.Nombre = reader.GetString(reader.GetOrdinal("nombre"));
                        userResult.RolNombre = reader.GetString(reader.GetOrdinal("rol_nombre"));
                        userResult.Mensaje = $"{userResult.Nombre}, bienvenido. Tu rol es {userResult.RolNombre}";
                    }
                    else
                    {
                        userResult.Mensaje = "Correo o contraseña incorrectos.";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error en la base de datos: " + ex.Message);
        }
        finally
        {
            Connection.Disconnect();
        }

        return userResult;
    }
}






