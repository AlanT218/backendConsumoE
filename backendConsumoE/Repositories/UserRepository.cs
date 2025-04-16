using backendConsumoE.Dtos;
using backendConsumoE.Utilities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserRepository
{
    private readonly DbContextUtility _dbContextUtility;

    public UserRepository()
    {
        _dbContextUtility = new DbContextUtility();
    }

    // Método para obtener los usuarios
    public async Task<List<UserDto>> ObtenerUsuarios()
    {
        List<UserDto> listUserDto = new List<UserDto>();
        string sql = "SELECT U.id_usuario, U.nombre, U.apellido, U.correo, " +
                     "U.id_rol, R.nombre AS rol_nombre, U.id_estado, S.nombre AS estado_nombre " +
                     "FROM [bdGestion].[dbo].[usuario] U " +
                     "JOIN [bdGestion].[dbo].[ROL] R ON R.id_rol = U.id_rol " +
                     "JOIN [bdGestion].[dbo].[ESTADO] S ON S.id_estado = U.id_estado";

        try
        {
            // Usamos 'using' para garantizar que la conexión se cierre automáticamente
            using (var connection = _dbContextUtility.GetOpenConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
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
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los usuarios: " + ex.Message);
        }

        return listUserDto;
    }

    // Método para registrar un usuario
    public async Task<int> RegistrarUsuario(RequestUserDto usuario)
    {
        string sql = "INSERT INTO [bdGestion].[dbo].[usuario] (nombre, apellido, correo, contra, id_rol, id_estado) " +
                     "VALUES (@Nombre, @Apellido, @Correo, @Contra, @IdRol, @IdEstado)";

        try
        {
            // Usamos 'using' para manejar la conexión automáticamente
            using (var connection = _dbContextUtility.GetOpenConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@Correo", usuario.Correo);
                    command.Parameters.AddWithValue("@Contra", usuario.Contra);
                    command.Parameters.AddWithValue("@IdRol", 2); // Se supone que el rol es '2' por defecto
                    command.Parameters.AddWithValue("@IdEstado", 1); // Se supone que el estado es '1' por defecto

                    // Ejecutamos el comando asincrónicamente
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al registrar el usuario en la base de datos: " + ex.Message);
        }

        return 1; // Devuelve el id del nuevo usuario, o puedes modificarlo para devolver algo más relevante.
    }

    // Método para login de usuario
    public async Task<UserDto?> Login(RequestInicioSesionDto user)
    {
        UserDto? userResult = null;
        string sql = "SELECT U.nombre, R.nombre AS rol_nombre " +
                     "FROM [bdGestion].[dbo].[usuario] U " +
                     "JOIN [bdGestion].[dbo].[ROL] R ON R.id_rol = U.id_rol " +
                     "WHERE U.correo = @Correo AND U.contra = @Contra;";

        try
        {
            // Usamos 'using' para manejar la conexión automáticamente
            using (var connection = _dbContextUtility.GetOpenConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Correo", user.Correo);
                    command.Parameters.AddWithValue("@Contra", user.Contra);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            userResult = new UserDto
                            {
                                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                                RolNombre = reader.GetString(reader.GetOrdinal("rol_nombre"))
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error en la base de datos: " + ex.Message);
        }

        return userResult;
    }
}






