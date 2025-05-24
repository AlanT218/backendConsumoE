using backendConsumoE.Dtos;
using backendConsumoE.Utilities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backendConsumoE.Repositories
{
        public class UserRepository
        {
            private readonly DbContextUtility _dbContextUtility;

            public UserRepository(DbContextUtility dbContextUtility)
            {
                _dbContextUtility = dbContextUtility ?? throw new ArgumentNullException(nameof(dbContextUtility));
            }

            public async Task<List<UserDto>> ObtenerUsuarios()
            {
                var usuarios = new List<UserDto>();

                const string sql = @"
                SELECT U.id_usuario, U.nombre, U.apellido, U.correo,
                       U.id_estado, S.nombre AS estado_nombre
                FROM [bdGestion].[dbo].[usuario] U
                JOIN [bdGestion].[dbo].[ESTADO] S ON S.id_estado = U.id_estado";

                try
                {
                    using var connection = _dbContextUtility.GetOpenConnection();
                    using var command = new SqlCommand(sql, connection);
                    using var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        usuarios.Add(new UserDto
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Correo = reader.GetString(3),
                            IdEstado = reader.GetInt32(4),
                            EstadoNombre = reader.GetString(5),
                            // Se omiten IdRol y RolNombre ya que no existen
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener los usuarios: " + ex.Message);
                }

                return usuarios;
            }

            public async Task<int> RegistrarUsuario(RequestUserDto usuario)
            {
                const string sql = @"
                INSERT INTO [bdGestion].[dbo].[usuario] 
                (nombre, apellido, correo, contra, id_estado) 
                VALUES (@Nombre, @Apellido, @Correo, @Contra, @IdEstado)";

                try
                {
                    using var connection = _dbContextUtility.GetOpenConnection();
                    using var command = new SqlCommand(sql, connection);

                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@Correo", usuario.Correo);
                    command.Parameters.AddWithValue("@Contra", usuario.Contra);
                    command.Parameters.AddWithValue("@IdEstado", 1); // Estado activo por defecto

                    return await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al registrar el usuario en la base de datos: " + ex.Message);
                }
            }
        public async Task<bool> CorreoExiste(string correo)
        {
            const string sql = "SELECT COUNT(1) FROM [bdGestion].[dbo].[usuario] WHERE correo = @Correo";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Correo", correo);

            var result = (int)await command.ExecuteScalarAsync();
            return result > 0;
        }

        public async Task<UserDto> Login(RequestInicioSesionDto request)
        {
            const string sql = @"
            SELECT U.id_usuario, U.nombre
            FROM [bdGestion].[dbo].[usuario] U 
            WHERE U.correo = @Correo AND U.contra = @Contra";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@Correo", request.Correo);
                command.Parameters.AddWithValue("@Contra", request.Contra);

                using var reader = await command.ExecuteReaderAsync();
                if (reader.Read())
                {
                    return new UserDto
                    {
                        Id = Convert.ToInt32(reader["id_usuario"]),
                        Nombre = reader["nombre"].ToString()
                        // RolNombre ya no se retorna porque no hay campo de rol en usuario
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en la base de datos: " + ex.Message);
            }

            return null;
        }
    }
}





