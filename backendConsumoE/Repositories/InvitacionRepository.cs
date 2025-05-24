using backendConsumoE.Dtos;
using backendConsumoE.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace backendConsumoE.Repositories
{
    public class InvitacionRepository
    {
        private readonly DbContextUtility _db;
        public InvitacionRepository(DbContextUtility dbContextUtility) => _db = dbContextUtility;


        public async Task<UsuarioPorCorreoDto> ObtenerUsuarioPorCorreoAsync(string correo)
        {
            const string sql = @"
        SELECT id_usuario, correo, nombre
        FROM USUARIO
        WHERE correo = @correo;
        ";
            using var connection = _db.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@correo", correo);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UsuarioPorCorreoDto
                {
                    IdUsuario = reader.GetInt32(0),
                    Correo = reader.GetString(1),
                    Nombre = reader.GetString(2)
                };
            }

            return null; // no encontrado
        }

        // Comprueba si ya existe invitación pendiente
        public async Task<bool> InvitacionPendienteExisteAsync(int idInvitado, int idHogar)
        {
            const string sql = @"SELECT COUNT(1) FROM INVITACION
                                 WHERE id_invitado = @id_invitado
                                   AND id_hogar    = @id_hogar
                                   AND estado      = 'Pendiente'";
            using var conn = _db.GetOpenConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id_invitado", idInvitado);
            cmd.Parameters.AddWithValue("@id_hogar", idHogar);
            return (int)await cmd.ExecuteScalarAsync() > 0;
        }

        /// <summary>
        /// Inserta una nueva invitación mediante SP.
        /// </summary>
        public async Task<bool> EnviarInvitacionAsync(int idInvitador, int idInvitado, int idHogar, int idRol)
        {
            using var conn = _db.GetOpenConnection();
            using var cmd = new SqlCommand("sp_EnviarInvitacion", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@id_invitador", idInvitador);
            cmd.Parameters.AddWithValue("@id_invitado", idInvitado);
            cmd.Parameters.AddWithValue("@id_hogar", idHogar);
            cmd.Parameters.AddWithValue("@id_rol", idRol);

            var output = new SqlParameter("@Resultado", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(output);

            try
            {
                await cmd.ExecuteNonQueryAsync();
                return (bool)output.Value;
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                // SP lanzó RAISERROR, capturamos para el servicio
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Acepta una invitación existente.
        /// </summary>
        public async Task<bool> AceptarInvitacionAsync(int idInvitacion)
        {
            using var conn = _db.GetOpenConnection();
            using var cmd = new SqlCommand("sp_AceptarInvitacion", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@id_invitacion", idInvitacion);
            var outParam = new SqlParameter("@Resultado", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outParam);

            await cmd.ExecuteNonQueryAsync();
            return (bool)outParam.Value;
        }

        public async Task<bool> RechazarInvitacionAsync(int idInvitacion)
        {
            using var conn = _db.GetOpenConnection();
            using var cmd = new SqlCommand("sp_RechazarInvitacion", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@id_invitacion", idInvitacion);
            var outParam = new SqlParameter("@Resultado", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outParam);

            await cmd.ExecuteNonQueryAsync();
            return (bool)outParam.Value;
        }
        public async Task<IEnumerable<InvitacionPendienteDto>> ListarPendientesPorInvitadoAsync(int idInvitado)
        {
            const string storedProcedure = "sp_ListarInvitacionesPendientes";
            var resultado = new List<InvitacionPendienteDto>();

            using var conn = _db.GetOpenConnection();
            using var cmd = new SqlCommand(storedProcedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id_invitado", idInvitado);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                resultado.Add(new InvitacionPendienteDto
                {
                    IdInvitacion = reader.GetInt32(reader.GetOrdinal("idInvitacion")),
                    NombreHogar = reader.GetString(reader.GetOrdinal("nombreHogar")),
                    NombreInvitador = reader.GetString(reader.GetOrdinal("nombreInvitador")),
                    FechaInvitacion = reader.GetDateTime(reader.GetOrdinal("fechaInvitacion"))
                });
            }

            return resultado;
        }
        public async Task ExpirarInvitacionesAsync()
        {
            using var conn = _db.GetOpenConnection();
            using var cmd = new SqlCommand("sp_ExpirarInvitaciones", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            await cmd.ExecuteNonQueryAsync();
        }


    }
}