using backendConsumoE.Dtos;
using backendConsumoE.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backendConsumoE.Repositories
{
    public class DuenioCasaRepository
    {
        private readonly DbContextUtility _dbContextUtility;

        public DuenioCasaRepository(DbContextUtility dbContextUtility)
        {
            _dbContextUtility = dbContextUtility ?? throw new ArgumentNullException(nameof(dbContextUtility));
        }
        public async Task<List<HogarDto>> ObtenerTiposHogar()
        {
            var tipos = new List<HogarDto>();
            const string sql = "SELECT id_tipo, nombre_tipo FROM TIPO_HOGAR";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    tipos.Add(new HogarDto
                    {
                        IdTipo = reader.GetInt32(0),
                        NombreTipo = reader.GetString(1)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los tipos de hogar: " + ex.Message);
            }

            return tipos;
        }

        public async Task<bool> RegistrarHogar(HogarDto dto)
        {
            const string sqlInsertHogar = @"
        INSERT INTO HOGAR (nombre, id_tipo) 
        OUTPUT INSERTED.id_hogar
        VALUES (@nombre, @idTipo);";

            const string sqlInsertUsuHogar = @"
        INSERT INTO USU_HOGAR (id_usuario, id_hogar)
        VALUES (@idUsuario, @idHogar);";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var transaction = connection.BeginTransaction();

                int nuevoIdHogar;

                using (var command = new SqlCommand(sqlInsertHogar, connection, transaction))
                {
                    command.Parameters.AddWithValue("@nombre", dto.Nombre);
                    command.Parameters.AddWithValue("@idTipo", dto.IdTipo);

                    nuevoIdHogar = (int)await command.ExecuteScalarAsync();
                }

                using (var command = new SqlCommand(sqlInsertUsuHogar, connection, transaction))
                {
                    command.Parameters.AddWithValue("@idUsuario", dto.IdUsuario);
                    command.Parameters.AddWithValue("@idHogar", nuevoIdHogar);

                    await command.ExecuteNonQueryAsync();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el hogar: " + ex.Message);
            }
        }

        public async Task<List<HogarDto>> ObtenerHogaresPorUsuario(int idUsuario)
        {
            var hogares = new List<HogarDto>();

            const string sql = @"
    SELECT H.id_hogar, H.nombre, TH.nombre_tipo, UH.id_usuario, H.id_tipo
    FROM HOGAR H
    INNER JOIN USU_HOGAR UH ON UH.id_hogar = H.id_hogar
    INNER JOIN TIPO_HOGAR TH ON H.id_tipo = TH.id_tipo
    WHERE UH.id_usuario = @idUsuario";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@idUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    hogares.Add(new HogarDto
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        NombreTipo = reader.GetString(2),
                        IdUsuario = reader.GetInt32(3),
                        IdTipo = reader.GetInt32(4)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los hogares: " + ex.Message);
            }

            return hogares;
        }

        public async Task<List<ZonaDto>> ObtenerZonasAsync()
        {
            var zonas = new List<ZonaDto>();

            const string sql = @"
        SELECT id_zona, nombre
        FROM [bdGestion].[dbo].[ZONA]";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    zonas.Add(new ZonaDto
                    {
                        IdZona = reader.GetInt32(0),
                        Nombre = reader.GetString(1)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las zonas: " + ex.Message);
            }

            return zonas;
        }
        public async Task<List<ElectrodomesticoDto>> ObtenerElectrodomesticosAsync()
        {
            var electrodomesticos = new List<ElectrodomesticoDto>();

            const string sql = @"
        SELECT id_electro, nombre
        FROM [bdGestion].[dbo].[ELECTRODOMESTICO]";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    electrodomesticos.Add(new ElectrodomesticoDto
                    {
                        IdElectro = reader.GetInt32(0),
                        Nombre = reader.GetString(1)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los electrodomésticos: " + ex.Message);
            }

            return electrodomesticos;
        }

        public async Task InsertarZonaElectroAsync(ZonaElectDto nuevaZonaElect)
        {
            const string sql = @"
            INSERT INTO ZONA_ELECT (id_zona, id_electro, id_hogar, consumo, estado)
            VALUES (@IdZona, @IdElectro, @IdHogar, @Consumo, @Estado)";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@IdZona", nuevaZonaElect.IdZona);
                command.Parameters.AddWithValue("@IdElectro", nuevaZonaElect.IdElectro);
                command.Parameters.AddWithValue("@IdHogar", nuevaZonaElect.IdHogar);
                command.Parameters.AddWithValue("@Consumo", nuevaZonaElect.Consumo);
                command.Parameters.AddWithValue("@Estado", false); // siempre falso

                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar en la tabla ZONA_ELECT: " + ex.Message);
            }
        }

        // LISTAR electrodomésticos de un hogar
        public async Task<List<ZonaElectroVistaDto>> ObtenerZonaElectPorHogarAsync(int idHogar)
        {
            const string sql = @"
            SELECT ZE.id_zona_elect, Z.nombre AS NombreZona, E.nombre AS NombreElectrodomestico, ZE.consumo
            FROM ZONA_ELECT ZE
            INNER JOIN ZONA Z ON ZE.id_zona = Z.id_zona
            INNER JOIN ELECTRODOMESTICO E ON ZE.id_electro = E.id_electro
            WHERE ZE.id_hogar = @IdHogar";

            var lista = new List<ZonaElectroVistaDto>();

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@IdHogar", idHogar);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    lista.Add(new ZonaElectroVistaDto
                    {
                        IdZonaElect = reader.GetInt32(0),
                        NombreZona = reader.GetString(1),
                        NombreElectrodomestico = reader.GetString(2),
                        Consumo = Convert.ToSingle(reader.GetDouble(3)) // Por si consumo es DOUBLE en la BD
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los electrodomésticos del hogar: " + ex.Message);
            }

            return lista;
        }


        public async Task ActualizarZonaElectroAsync(int idZonaElect, ZonaElectroActualizarDto dto)
        {
            const string sql = @"
            UPDATE ZONA_ELECT
            SET id_zona = @IdZona,
                id_electro = @IdElectro,
                consumo = @Consumo,
                estado = 0
            WHERE id_zona_elect = @IdZonaElect";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@IdZonaElect", idZonaElect);
                command.Parameters.AddWithValue("@IdZona", dto.IdZona);
                command.Parameters.AddWithValue("@IdElectro", dto.IdElectro);
                command.Parameters.AddWithValue("@Consumo", dto.Consumo);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    throw new Exception("No se encontró el registro para actualizar.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar en la tabla ZONA_ELECT: " + ex.Message);
            }
        }


        // ELIMINAR un electrodoméstico
        public async Task<bool> EliminarZonaElectAsync(int idZonaElect)
        {
            const string sql = @"
            DELETE FROM ZONA_ELECT
            WHERE id_zona_elect = @IdZonaElect";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@IdZonaElect", idZonaElect);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el electrodoméstico de la zona: " + ex.Message);
            }
        }

        public async Task CambiarEstadoElectrodomesticoAsync(CambioEstadoDto dto)
        {
            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();

                // Iniciar transacción
                using var transaction = connection.BeginTransaction();

                // Obtener estado actual
                const string selectSql = @"SELECT estado FROM ZONA_ELECT WHERE id_zona_elect = @IdZonaElect";
                using var selectCmd = new SqlCommand(selectSql, connection, transaction);
                selectCmd.Parameters.AddWithValue("@IdZonaElect", dto.IdZonaElect);

                var estadoActual = (bool?)await selectCmd.ExecuteScalarAsync();

                if (estadoActual == null)
                    throw new Exception("ZonaElectro no encontrada.");

                if (estadoActual == dto.NuevoEstado)
                    throw new Exception("El estado ya se encuentra como solicitado.");

                // Actualizar estado
                const string updateSql = @"UPDATE ZONA_ELECT SET estado = @NuevoEstado WHERE id_zona_elect = @IdZonaElect";
                using var updateCmd = new SqlCommand(updateSql, connection, transaction);
                updateCmd.Parameters.AddWithValue("@NuevoEstado", dto.NuevoEstado);
                updateCmd.Parameters.AddWithValue("@IdZonaElect", dto.IdZonaElect);
                await updateCmd.ExecuteNonQueryAsync();

                // Insertar o actualizar en CONSUMO
                if (dto.NuevoEstado) // ENCENDER
                {
                    const string insertConsumo = @"
                INSERT INTO CONSUMO (id_zona_elect, fecha_inicio, id_user_inicio)
                VALUES (@IdZonaElect, GETDATE(), @IdUsuario)";
                    using var insertCmd = new SqlCommand(insertConsumo, connection, transaction);
                    insertCmd.Parameters.AddWithValue("@IdZonaElect", dto.IdZonaElect);
                    insertCmd.Parameters.AddWithValue("@IdUsuario", dto.IdUsuario);
                    await insertCmd.ExecuteNonQueryAsync();
                }
                else // APAGAR
                {
                    const string updateConsumo = @"
                UPDATE CONSUMO
                SET fecha_fin = GETDATE(), id_user_fin = @IdUsuario
                WHERE id_zona_elect = @IdZonaElect AND fecha_fin IS NULL";
                    using var updateConsumoCmd = new SqlCommand(updateConsumo, connection, transaction);
                    updateConsumoCmd.Parameters.AddWithValue("@IdZonaElect", dto.IdZonaElect);
                    updateConsumoCmd.Parameters.AddWithValue("@IdUsuario", dto.IdUsuario);
                    int rows = await updateConsumoCmd.ExecuteNonQueryAsync();
                    if (rows == 0)
                        throw new Exception("No se encontró un consumo activo para cerrar.");
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cambiar el estado del electrodoméstico: " + ex.Message);
            }
        }
        public async Task<bool> ObtenerEstadoZonaElectAsync(int idZonaElect)
        {
            using var connection = _dbContextUtility.GetOpenConnection();
            const string sql = @"SELECT estado FROM ZONA_ELECT WHERE id_zona_elect = @IdZonaElect";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@IdZonaElect", idZonaElect);

            var result = await command.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
                throw new Exception("ZonaElectro no encontrada.");

            return (bool)result;
        }
        public List<ConsumoReporteDto> ObtenerDatosReporteConsumo(int idHogar)
        {
            const string sql = @"
        SELECT 
            z.Nombre                AS NombreZona,
            e.Nombre                AS NombreElectrodomestico,
            ze.Consumo              AS Consumo,
            SUM(DATEDIFF(SECOND, c.fecha_inicio, c.fecha_fin) / 3600.0 * ze.Consumo) AS ConsumoWh
        FROM ZONA_ELECT ze
        INNER JOIN CONSUMO c 
            ON ze.id_zona_elect = c.id_zona_elect
        INNER JOIN ZONA z 
            ON ze.id_zona = z.id_zona
        INNER JOIN ELECTRODOMESTICO e 
            ON ze.id_electro = e.id_electro
        WHERE ze.id_hogar = @idHogar
          AND c.fecha_fin IS NOT NULL
        GROUP BY z.Nombre, e.Nombre, ze.Consumo;
        ";

            var result = new List<ConsumoReporteDto>();

            using var connection = _dbContextUtility.GetOpenConnection();
            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@idHogar", idHogar);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new ConsumoReporteDto
                {
                    NombreZona = reader.GetString(0),
                    NombreElectrodomestico = reader.GetString(1),
                    Consumo = reader.GetDouble(2),
                    ConsumoWh = reader.GetDouble(3)
                });
            }

            return result;
        }
        public async Task<List<RecomendacionDto>> ObtenerTodasRecomendacionesAsync()
        {
            const string sql = @"
            SELECT r.id_recomendacion AS IdRecomendacion,
                   e.nombre          AS NombreElectro,
                   r.texto           AS Texto
            FROM RECOMENDACION r
            INNER JOIN ELECTRODOMESTICO e ON r.id_electro = e.id_electro
            ORDER BY e.nombre;";

            var lista = new List<RecomendacionDto>();
            using var connection = _dbContextUtility.GetOpenConnection();
            using var cmd = new SqlCommand(sql, connection);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new RecomendacionDto
                {
                    IdRecomendacion = reader.GetInt32(0),
                    NombreElectro = reader.GetString(1),
                    Texto = reader.GetString(2)
                });
            }
            return lista;
        }
    }
}